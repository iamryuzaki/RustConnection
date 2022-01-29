namespace RustConnection.Struct
{
    public class SteamUserSummariesType
    {
        public SteamUserSummariesPlayersType response { get; set; }
    }

    public class SteamUserSummariesPlayersType
    {
        public SteamUserSummariesPlayerType[] players { get; set; }
    }

    public class SteamUserSummariesPlayerType
    {
        public string steamid { get; set; }
        public int communityvisibilitystat { get; set; }
        public int profilestate { get; set; }
        public string personaname { get; set; }
        public int commentpermission { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public string avatarhash { get; set; }
        public int personastate { get; set; }
        public string primaryclanid { get; set; }
        public int timecreated { get; set; }
        public int personastateflags { get; set; }
        public string gameserverip { get; set; }
        public string gameserversteamid { get; set; }
        public string gameextrainfo { get; set; }
        public string gameid { get; set; }
        public string vloccountrycode { get; set; }
        public string locstatecode { get; set; }
        public int loccityid { get; set; }
    }
}