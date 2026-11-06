using VibeHive.AlbumInventory.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<AlbumInventoryService>();
app.MapGet("/", () => "VibeHive Album Inventory. Connect with the gRPC client on HTTP/2.");

app.Run();
