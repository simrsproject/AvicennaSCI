using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Temiang.Avicenna.Util
{
    public static class DynamicInvoker
    {
        // Kamus (Cache) untuk menyimpan Type dan MethodInfo yang sudah ditemukan.
        // Ini meningkatkan performa dengan menghindari pencarian berulang.
        private static readonly Dictionary<string, Type> TypeCache = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, MethodInfo> MethodCache = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
        private static readonly object _lock = new object(); // Objek pengunci untuk memastikan thread-safety saat mengakses cache.

        public static object InvokeFromString(string expression, int[] jsonParamIndexes = null)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression tidak boleh kosong.");
                
            int parenIndex = expression.IndexOf('(');
            if (parenIndex == -1)
                throw new ArgumentException("Format ekspresi tidak valid.");

            string beforeParen = expression.Substring(0, parenIndex);
            string paramSection = expression.Substring(parenIndex + 1, expression.Length - parenIndex - 2).Trim();
            paramSection = paramSection.TrimEnd(')', '"');

            int lastDot = beforeParen.LastIndexOf('.');
            if (lastDot == -1)
                throw new ArgumentException("Format ekspresi tidak valid (tidak ada method).");

            string classNameOrFull = beforeParen.Substring(0, lastDot);
            string methodNameRaw = beforeParen.Substring(lastDot + 1);

            string methodName;
            string genericTypeName = null;
            int genericStart = methodNameRaw.IndexOf('<');
            if (genericStart >= 0)
            {
                int genericEnd = methodNameRaw.IndexOf('>', genericStart);
                if (genericEnd > genericStart)
                {
                    methodName = methodNameRaw.Substring(0, genericStart);
                    genericTypeName = methodNameRaw.Substring(genericStart + 1, genericEnd - genericStart - 1);
                }
                else
                {
                    throw new ArgumentException("Format generic method tidak valid.");
                }
            }
            else
            {
                methodName = methodNameRaw;
            }

            Type type;
            lock (_lock)
            {
                if (!TypeCache.TryGetValue(classNameOrFull, out type))
                {
                    type = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => SafeGetTypes(a))
                        .FirstOrDefault(t =>
                            (string.Equals(t.FullName, classNameOrFull, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(t.Name, classNameOrFull, StringComparison.OrdinalIgnoreCase)) &&
                            t.FullName.StartsWith("Temiang.Avicenna", StringComparison.OrdinalIgnoreCase));

                    if (type == null)
                        throw new Exception($"Class '{classNameOrFull}' tidak ditemukan atau tidak diizinkan.");

                    TypeCache[classNameOrFull] = type;
                }
            }

            string[] paramStrings = string.IsNullOrWhiteSpace(paramSection)
                ? new string[0]
                : SplitParameters(paramSection).ToArray();

            MethodInfo method;
            string methodKey = $"{type.FullName}.{methodName}:{paramStrings.Length}";

            lock (_lock)
            {
                if (!MethodCache.TryGetValue(methodKey, out method))
                {
                    var candidateMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                         .Where(m => m.Name == methodName && m.GetParameters().Length == paramStrings.Length);

                    foreach (var m in candidateMethods)
                    {
                        try
                        {
                            if (m.IsGenericMethodDefinition && genericTypeName != null)
                            {
                                var genericType = ResolveType(genericTypeName);
                                method = m.MakeGenericMethod(genericType);
                            }
                            else if (!m.IsGenericMethod && genericTypeName == null)
                            {
                                method = m;
                            }
                            if (method != null) break;
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    if (method == null)
                        throw new Exception($"Method '{methodName}' tidak ditemukan atau tipe generic tidak cocok.");

                    MethodCache[methodKey] = method;
                }
            }

            ParameterInfo[] paramInfos = method.GetParameters();
            object[] parsedParams = new object[paramStrings.Length];

            for (int i = 0; i < paramStrings.Length; i++)
            {
                var paramType = paramInfos[i].ParameterType;
                var elementType = paramType.IsByRef ? paramType.GetElementType() : paramType;

                if (paramInfos[i].IsOut)
                {
                    parsedParams[i] = elementType.IsValueType ? Activator.CreateInstance(elementType) : null;
                }
                else if (jsonParamIndexes != null && jsonParamIndexes.Contains(i))
                {
                    try
                    {
                        parsedParams[i] = JsonDocument.Parse(paramStrings[i]).RootElement.Clone();
                    }
                    catch (JsonException ex)
                    {
                        throw new Exception($"Parameter ke-{i + 1} bukan JSON valid: {ex.Message}");
                    }
                }
                else if (elementType.IsEnum)
                {
                    parsedParams[i] = Enum.Parse(elementType, paramStrings[i].Trim('"'), ignoreCase: true);
                }
                else
                {
                    parsedParams[i] = Convert.ChangeType(paramStrings[i].Trim('"'), elementType);
                }
            }

            object instance = method.IsStatic ? null : Activator.CreateInstance(type);
            try
            {
                return method.Invoke(instance, parsedParams);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public static IEnumerable<string> SplitParameters(string input)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            bool inQuote = false;
            int backslashCount = 0;
            int parenLevel = 0;
            int bracketLevel = 0;
            int braceLevel = 0;
            bool isJsonStart = input.TrimStart().StartsWith("\"{\"");
            if (isJsonStart)
            {
                int jsonEndIndex = input.IndexOf("}\""); // cari akhir json yang dibungkus kutip

                if (jsonEndIndex != -1)
                {
                    // Ambil substring JSON + "}" = sampai posisi + 2 (karena }")
                    string jsonParam = input.Substring(0, jsonEndIndex + 2).Trim();
                    result.Add(jsonParam);

                    // Cek apakah masih ada parameter lain setelahnya
                    int nextComma = input.IndexOf(',', jsonEndIndex + 1);
                    if (nextComma != -1)
                    {
                        string remaining = input.Substring(nextComma + 1).Trim().Trim(')', '"', '(', ' ');
                        result.Add(remaining);
                    }

                    return result;
                }
            }
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c == '\\')
                {
                    backslashCount++;
                    current.Append(c);
                    continue;
                }

                if (c == '"' && backslashCount % 2 == 0)
                {
                    inQuote = !inQuote;
                }

                backslashCount = 0;

                if (!inQuote)
                {
                    if (c == '(') parenLevel++;
                    else if (c == ')') parenLevel--;
                    else if (c == '[') bracketLevel++;
                    else if (c == ']') bracketLevel--;
                    else if (c == '{') braceLevel++;
                    else if (c == '}') braceLevel--;

                    if (c == ',' && parenLevel == 0 && bracketLevel == 0 && braceLevel == 0)
                    {
                        result.Add(current.ToString().Trim());
                        current.Clear();
                        continue;
                    }
                }

                current.Append(c);
            }

            if (current.Length > 0)
            {
                result.Add(current.ToString().Trim());
            }

            return result;
        }

        private static Type ResolveType(string typeName)
        {
            var typeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "string", typeof(string) },
                { "int", typeof(int) },
                { "long", typeof(long) },
                { "bool", typeof(bool) },
                { "double", typeof(double) },
                { "float", typeof(float) },
                { "decimal", typeof(decimal) },
                { "datetime", typeof(DateTime) },
                { "object", typeof(object) }
            };

            if (typeMap.TryGetValue(typeName, out var mappedType))
                return mappedType;

            return Type.GetType(typeName, false, true)
                ?? AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => SafeGetTypes(a))
                    .FirstOrDefault(t =>
                        (string.Equals(t.FullName, typeName, StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase)) &&
                        t.FullName.StartsWith("Temiang.Avicenna", StringComparison.OrdinalIgnoreCase))
                ?? throw new Exception($"Tipe '{typeName}' tidak ditemukan atau tidak diizinkan.");
        }

        private static Type[] SafeGetTypes(Assembly assembly)
        {
            try { return assembly.GetTypes(); }
            catch { return new Type[0]; }
        }
    }
}