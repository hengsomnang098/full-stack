using FullStack.Api.Endpoints;
using FullStack.Api.Extensions;
using FullStack.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

// The React client sometimes sends numbers as strings (e.g. "5"); accept both when reading request bodies.
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString);

builder.Services
    .RegisterLayers(builder.Configuration)
    .RegisterCors(builder.Configuration)
    .RegisterSwagger();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FullStack API v1"));

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors("Default");
app.UseAuthentication();
app.UseAuthorization();

// Initialise DB (EnsureCreated/Migrate/None via Database:InitMode) and seed admin.
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    try
    {
        await DbSeeder.InitializeAsync(scope.ServiceProvider, app.Configuration, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialisation/seed failed. API will continue to start.");
    }
}

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapAuthEndpoints();
app.MapCategoryEndpoints();
app.MapCustomerEndpoints();
app.MapRoleEndpoints();
app.MapProductEndpoints();
app.MapSupplierEndpoints();
app.MapOrderStatusEndpoints();
app.MapOrderPaymentMethodEndpoints();
app.MapEmployeeEndpoints();
app.MapInvoiceEndpoints();
app.MapPosEndpoints();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
