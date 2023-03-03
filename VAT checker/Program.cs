using VAT_checker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IVatCheckerService,VatCheckerService>();
builder.Services.AddScoped(provider =>
{
    var client = new HttpClient();
    return client;
});

var app = builder.Build();

app.MapGet("/vat/{id}", async (HttpContext context, string id) =>
{
    var httpClient = context.RequestServices.GetRequiredService<HttpClient>();
    var vatService = new VatCheckerService(httpClient);

    var info = await vatService.FetchVatAsync(id);
    if (info == null)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }
    await context.Response.WriteAsJsonAsync(info);
});

app.Run();
