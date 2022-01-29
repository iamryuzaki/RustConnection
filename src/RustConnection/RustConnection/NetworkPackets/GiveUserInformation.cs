using System;
using Network;
using RustConnection.Base;
using RustConnection.Help;
using RustConnection.Manager;
using RustConnection.Rust;
using RustConnection.Steamwork;

namespace RustConnection.NetworkPackets
{
    public class GiveUserInformation : BaseNetworkPacket
    {
        public Byte Protocol { get; set; } = 228;
        public UInt64 SteamID { get; set; } = 0;
        public UInt32 GameVersion { get; set; } = 0;
        public String OsName { get; set; } = "windows";
        public String Username { get; set; } = "noname";
        public String Branch { get; set; } = "public";
        public Byte[] SteamToken { get; set; } = new byte[0];

        public override void SendTo()
        {
            if (this.SteamToken.Length == 0)
            {
                this.SteamToken = Token.getBytes(new Token
                {
                    Information = new TokenInformation
                    {
                        Length = 182,
                        Unknown0x38 = 0,
                        Unknown0x50 = 0,
                        Unknown0x51 = 0,
                        Unknown0x52 = 0,
                        Unknown0x53 = 0,
                        Unknown0x54 = 0,
                        Unknown0x60 = 0,
                        Unknown0x61 = 0,
                        Unknown0x62 = 0,
                        Unknown0x63 = 0,
                        Unknown0x64 = 0,
                        Unknown0x66 = 0,
                        Unknown0x68 = 0,
                        EndedTime = 0,
                        StartTime = 0,
                        Unknown0x3C = 0,
                        Unknown0x4C = 0,
                        AppID = 480,
                        SHA128 = new byte[128],
                        UserID = this.SteamID
                    },
                    Session = new Session
                    {
                        Length = 28,
                        Unknown0x20 = 0,
                        Unknown0x24 = 0,
                        Unknown0x28 = 0,
                        ConnectNumber = 0,
                        Unknown0x1C = 0,
                        SessionID = 0
                    },
                    Length = 234,
                    ConnectionTime = 0,
                    ID = 0,
                    SteamID = this.SteamID
                });
            }


            this.Type = Message.Type.GiveUserInformation;
            
            if (NetworkManager.Instance.BaseClient.write.Start())
            {
                NetworkManager.Instance.BaseClient.write.PacketID(Message.Type.GiveUserInformation);
                NetworkManager.Instance.BaseClient.write.UInt8(this.Protocol);
                NetworkManager.Instance.BaseClient.write.UInt64(this.SteamID);
                NetworkManager.Instance.BaseClient.write.UInt32(this.GameVersion);
                NetworkManager.Instance.BaseClient.write.String(this.OsName);
                NetworkManager.Instance.BaseClient.write.String(this.Username);
                NetworkManager.Instance.BaseClient.write.String(this.Branch);
                NetworkManager.Instance.BaseClient.write.BytesWithSize(this.SteamToken);
                NetworkManager.Instance.BaseClient.write.Send(new SendInfo());
            }
            
            NetworkCryptographyClient.EncryptionSeed = this.GameVersion;
        }
    }
}