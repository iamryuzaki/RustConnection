using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Facepunch.Extend;
using Newtonsoft.Json;
using RustConnection.Base;
using RustConnection.Manager;
using RustConnection.Struct;
using global::Steamworks;
using Steamworks.Data;
using Timer = RustConnection.Help.Timer;

namespace RustConnection
{
    internal class Bootstrap : IWorker
    {
        public static Bootstrap Instance { get; set; }

        public static void Main(string[] args) => AppManager.Init<Bootstrap>();
        public static List<MirrorServerType> ListMirrors;
        public static MirrorServerType CurrentMirror;
        public static string CurrentAddr;
        
        public void Awake()
        {
            Instance = this;
            
            SteamClient.Init(252490);
            UpdateTaskList();
            
            AppManager.AddWorker<NetworkManager>();
            AppManager.AddWorker<ConsoleManager>();
            ConsoleManager.RegisterConsoleHandler(OnConsoleCommand);
            
            // Строчка для ленивых, что бы каждый раз не писать конект в консоли
            Timer.Once(() => NextStep(), 2f);
           
        }
        
        public void NextStep()
        {
            try
            {
                if (ListMirrors.Count > 0)
                {
                    CurrentMirror = ListMirrors.First();
                    ListMirrors.Remove(CurrentMirror);

                    try
                    {
                        CurrentAddr = CurrentMirror.server.addr;
                    }
                    catch (Exception ex)
                    {
                        CurrentAddr = "";
                    }

                    if (CurrentMirror.joinInfo != null && CurrentMirror.joinInfo.name != null)
                    {
                        Console.WriteLine("Skip Steap: " + CurrentMirror.server.addr);
                        Timer.Once(() => this.NextStep(), 0.01f);
                    }
                    else
                    {
                        Console.WriteLine("Steap: " + CurrentMirror.server.addr);
                        Timer.Once(() => OnConsoleCommand("connect " + CurrentMirror.server.addr.Split(':')[0] + ":" + CurrentMirror.server.gameport), 1f);
                    }
                }
                else
                {
                    Console.WriteLine("ListMirrors is end");
                    try
                    {
                        UpdateTaskList();
                        Timer.Once(() => this.NextStep(), 1f);
                    }
                    catch (Exception ex)
                    {
                        Timer.Once(() => this.NextStep(), 5f);
                    }
                }
            }
            catch
            {

            }
        }

        public static string TakeJoinedServer()
        {
            try
            {
                string content = new WebClient().DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=0F802634524A26C38F593A1C25FE547E&steamids=76561198961339655");
                SteamUserSummariesType userSummarie = JsonConvert.DeserializeObject<SteamUserSummariesType>(content);

                string addr = userSummarie.response.players[0].gameserverip;

                return addr;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }

        public static void UpdateTaskList()
        {
            string content = new WebClient().DownloadString("http://mirror-finder.alkad.org/api/rust/mirror");
            ListMirrors = JsonConvert.DeserializeObject<List<MirrorServerType>>(content);
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