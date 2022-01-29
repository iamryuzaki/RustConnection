namespace RustConnection.Struct
{
    public class RustQueryInfoResponseType
    {
        public int protocol { get; set; }
        public string name { get; set; }
        public string map { get; set; }
        public string folder { get; set; }
        public string game { get; set; }
        public int appid { get; set; }
        public int playersnum { get; set; }
        public int maxplayers { get; set; }
        public int botsnum { get; set; }
        public string servertype { get; set; }
        public string environment { get; set; }
        public int visibility { get; set; }
        public int vac { get; set; }
        public string version { get; set; }
        public int port { get; set; }
        public int steamID { get; set; }
        public string keywords { get; set; }
        public int gameID { get; set; }
    }
}