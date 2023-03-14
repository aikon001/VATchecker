using Microsoft.AspNetCore.Mvc;
using VAT_checker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IVatCheckerService,VatCheckerService>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/vat/{id}", async (IVatCheckerService vatService, string id) =>
{
    var info = await vatService.FetchVatAsync(id);
    if (info == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(info);
});

app.Run();
