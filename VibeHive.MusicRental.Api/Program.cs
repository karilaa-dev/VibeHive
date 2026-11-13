using VibeHive.MusicRental.Api.Services;
using Grpc.Core;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Rental records stay in this API. Album state belongs to Album Inventory.
builder.Services.AddSingleton<MusicService>();
builder.Services.AddSingleton<RentalManager>();
builder.Services.AddHttpClient("Events", client =>
{
    client.BaseAddress = new Uri((Environment.GetEnvironmentVariable("VIBEHIVE_EVENTS_URL")
        ?? builder.Configuration["Events:Url"] ?? "http://localhost:5273").TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(3);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Validate the same tokens issued by Events; no separate accounts or login service.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "VibeHive.Events.Api",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "VibeHive.Clients",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["Jwt:Key"] ?? "VibeHiveSecretKeyForAuthentication2026!"))
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Translate inventory errors once so both REST controllers return the same status codes.
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (RpcException error)
    {
        context.Response.StatusCode = error.StatusCode switch
        {
            StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
            StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
            StatusCode.NotFound => StatusCodes.Status404NotFound,
            StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
            StatusCode.FailedPrecondition or StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status503ServiceUnavailable
        };
        string message = error.Status.Detail;
        if (context.Response.StatusCode == StatusCodes.Status503ServiceUnavailable)
        {
            message = "Album Inventory is unavailable. Start it, then refresh. "
                + "If a Pending rental appears in Active Rentals, return it before trying again.";
        }
        await context.Response.WriteAsJsonAsync(message);
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok());
app.MapControllers();

app.Run();
