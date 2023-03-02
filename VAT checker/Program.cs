using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using VAT_checker;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<VatDb>(opt => opt.UseInMemoryDatabase("VatList"));
var app = builder.Build();

Vat ParseTable (string html)
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

app.MapGet("/vatitems/{id}", async (string id, VatDb db) =>
    await db.Vats.FindAsync(id)
        is Vat vat
            ? Results.Ok(vat)
            : Results.NotFound());

app.MapPost("/vatitems", async (RequestVat request, VatDb db) =>
{
    string url = "https://www.iban.com/vat-checker";
    var parameters = new Dictionary<string, string>
            {
                { "vat_id", request.Vat_id }
            };
    using (HttpClient client = new HttpClient())
    {
        var content = new FormUrlEncodedContent(parameters);
        HttpResponseMessage response = await client.PostAsync(url, content);

        var responseContent = await response.Content.ReadAsStringAsync();
        Vat myvat = ParseTable(responseContent);
        myvat.Id = request.Vat_id;

        db.Vats.Add(myvat);
        await db.SaveChangesAsync();

        return Results.Created($"/vatitems/{myvat.Id}",myvat);
    }

});

app.Run();
