var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEcommerceApi(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json
}

if (app.Configuration.GetValue<bool>("Database:AutoInitialize"))
{
    await app.InitialiseDatabaseAsync();
}

app.MapControllers();

app.Run();
