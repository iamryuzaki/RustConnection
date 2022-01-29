namespace RustConnection.Struct
{
    public class ResponseServersSteamAPIType
    {
        public string addr { get; set; }
        public int gameport { get; set; }
        public string steamid { get; set; }
        public string name { get; set; }
        public int appid { get; set; }
        public string gamedir { get; set; }
        public string version { get; set; }
        public string product { get; set; }
        public int region { get; set; }
        public int players { get; set; }
        public int max_players { get; set; }
        public int bots { get; set; }
        public string map { get; set; }
        public bool secure { get; set; }
        public bool dedicated { get; set; }
        public string os { get; set; }
        public string gametype { get; set; }
    }
}