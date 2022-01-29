using System;
using System.Net;
using System.Threading;
using Network;
using RustConnection.Base;
using RustConnection.NetworkPackets;
using RustConnection.Rust;
using Steamworks;
using Client = Facepunch.Network.Raknet.Client;
using Timer = RustConnection.Help.Timer;

namespace RustConnection.Manager
{
    public class NetworkManager : IWorker, IClientCallback
    {
        public static NetworkManager Instance { get; private set; }
        public Facepunch.Network.Raknet.Client BaseClient { get; set; } = null;
        public bool HaveConnection { get; private set; } = false;

        public void Awake()
        {
            Instance = this;
            this.BaseClient = new Client();
            this.BaseClient.cryptography = new NetworkCryptographyClient();
            this.BaseClient.callbackHandler = this;
        }

        public void Update(float deltaTime)
        {
            this.BaseClient.Cycle();
        }

        public void Connect(string addr, int port)
        {
            this.HaveConnection = true;
            this.BaseClient.Connect(addr, port);
        }

        public void OnNetworkMessage(Message message)
        {
            switch (message.type)
            {
                case Message.Type.RequestUserInformation:
                    Console.WriteLine("[NetworkManager]: RequestUserInformation");
                    new GiveUserInformation
                    {
                        GameVersion = UInt32.Parse(Bootstrap.CurrentMirror.server.version),
                        SteamID = SteamClient.SteamId.Value,
                        Username = SteamClient.Name,
                        SteamToken = SteamUser.GetAuthSessionTicket().Data
                    }.SendTo();

                    Timer.Once(() =>
                    {
                        string joinAddr = "";
                        int maybe = 0;
                        maybeRenew:
                        try
                        {
                            joinAddr = Bootstrap.TakeJoinedServer();
                            if (joinAddr.Length == 0)
                            {
                                maybe++;
                                if (maybe <= 5)
                                {
                                    goto maybeRenew;
                                }
                            }
                        }
                        catch
                        {
                            maybe++;
                            if (maybe <= 5)
                            {
                                goto maybeRenew;
                            }
                        }

                        if (Bootstrap.CurrentAddr.Length > 0)
                        {

                            if (joinAddr.Length > 0)
                            {
                                Console.WriteLine($"Detected {Bootstrap.CurrentMirror.server.addr} join to " + joinAddr);
                                try
                                {
                                    new WebClient().DownloadString("http://mirror-finder.alkad.org/api/rust/mirror_join?addr=" + Bootstrap.CurrentMirror.server.addr + "&join=" + joinAddr);
                                }
                                catch
                                {

                                }

                                ThreadPool.QueueUserWorkItem(_ =>
                                {
                                    try
                                    {
                                        new WebClient().DownloadString("http://127.0.0.1/api/rust/mirror_join?addr=" + Bootstrap.CurrentMirror.server.addr + "&join=" + joinAddr);
                                    }
                                    catch
                                    {

                                    }
                                });
                            }
                        }
                        else
                        {
                            Console.WriteLine("Currrent addr = 0");
                        }

                        Timer.Once(() =>
                        {
                            this.BaseClient.Disconnect("next", false);
                            
                        }, 0.5f);
                    }, 1f);
                    break;
                case Message.Type.Approved:
                    Console.WriteLine("[NetworkManager]: Approved");
                    new ClientReady().SendTo();
                    message.connection.decryptIncoming = true;
                    message.connection.encryptOutgoing = true;
                    message.connection.encryptionLevel = 1;
                    break;
                case Message.Type.ConsoleCommand:
                    Console.WriteLine("[NetworkManager]: ConsoleCommand: " + message.read.String());
                    break;
                case Message.Type.ConsoleMessage:
                    Console.WriteLine("[NetworkManager]: ConsoleMessage: " + message.read.String());
                    break;
                case Message.Type.DisconnectReason:
                    Console.WriteLine("[NetworkManager]: DisconnectReason: " + message.read.String());
                    
                    break;
                case Message.Type.RPCMessage:
                    break;
                case Message.Type.Entities:
                    break;
                case Message.Type.EntityDestroy:
                    break;
                case Message.Type.ConsoleReplicatedVars:
                    Console.WriteLine("[NetworkManager]: ConsoleReplicatedVars");
                    new ConsoleCommand
                    {
                        Command = "chat.say \"PIZDEC\""
                    }.SendTo();
                    break;
                case Message.Type.GroupDestroy:
                    Console.WriteLine("[NetworkManager]: GroupDestroy" );
                    break;
            }
        }

        public void OnClientDisconnected(string reason)
        {
            this.HaveConnection = false;
            Bootstrap.Instance.NextStep();
            Console.WriteLine("[NetworkManager]: OnClientDisconnected => " + reason);
        }
    }
}