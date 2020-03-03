using EXILED;

namespace DCReplace
{
	public class Plugin : EXILED.Plugin
	{
		private EventHandlers ev;

		public override void OnEnable()
		{
			ev = new EventHandlers();

			Events.PlayerLeaveEvent += ev.OnPlayerLeave;
		}

		public override void OnDisable()
		{
			ev = null;
		}

		public override void OnReload() { }

		public override string getName { get; } = "DCReplace";
	}
}
