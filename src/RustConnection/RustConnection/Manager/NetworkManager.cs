using System;
using Network;
using RustConnection.Base;
using RustConnection.Help;
using RustConnection.NetworkPackets;
using RustConnection.Rust;
using Client = Facepunch.Network.Raknet.Client;

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
                        GameVersion = 2218,
                        SteamID = 76561198006885791,
                        Username = "Ghojo",
                    }.SendTo();
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
            Console.WriteLine("[NetworkManager]: OnClientDisconnected => " + reason);
        }
    }
}