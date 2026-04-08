namespace Temiang.Avicenna.BusinessObject
{
    public partial class ItemImmunization
    {
        public string ImmunizationName
        {
            get { return GetColumn("refToImmunization_ImmunizationName").ToString(); }
            set { SetColumn("refToImmunization_ImmunizationName", value); }
        }
    }
}
