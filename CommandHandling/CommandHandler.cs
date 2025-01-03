﻿using CommandModels;
using DcsTcp;
using DcsPredictions;
using Newtonsoft.Json;
using UnitManager;
using UnitManager.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandHandling {
    public class CommandHandler : ICommandHandler{ 

        public CommandHandler(IServiceProvider services) {
            Connection = services.GetRequiredService<DcsTcpSender>();
            UnitManager = services.GetRequiredService<Manager>();
        }

        readonly DcsTcpSender Connection;
        readonly Manager UnitManager;
        public Action<string>? OnMessageHandled = null;
        public Action<string>? OnWriteMessage = null;
        public Action<List<Unit>>? OnUnitsUpdated = null;

        public async Task<HandleCommandResult> HandleMessage(string message) {
            var i = message.IndexOf('\n');
            var command = (i == -1) ? message : message[..i];
            var payload = (i == -1) ? string.Empty : message[(i + 1)..];

            try {
                switch(command) {
                    case Command.Predict:
                    await Predict(payload);
                    break;

                    case Command.Write:
                    Write(payload);
                    break;

                    case Command.OutFile:
                    await WriteToFile(payload);
                    break;

                    case Command.UpdateUnits:
                    await UpdateUnits(payload);
                    break;

                    default:
                    throw new Exception($"Command from DCS not recognised: {command}");
                }
            } catch(Exception ex) {
                return new HandleCommandResult {
                    Success = false,
                    Message = ex.Message
                };
            }

            if(OnMessageHandled is not null)
                OnMessageHandled(command);

            return new HandleCommandResult { Success = true };
        }

        public async Task Predict(string message) {
            var predictions = await Prediction.Predict(message);
            await Connection.SendMessage(JsonConvert.SerializeObject(predictions) ?? "{\"error\": true}");
        }

        public void Write(string message) {
            if(OnWriteMessage is not null) OnWriteMessage(message);
        }

        public static async Task WriteToFile(string message) {
            const string testOutDir = "./TestOutput";
            if(!Directory.Exists(testOutDir))
                Directory.CreateDirectory(testOutDir);
            await File.WriteAllTextAsync($"{testOutDir}/out{DateTime.Now:yyyyMMdd_HHmmss}.json", message);
        }

        public async Task UpdateUnits(string payload) {
            await UnitManager.UpdateUnits(JsonConvert.DeserializeObject<UpdateUnitsRequest>(payload)?.Units
                ?? throw new Exception("Update units request from dcs resulted in null"));
            if(OnUnitsUpdated is not null)
                OnUnitsUpdated(await UnitManager.GetAllUnits());
        }
    }
}
