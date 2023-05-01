using System.Net;
using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace VoteRank
{
    public class VoteHandler
    {
        public static string GetVote(UnturnedPlayer player)
        {
            WebClient wc = new WebClient();
            string result = null;

            if (string.IsNullOrEmpty(VoteRank.Instance.Configuration.Instance.APIKey))
            {
                Logger.LogError("\nVoteRank >> API key(s) not found\n");

                return null;
            }

            try
            {
                result = wc.DownloadString(string.Format("http://unturned-servers.net/api/?object=votes&element=claim&key={0}&steamid={1}", VoteRank.Instance.Configuration.Instance.APIKey, player.CSteamID.m_SteamID));
            }
            catch (WebException)
            {
                Logger.LogError(string.Format("\nVoteRank >> Could not connect to Unturned-Servers.net's API\n"));
            
                return null;
            }

            
            if (result.Length != 1)
            {
                if (result == "Error: invalid server key")
                {
                    Logger.LogError("\nVoteRank >> API key is invalid\n");
                }
                else if (result == "Error: no server key")
                {
                    Logger.LogError("\nVoteRank >> API key not found\n");
                }
                return null;
            }
            return result;
        }
        public static bool SetVote(UnturnedPlayer player)
        {
            WebClient wc = new WebClient();
            string result = null;
            string url = "http://unturned-servers.net/api/?action=post&object=votes&element=claim&key={0}&steamid={1}";;

            if (string.IsNullOrEmpty(VoteRank.Instance.Configuration.Instance.APIKey))
            {
                Logger.LogError("\nVoteRank >> API key(s) not found\n");

                return false;
            }
            try
            {
                result = wc.DownloadString(string.Format(url, VoteRank.Instance.Configuration.Instance.APIKey, player.CSteamID.m_SteamID));
            }
            catch (WebException)
            {
                Logger.LogError(string.Format("\nVoteRewards >> Could not connect to Unturned-Servers.net's API\n"));

                return false;
            }

            if (result.Length != 1)
            {
                Logger.LogError(string.Format("\nVoteRewards >> Unturned-Servers.net's API cannot be used with this plugin\n"));

                return false;
            }

            if (result == "0") // Not claimed
            {
                return false;
            }
            if (result == "1") // Claimed
            {
                return true;
            }

            return false;
        }
        public static void HandleVote(UnturnedPlayer player, bool giveReward)
        {
            string voteResult = GetVote(player);;
            
            if (voteResult == null && giveReward == true)
            {
                
            }
            else
            {
                if (voteResult == "1") // Has voted & not claimed
                {
                    if (giveReward)
                    {
                        if (SetVote(player))
                        {
                            GiveReward(player);
                        }
                    }
                }
            }
        }
        public static void GiveReward(UnturnedPlayer player)
        {
            R.Permissions.AddPlayerToGroup(VoteRank.Instance.Configuration.Instance.RankName, player);
            R.Permissions.Reload();
            if (VoteRank.Instance.Configuration.Instance.GlobalAnnouncement)
            {
                foreach (SteamPlayer sP in Provider.clients)
                {
                    var p = sP.playerID.steamID;
                    if (p != player.CSteamID)
                    {
                        ChatManager.say(p, VoteRank.Instance.Translate("reward_announcement", player.CharacterName, "Unturned-Servers.net"), Color.green, EChatMode.GLOBAL);
                    }
                }
            }
        }
    }
}