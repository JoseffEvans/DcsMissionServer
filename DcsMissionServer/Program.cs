using CommandHandling;
using DcsTcp;
using UnitManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSqlite<UnitDbContext>("Data Source=unitdb.db");
builder.Services.AddSingleton<Manager>();
builder.Services.AddSingleton<DcsTcpSender>((services) => new DcsTcpSender("127.0.0.1", 12522));
builder.Services.AddSingleton<CommandHandler>(services => new CommandHandler(services) {
    OnMessageHandled = command => Console.WriteLine($"Command from DCS handled: ${command}"),
    OnWriteMessage = message => Console.WriteLine($"Message from DCS: {message}")
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
