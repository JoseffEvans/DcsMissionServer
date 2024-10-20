using DcsTcp;
using SharedUtil;

var testOutDir = "./TestOutput";

var connection = new DcsTcpConnection(
    "127.0.0.1", 12534,
    "127.0.0.1", 12545
);

try {
    connection.StartServer((message) => {
        if(!Directory.Exists(testOutDir))
            Directory.CreateDirectory(testOutDir);
        File.WriteAllText($"{testOutDir}/{DateTime.Now}", message);
        Console.WriteLine(message);
        return "Recived";
    });
} catch(Exception ex) {
    Console.WriteLine($"An Exception Occurred: \n {ex.CombinedMessage()}");
}


Thread.Sleep(1000 * 60);

connection.StopServer();

Console.ReadLine();


