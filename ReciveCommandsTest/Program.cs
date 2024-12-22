//using CommandHandling;
//using DcsTcp;
//using SharedUtil;
//using System;
//using System.ComponentModel.Design;
//using System.Diagnostics;

Console.WriteLine("Starting Server");
//Logger.Log("Server Started");

//using var connection = new DcsTcpReciver(
//    "127.0.0.1", 12622

//);

//var sender = new DcsTcpSender("127.0.0.1", 12522);

//var messageHandler = new CommandHandler(sender, null) {
//    OnWriteMessage = message => Console.WriteLine($"Message from DCS: {message}"),
//    OnMessageHandled = command => Console.WriteLine($"Command from DCS handled: {command}")
//};

//var logicThread = new Thread(async () => {
//    var messageQueue = new Queue<string>();

//    try {
//        connection.OnMessageReceived = (message) => {
//            lock(messageQueue) messageQueue.Enqueue(message);
//            return "Success";
//        };
//        connection.StartServer();

//        Console.WriteLine("Server Started");
//    } catch(Exception ex) {
//        Console.WriteLine($"An Exception Occurred: \n {ex.CombinedMessage()}");
//    }

//    while(true) {
//        if(messageQueue.Count > 0) {
//            string message;
//            lock(messageQueue) message = messageQueue.Dequeue();
//            var commandResult = await messageHandler.HandleMessage(message);

//            if(!commandResult.Success) Console.WriteLine(
//                $"Error occurred while handling a message from DCS: {commandResult.Message}"
//            );
//        }
//    }
//}) {
//    IsBackground = true
//};

//logicThread.Start();

//while(true) {
//    var userInput = Console.ReadLine();
//    switch(userInput) {
//        case "EXIT" or "exit" or "Exit":
//        Logger.Log("Server Shutting Down \n\n");
//        return;

//        default:
//        Console.WriteLine($"Unrecognised command: {userInput}");
//        break;
//    }
//}