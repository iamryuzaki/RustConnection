using System;
using RustConnection.Manager;

namespace RustConnection.ConsoleCommands
{
    public class Client
    {
        [ConsoleManager.ConsoleCommandAttribute("connect")]
        static void ConnectCommand(string command, string[] args)
        {
            if (NetworkManager.Instance.HaveConnection == true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[ConsoleCommand] <connect> You is already connection!");
                Console.ResetColor();
                return;
            }
            
            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[ConsoleCommand] <connect> is not correct syntax!");
                Console.ResetColor();
                return;
            } 
            string[] ex = args[0].Split(new char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
            if (ex.Length < 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[ConsoleCommand] <connect> is not correct addr!");
                Console.ResetColor();
                return;
            }

            int port = 0;
            if (int.TryParse(ex[1], out port) == false)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[ConsoleCommand] <connect> is not correct port!");
                Console.ResetColor();
                return;
            }
            
            NetworkManager.Instance.Connect(ex[0], port);
        }

        [ConsoleManager.ConsoleCommandAttribute("disconnect")]
        static void DisconnectCommand(string command, string[] args)
        {
            if (NetworkManager.Instance.HaveConnection == false)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[ConsoleCommand] <connect> You not have connection!");
                Console.ResetColor();
                return;
            }
            
            NetworkManager.Instance.BaseClient.Disconnect("Disconnected!", true);
        }
    }
}