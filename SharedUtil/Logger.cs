namespace SharedUtil {
    public class Logger {
        const string LogFolder = @".\Logs";

        public static void Log(string message) {
            if(!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);
            File.AppendAllText(
                @$"{LogFolder}\log{DateTime.Today:yy-MM-dd}.txt",
                $"\nLog at {DateTime.Now}:\n{message}\n"
            );
        }

        public static void Log(Exception ex) {
            Log($"{ex.CombinedMessage()}\nStack trace: {ex.CombinedStackTrace()}");
        }
    }
}
