using Rocket.API;

namespace VoteRank
{
    public class VoteRankConfiguration : IRocketPluginConfiguration
    {
        public string APIKey;
        public string RankName;
        public int VoteCheckInterval;
        public bool GlobalAnnouncement;
        public void LoadDefaults()
        {
            APIKey = "";
            RankName = "voter";
            VoteCheckInterval = 300;
            GlobalAnnouncement = true;
        }
    }
}