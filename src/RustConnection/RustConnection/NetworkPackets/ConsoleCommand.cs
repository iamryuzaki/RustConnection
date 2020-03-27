using Network;
using RustConnection.Base;
using RustConnection.Manager;

namespace RustConnection.NetworkPackets
{
    public class ConsoleCommand : BaseNetworkPacket
    {
        public string Command { get; set; } = "";

        public override void SendTo()
        {
            this.Type = Message.Type.ConsoleCommand;
            
            if (NetworkManager.Instance.BaseClient.write.Start())
            {
                NetworkManager.Instance.BaseClient.write.PacketID(Message.Type.ConsoleCommand);
                NetworkManager.Instance.BaseClient.write.String(this.Command);
                NetworkManager.Instance.BaseClient.write.Send(new SendInfo());
            }
        }
    }
}