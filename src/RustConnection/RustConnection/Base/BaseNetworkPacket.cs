namespace RustConnection.Base
{
    public class BaseNetworkPacket
    {
        public Network.Message.Type Type;

        public virtual void SendTo()
        {
            
        }
    }
}