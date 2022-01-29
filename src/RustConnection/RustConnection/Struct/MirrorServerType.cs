namespace RustConnection.Struct
{
    public class MirrorServerType
    {
        public MirrorServerDetectType detect { get; set; }
        public string country { get; set; }
        public bool hasBannedFacepunch { get; set; }
        public ResponseServersSteamAPIType server { get; set; }
        public RustQueryInfoResponseType serverInfo { get; set; }
        public RustQueryInfoResponseType joinInfo { get; set; }
    }
}