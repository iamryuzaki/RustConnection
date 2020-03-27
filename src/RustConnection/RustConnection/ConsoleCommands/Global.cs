using RustConnection.Manager;

namespace RustConnection.ConsoleCommands
{
    class Global
    {
        [ConsoleManager.ConsoleCommandAttribute("exit")]
        static void ExitCommand(string command, string[] args)
        {
            AppManager.Instance.IsWork = false;
        }
    }
}