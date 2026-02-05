using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Temiang.Avicenna.Util
{
    public class JsonHelper
    {
        public static JsonElement? GetElementByPath(JsonElement root, string path)
        {
            string[] parts = path.Split('.');
            JsonElement current = root;

            foreach (var part in parts)
            {
                int bracketStart = part.IndexOf('[');
                if (bracketStart >= 0)
                {
                    string propName = part.Substring(0, bracketStart);
                    int bracketEnd = part.IndexOf(']');
                    if (bracketEnd < 0)
                        return null;

                    string indexStr = part.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
                    int index;
                    if (!int.TryParse(indexStr, out index))
                        return null;

                    JsonElement arrElement;
                    if (!current.TryGetProperty(propName, out arrElement))
                        return null;

                    if (arrElement.ValueKind != JsonValueKind.Array || arrElement.GetArrayLength() <= index)
                        return null;

                    current = arrElement[index];
                }
                else
                {
                    JsonElement prop;
                    if (!current.TryGetProperty(part, out prop))
                        return null;

                    current = prop;
                }
            }

            return current;
        }

        /// <summary>
        /// Untuk mengambil value json berdasarkan path
        /// </summary>
        /// <typeparam name="T">Tipe data value json</typeparam>
        /// <param name="root">json string</param>
        /// <param name="path">path yang mau diambil valuenya, bisa pakai tanda titik (.) untuk value bertingkat, bisa pakai notasi array untuk element yang berupa array, contoh "resource.code" atau "resource[0].code"</param>
        /// <returns></returns>
        private static T GetValueByPath<T>(JsonElement root, string path)
        {
            var element = GetElementByPath(root, path);
            if (element == null)
                return default(T);

            try
            {
                object result = null;
                var value = element.Value;

                if (typeof(T) == typeof(string) && value.ValueKind == JsonValueKind.String)
                    result = value.GetString();
                else if (typeof(T) == typeof(int) && value.ValueKind == JsonValueKind.Number)
                    result = value.GetInt32();
                else if (typeof(T) == typeof(long) && value.ValueKind == JsonValueKind.Number)
                    result = value.GetInt64();
                else if (typeof(T) == typeof(bool) && (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False))
                    result = value.GetBoolean();
                else if (typeof(T) == typeof(double) && value.ValueKind == JsonValueKind.Number)
                    result = value.GetDouble();
                else
                    result = Convert.ChangeType(value.ToString(), typeof(T));

                return (T)result;
            }
            catch
            {
                return default(T);
            }
        }

        public static T GetValueByPath<T>(string json, string path)
        {
            try
            {
                using (var doc = JsonDocument.Parse(json))
                {
                    return GetValueByPath<T>(doc.RootElement, path);
                }
            }
            catch
            {
                return default(T);
            }
        }
        public static List<T> GetValuesByPath<T>(string json, string path)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;
            var result = new List<T>();

            try
            {
                using (var doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    string[] parts = path.Split('.');
                    if (parts.Length == 0) return null;

                    string arrayKey = parts[0];
                    string subPath = string.Join(".", parts.Skip(1));

                    if (!root.TryGetProperty(arrayKey, out var arrayElement) || arrayElement.ValueKind != JsonValueKind.Array)
                        return null;

                    foreach (var item in arrayElement.EnumerateArray())
                    {
                        var subElement = GetElementByPath(item, subPath);
                        if (subElement != null)
                        {
                            var value = subElement.Value;
                            try
                            {
                                object converted = null;
                                if (typeof(T) == typeof(string) && value.ValueKind == JsonValueKind.String)
                                    converted = value.GetString();
                                else if (typeof(T) == typeof(int) && value.ValueKind == JsonValueKind.Number)
                                    converted = value.GetInt32();
                                else if (typeof(T) == typeof(long) && value.ValueKind == JsonValueKind.Number)
                                    converted = value.GetInt64();
                                else if (typeof(T) == typeof(bool) && (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False))
                                    converted = value.GetBoolean();
                                else if (typeof(T) == typeof(double) && value.ValueKind == JsonValueKind.Number)
                                    converted = value.GetDouble();
                                else
                                    converted = Convert.ChangeType(value.ToString(), typeof(T));

                                result.Add((T)converted);
                            }
                            catch
                            {
                                // Skip on conversion error
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return result;
        }
    }
}