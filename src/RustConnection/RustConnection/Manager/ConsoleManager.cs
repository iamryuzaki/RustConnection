using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Facepunch;
using RustConnection.Base;

namespace RustConnection.Manager
{
    public class ConsoleManager : IWorker
    {
        private static List<Action<string>> ListConsoleHandlers { get; set; } = new List<Action<string>>();
        private static Dictionary<string, MethodInfo> ListCommandMethods { get; } = new Dictionary<string, MethodInfo>();

        public static void RegisterConsoleHandler(Action<string> handler)
        {
            ListConsoleHandlers.Add(handler);
        }

        public static bool RunConsoleCommand(string command, string[] args = null)
        {
            if (args == null)
            {
                args = new string[0];
            }

            if (ListCommandMethods.TryGetValue(command.ToLower(), out MethodInfo method))
            {
                try
                {
                    method.Invoke(null, new object[] {command, args});
                    return true;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Exception in {method.DeclaringType}.{method}(): ");
                    Console.WriteLine(ex);
                    Console.ResetColor();
                }
            }

            return false;
        }

        private Queue<string> QueueIncomingCommands { get; } = new Queue<string>();

        public void Awake()
        {
            Type[] types = typeof(ConsoleManager).Assembly.GetTypes();
            for (var i = 0; i < types.Length && AppManager.Instance.IsWork; i++)
            {
                MethodInfo[] methods = types[i].GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Static);
                for (var j = 0; j < methods.Length && AppManager.Instance.IsWork; j++)
                {
                    object[] attrs = methods[j].GetCustomAttributes(typeof(ConsoleCommandAttribute), false);
                    if (attrs.Length > 0)
                    {
                        ConsoleCommandAttribute attr = (ConsoleCommandAttribute) attrs[0];
                        ListCommandMethods[attr.Command.ToLower()] = methods[j];
                    }
                }
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (AppManager.Instance.IsWork)
                {
                    string line = Console.ReadLine();
                    if (string.IsNullOrEmpty(line) == false)
                    {
                        lock (QueueIncomingCommands)
                        {
                            QueueIncomingCommands.Enqueue(line.Trim());
                        }
                    }
                }
            });
        }

        public void Update(float deltaTime)
        {
            List<string> listCommandsNow = null;
            lock (QueueIncomingCommands)
            {
                while (QueueIncomingCommands.Count != 0)
                {
                    if (listCommandsNow == null)
                    {
                        listCommandsNow = Pool.GetList<string>();
                    }

                    listCommandsNow.Add(QueueIncomingCommands.Dequeue());
                }
            }

            if (listCommandsNow != null)
            {
                for (var i = 0; i < listCommandsNow.Count && AppManager.Instance.IsWork; i++)
                {
                    string command = listCommandsNow[i];
                    AppManager.CallInMainThread(() =>
                    {
                        for (var j = 0; j < ListConsoleHandlers.Count && AppManager.Instance.IsWork; j++)
                        {
                            try
                            {
                                ListConsoleHandlers[j].Invoke(command);
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine($"Exception in {ListConsoleHandlers[j].Method.DeclaringType}.{ListConsoleHandlers[j].Method}(): ");
                                Console.WriteLine(ex);
                                Console.ResetColor();
                            }
                        }
                    });
                }

                Pool.FreeList(ref listCommandsNow);
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class ConsoleCommandAttribute : Attribute
        {
            public string Command { get; private set; } = null;
            public string Syntax { get; private set; } = null;
            public string Description { get; private set; } = null;

            public ConsoleCommandAttribute(string command, string syntax = null, string description = null)
            {
                this.Command = command;
                this.Syntax = syntax;
                this.Description = description;
            }
        }

    }
}