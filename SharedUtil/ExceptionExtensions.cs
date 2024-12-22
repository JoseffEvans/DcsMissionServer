namespace SharedUtil {
    public static class ExceptionExtensions {
        public static string CombinedMessage(this Exception exception, string join = "\n") {
            var ex = exception;

            var messages = new List<string>();
            while(ex is not null) {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }

            return string.Join(join, messages);
        }


        public static string CombinedStackTrace(this Exception exception, string join = "\n") {
            var ex = exception;

            var messages = new List<string>();
            while(ex is not null) {
                messages.Add(ex.StackTrace ?? string.Empty);
                ex = ex.InnerException;
            }

            return string.Join(join, messages);
        }
    }
}
