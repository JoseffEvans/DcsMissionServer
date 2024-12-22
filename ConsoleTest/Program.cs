//using DcsPredictions;
//using DcsTcp;
//using Newtonsoft.Json;
//using SharedUtil;
//using System.Diagnostics;
//using System.Text.Json.Nodes;

//var testOutDir = "./TestOutput";

//using var connection = new DcsTcpConnection(
//    "127.0.0.1", 12622,
//    "127.0.0.1", 12522
//);

//int version = 0;
//string raw = string.Empty;

//try {
//    connection.StartServer((message) => {
//        if(!Directory.Exists(testOutDir))
//            Directory.CreateDirectory(testOutDir);
//        File.WriteAllText($"{testOutDir}/out{DateTime.Now:yyyyMMdd_HHmmss}.json", message);
//        raw = message;
//        version++;
//        return "Recived";
//    });
//} catch(Exception ex) { 
//    Console.WriteLine($"An Exception Occurred: \n {ex.CombinedMessage()}");
//}

//int currentVersion = 0;
//while(true) {
//    await Task.Run(() => Thread.Sleep(10));
//    if(currentVersion < version) {
//        currentVersion++;

//        DcsData? data;
//        lock(raw) {
//            try {
//                data = JsonConvert.DeserializeObject<DcsData>(raw);
//            } catch(Exception ex) {
//                Console.WriteLine(
//                    $"An error occurred deserializing data from dcs. Ex: {ex.CombinedMessage()}. Raw: {raw}"
//                );
//                continue;
//            }
//            if(data is null) {
//                Console.WriteLine($"Data from DCS deserialised to null.");
//                continue;
//            }
//        }

//        try {
//            var predictions = await Prediction.Predict(data);
//            Debug.Assert(predictions is not null);
//            await connection.SendMessage(JsonConvert.SerializeObject(predictions) ?? "{\"error\": true}");
//        }catch(Exception ex) {
//            Console.WriteLine($"An error occurred doing predictions: {ex.CombinedMessage()}");
//        }
//    }
//}