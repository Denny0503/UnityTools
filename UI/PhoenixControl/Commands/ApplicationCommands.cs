using Prism.Commands;

namespace PhoenixControl.Commands
{
    public interface IApplicationCommands
    {
        CompositeCommand SaveCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        public CompositeCommand SaveCommand { get; } = new CompositeCommand(true);
    }
}
