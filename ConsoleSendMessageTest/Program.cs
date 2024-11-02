using DcsTcp;

using var connection = new DcsTcpConnection(
    "127.0.0.1", 12622,
    "127.0.0.1", 12522
);

while(true) {
    Console.WriteLine("Send Message");
    var message = Console.ReadLine();

    if(string.IsNullOrEmpty(message)) {
        Console.WriteLine("Non null input expected"); break;
    }
    try {
        await connection.SendMessage(message);
        Console.WriteLine("Success\n");
    } catch(Exception ex) {
        Console.WriteLine(ex.ToString());
    }
}