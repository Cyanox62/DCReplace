using EXILED;

namespace DCReplace
{
	public class Plugin : EXILED.Plugin
	{
		private EventHandlers ev;

		public override void OnEnable()
		{
            this.enabled = Plugin.Config.GetBool("dcr_enabled", true);
            if (!this.enabled)
            {
                Plugin.Error("DCReplace will not load, is disabled by config.");
                return;
            }
            MsgConfig.ReplaceCustomMsg = Plugin.Config.GetString("dcr_message", "<i>You have replaced a disconnected player</i>");
            Plugin.Info("DCReplace loaded successfully.");
            ev = new EventHandlers();

			Events.PlayerLeaveEvent += ev.OnPlayerLeave;
		}

		public override void OnDisable()
		{
			ev = null;
		}

		public override void OnReload() { }
        public void ReloadConfig()
        {
            MsgConfig.ReplaceCustomMsg = Plugin.Config.GetString("dcr_message", "<i>You have replaced a disconnected player</i>");
        }


        public override string getName { get; } = "DCReplace";
        public bool enabled;
    }
}
