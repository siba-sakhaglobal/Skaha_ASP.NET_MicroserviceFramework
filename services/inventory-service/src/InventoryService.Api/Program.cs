using InventoryService.Application;
using InventoryService.Infrastructure;
using InventoryService.Api.Middleware;
using InventoryService.Api.RateLimiting;
using InventoryService.Api.Resilience;
using SakhaERP.Shared.Security;
using SakhaERP.Shared.Security.MultiTenancy;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ServiceName", "InventoryService")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}"));

// Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Multi-tenancy + security (useConfigResolver=true for dev, false for production)
builder.Services.AddSakhaSecurity(useConfigResolver: builder.Environment.IsDevelopment());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS for frontend (dev: localhost:3000)
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Inventory Service API", Version = "v1" }));

// Per-tenant rate limiting (architecture doc: prevent noisy-neighbor)
builder.Services.AddTenantRateLimiting(builder.Configuration);

// Resilient HTTP clients with Polly (architecture doc: retry + circuit breaker)
builder.Services.AddResilientHttpClients(builder.Configuration);

var app = builder.Build();

// CORS must be before other middleware
app.UseCors("Frontend");

// Middleware pipeline (ORDER MATTERS)
// 1. Correlation ID first (for all logging)
app.UseMiddleware<CorrelationIdMiddleware>();
// 2. Exception handling (catches all downstream errors)
app.UseMiddleware<ExceptionHandlingMiddleware>();
// 3. Tenant resolution (resolves tenant_id → connection string, shared/dedicated DB)
//    Sets TenantInfo in HttpContext.Items for DbContext factory
app.UseMiddleware<TenantMiddleware>();
// 4. Rate limiting (per-tenant, needs tenant resolved first)
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
