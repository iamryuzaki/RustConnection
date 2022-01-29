using System;
using Network;
using Steamworks;

namespace RustConnection.Struct
{
    public class ClientHandler : IClientCallback
    {
        public void OnNetworkMessage(Message message)
        {
            Console.WriteLine(message.type);
            switch (message.type)
            {
                case Message.Type.RequestUserInformation:
                    message.write.Start();
                    message.write.PacketID(Message.Type.GiveUserInformation);
                    message.write.UInt8(228);
                    message.write.UInt64(SteamClient.SteamId.Value);
                    message.write.UInt32(2303);
                    message.write.String("windows");
                    message.write.String(SteamClient.Name);
                    message.write.BytesWithSize(SteamUser.GetAuthSessionTicket().Data);
                    message.write.Send(new SendInfo());
                    break;
                default:
                    break;
            }
        }

        public void OnClientDisconnected(string reason)
        {
            Console.WriteLine("OnClientDisconnected");
        }
    }
}