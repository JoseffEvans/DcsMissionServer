using CommandHandling;
using DcsMissionServer;
using DcsTcp;
using Microsoft.AspNetCore.SignalR;
using UnitManager;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSqlite<UnitDbContext>("Data Source=unitdb.db");
builder.Services.AddSingleton<Manager>();
builder.Services.AddSingleton<DcsTcpSender>((services) => new DcsTcpSender("127.0.0.1", 12522));
builder.Services.AddSingleton<CommandHandler>(services => {
    var hub = services.GetRequiredService<IHubContext<UnitsHub>>();

    return new CommandHandler(services) {
        OnMessageHandled = command => Console.WriteLine($"Command from DCS handled: ${command}"),
        OnWriteMessage = message => Console.WriteLine($"Message from DCS: {message}"),
        OnUnitsUpdated = async units => await hub.Clients.All.SendAsync(
            "UpdateUnits", JsonSerializer.Serialize(units)
        )
    };
});


builder.Services.AddHostedService<DcsTcpReciver>(
    services => {
        var commandHandler = services.GetRequiredService<CommandHandler>();

        return new DcsTcpReciver(
            "127.0.0.1", 12622
        ) {
            OnMessageReceived = message => {
                var commandResult = commandHandler.HandleMessage(message).Result;

                if(!commandResult.Success)
                    Console.WriteLine($"Error handing dcs command: {commandResult.Message}");

                return string.Empty;
            }
        };
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapHub<UnitsHub>("/unitsHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
