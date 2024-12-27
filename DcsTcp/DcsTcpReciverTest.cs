using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;

class Unit{
    public int UnitId { get; set; }
    public int CoalitionId { get; set; }
    public string? UnitName { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    [JsonIgnore]
    public Action<Unit>? OnUpdate {get; set;}
    public void Update(){
        if(OnUpdate is not null)
            OnUpdate(this);
    }
}

class UpdateUnitRequest{
    public required List<Unit> Units {get; set;}
}

namespace DcsTcp{
    public class DcsTcpReciverTest : IHostedService{

        bool _stop;
        Thread? _thread;
        CancellationToken? _token;
        List<Unit>? _units;
        public Action<string>? OnMessageRecived {get; set;}

        public async Task StartAsync(CancellationToken cancellationToken){
            _token = cancellationToken;
            _units = CreateUnits();
            _thread = new Thread(RunLoop){
                IsBackground = true
            };
            _thread.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken){
            _stop = true; 
            _thread?.Join();
        }

        public void RunLoop(){
            while(
                _token is not null 
                && !_stop
            ){
                foreach(var unit in _units) unit.Update();

                var s = "UpdateUnits\n" + 
                    JsonSerializer.Serialize(
                        new UpdateUnitRequest{
                            Units = _units ??
                                throw new Exception("Units cannot be null")
                    });

                Console.WriteLine($"sending message {s}");

                if(OnMessageRecived is not null) 
                    OnMessageRecived(s);

                Thread.Sleep(100);
            }
        }

        List<Unit> CreateUnits() => [
            new Unit{
                UnitId = 1,
                UnitName = "Player 0",
                PosX = -52342.53242f,
                PosY = 41231.12412f,
                CoalitionId = 1,
                OnUpdate = unit => unit.PosY += 500f
            },
            new Unit{
                UnitId = 2,
                UnitName = "Red 1-1",
                PosX = -52242.53242f,
                PosY = 41431.12412f,
                CoalitionId = 1
            }
        ];

    }
}