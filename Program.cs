using Prometheus;
using PrometheusDemo.Extensions;
using PrometheusDemo.MetricsConfig;
using PrometheusDemo.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ProductsService>();
builder.Services.AddLogging(
    builder =>
    {
        builder.AddConsole();
        // Add other logging providers as needed
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseHttpMetrics();

app.Map("/metrics", metricsApp =>
{
    metricsApp.UseMiddleware<BasicAuthMiddleware>("");

    // We already specified URL prefix in .Map() above, no need to specify it again here.
    metricsApp.UseMetricServer("");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
