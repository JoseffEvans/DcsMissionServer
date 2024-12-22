using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UnitManager;
using UnitManager.Models;

var builder = new ServiceCollection();
builder.AddSqlite<UnitDbContext>("Data Source=unitdb.db");

var services = builder.BuildServiceProvider();

var unitmanager = new Manager(services);

//Console.WriteLine("Clearing Db");
//await unitmanager.ClearDb();

Console.WriteLine("Adding two units");
await unitmanager.UpdateUnits([
    new Unit{
        UnitId = 1,
        CoalitionId = 1,
        UnitName = "Big steve"
    },
    new Unit{
        UnitId = 2,
        CoalitionId = 2,
        UnitName = "Big steve2"
    },
]);

Console.WriteLine("All coalition 1 units:");
Console.WriteLine(JsonSerializer.Serialize(unitmanager.GetUnits(1)));

Console.WriteLine("Adding / updating");
await unitmanager.UpdateUnits([
    new Unit{
        UnitId = 3,
        CoalitionId = 1,
        UnitName = "Big steve3"
    },    
    new Unit{
        UnitId = 1,
        CoalitionId = 1,
        UnitName = "Big steve4"
    },
]);

Console.WriteLine("All Units");
Console.WriteLine(JsonSerializer.Serialize(unitmanager.GetAllUnits()));
Console.ReadLine();