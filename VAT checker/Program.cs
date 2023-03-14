using Microsoft.AspNetCore.Mvc;
using VAT_checker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IVatCheckerService,VatCheckerService>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/vat/{id}", async ([FromServices]IVatCheckerService vatService, HttpContext context, string id) =>
{
    var info = await vatService.FetchVatAsync(id);
    if (info == null)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }
    await context.Response.WriteAsJsonAsync(info);
});

app.Run();
