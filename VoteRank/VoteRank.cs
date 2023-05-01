using System.Timers;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace VoteRank
{
    public class VoteRank : RocketPlugin<VoteRankConfiguration>
    {
        public static VoteRank Instance;
        private static System.Timers.Timer CheckTimer;
        private static Timer RemoveTimer;
        protected override void Load()
        {
            Instance = this;
            Logger.Log(@"
                ╔═══════════════════════════════╗
                ║       VoteRank by Pixel8      ║
                ║ https://discord.gg/HtChrYJkYd ║
                ╚═══════════════════════════════╝
            ");
            SetTimer();
            SetRemoveTimer();
        }

        protected override void Unload()
        {
            Instance = null;
            Logger.Log(@"
                ╔═══════════════════════════════╗
                ║       VoteRank by Pixel8      ║
                ║ https://discord.gg/HtChrYJkYd ║
                ╚═══════════════════════════════╝
            ");
        }
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    {"reward_announcement", "{0} voted on {1} and has received a reward! Vote now!"},
                };
            }
        }
        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            CheckTimer = new System.Timers.Timer(VoteRank.Instance.Configuration.Instance.VoteCheckInterval * 1000);
            // Hook up the Elapsed event for the timer. 
            CheckTimer.Elapsed += OnTimedEvent;
            CheckTimer.AutoReset = true;
            CheckTimer.Enabled = true;
        }
        private static void SetRemoveTimer()
        {
            // Create a timer with a two second interval.
            RemoveTimer = new System.Timers.Timer(86400000);
            // Hook up the Elapsed event for the timer. 
            RemoveTimer.Elapsed += RemovePlayers;
            RemoveTimer.AutoReset = true;
            RemoveTimer.Enabled = true;
        }

        private static void RemovePlayers(object sender, ElapsedEventArgs e)
        {
            var group = R.Permissions.GetGroup(VoteRank.Instance.Configuration.Instance.RankName);
            group.Members.Clear();
            R.Permissions.Reload();
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Logger.Log("VoteRank >> Checking all players for Vote!");
            foreach (var player in Provider.clients)
            {
                VoteHandler.HandleVote(UnturnedPlayer.FromSteamPlayer(player), true);
            }
        }
    }
}