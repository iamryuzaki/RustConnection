using System.Collections.Generic;
using Network;
using RustConnection.Base;
using RustConnection.Manager;

namespace RustConnection.NetworkPackets
{
    public class ClientReady : BaseNetworkPacket
    {
        public override void SendTo()
        {
            this.Type = Message.Type.Ready;
            
            if (NetworkManager.Instance.BaseClient.write.Start())
            {
                NetworkManager.Instance.BaseClient.write.PacketID(Message.Type.Ready);
                ProtoBuf.ClientReady clientReadyProto = new ProtoBuf.ClientReady
                {
                    clientInfo = new List<ProtoBuf.ClientReady.ClientInfo>()
                };
                clientReadyProto.WriteToStream(NetworkManager.Instance.BaseClient.write);
                NetworkManager.Instance.BaseClient.write.Send(new SendInfo());
            }
        }
    }
}