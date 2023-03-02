namespace VAT_checker
{
    public class RequestVat
    {
        public string Vat_id { get; set; }
    }
    public class Vat
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
}
