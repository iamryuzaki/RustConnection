using System;
using Facepunch.Extend;
using RustConnection.Base;
using RustConnection.Manager;
using Timer = RustConnection.Help.Timer;

namespace RustConnection
{
    internal class Bootstrap : IWorker
    {
        public static void Main(string[] args) => AppManager.Init<Bootstrap>();

        public void Awake()
        {
            AppManager.AddWorker<NetworkManager>();
            AppManager.AddWorker<ConsoleManager>();
            ConsoleManager.RegisterConsoleHandler(OnConsoleCommand);

            // Строчка для ленивых, что бы каждый раз не писать конект в консоли
            Timer.Once(() => OnConsoleCommand("connect 212.22.93.83:28015"), 2f);
        }

        private void OnConsoleCommand(string line)
        {
            string command = line.IndexOf(' ') != -1 ? line.Substring(0, line.IndexOf(' ')) : line;
            string[] args = (command.Length != line.Length ? line.Substring(command.Length + 1) : "").SplitQuotesStrings();
            if (ConsoleManager.RunConsoleCommand(command, args) == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Command [{command}] not found!");
                Console.ResetColor();
            }
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}