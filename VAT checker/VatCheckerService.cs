using HtmlAgilityPack;

namespace VAT_checker
{
    public interface IVatCheckerService
    {
        Task<Vat> FetchVatAsync(string vatId, CancellationToken cancellationToken = default);
    }
    public class VatCheckerService : IVatCheckerService
    {
        private readonly HttpClient _httpClient;

        public VatCheckerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Vat> FetchVatAsync(string vatId, CancellationToken cancellationToken = default)
        {
            string url = "https://www.iban.com/vat-checker";
            var parameters = new Dictionary<string, string>
            {
                { "vat_id", vatId }
            };

            var content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync();
            Vat myvat = ParseTable(responseContent);
            if (myvat.CountryName != string.Empty) myvat.Id = vatId;
            else return null;

            return myvat; 
        }

        private Vat ParseTable(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectNodes(".//table")
                .Where(x => x.HasClass("downloads")).FirstOrDefault();
            var rows = table.SelectNodes(".//tr");
            rows.RemoveAt(0);

            Vat myvat = new Vat();

            foreach (var row in rows)
            {
                var cell = row.SelectNodes(".//td");
                var name = cell[0].SelectSingleNode(".//strong");
                var value = cell[1];
                if (name.InnerText == "Company Name") myvat.CompanyName = value.InnerText;
                if (name.InnerText == "Company Address") myvat.CompanyAddress = value.InnerText;
                if (name.InnerText == "Country Name") myvat.CountryName = value.InnerText;
                if (name.InnerText == "Country Code") myvat.CountryCode = value.InnerText;
            }
            return myvat;
        }
    }
}
