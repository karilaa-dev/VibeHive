using VibeHive.AlbumInventory.Grpc.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
// Validate the same Events JWT used by the REST APIs and Windows Forms session.
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

app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<AlbumInventoryService>();
app.MapGet("/", () => "VibeHive Album Inventory. Connect with the gRPC client on HTTP/2.");

app.Run();
