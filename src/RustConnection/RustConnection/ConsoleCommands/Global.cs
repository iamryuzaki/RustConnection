using System;
using System.Reflection;
using RustConnection.Manager;

namespace RustConnection.ConsoleCommands
{
    class Global
    {
        [ConsoleManager.ConsoleCommandAttribute("exit", "exit", "Close this application")]
        static void ExitCommand(string command, string[] args)
        {
            AppManager.Instance.IsWork = false;
        }

        [ConsoleManager.ConsoleCommandAttribute("help", "help", "Show all application commands")]
        static void HelpCommand(string command, string[] args)
        {
            Console.WriteLine("### List commands:");
            
            Type[] types = typeof(ConsoleManager).Assembly.GetTypes();
            for (var i = 0; i < types.Length && AppManager.Instance.IsWork; i++)
            {
                MethodInfo[] methods = types[i].GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Static);
                for (var j = 0; j < methods.Length && AppManager.Instance.IsWork; j++)
                {
                    object[] attrs = methods[j].GetCustomAttributes(typeof(ConsoleManager.ConsoleCommandAttribute), false);
                    if (attrs.Length > 0)
                    {
                        ConsoleManager.ConsoleCommandAttribute attr = (ConsoleManager.ConsoleCommandAttribute) attrs[0];
                        Console.WriteLine("# Command: " + attr.Command);
                        if (string.IsNullOrEmpty(attr.Syntax) == false)
                        {
                            Console.WriteLine("- Sytax: " + attr.Syntax);
                        }
                        if (string.IsNullOrEmpty(attr.Description) == false)
                        {
                            Console.WriteLine("- Description: " + attr.Description);
                        }
                    }
                }
            }
        }
    }
}