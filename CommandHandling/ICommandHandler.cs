using CommandModels;

namespace CommandHandling{
    public interface ICommandHandler{
        public Task<HandleCommandResult> HandleMessage(string message);
    }
}