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
			Events.Scp106ContainEvent += ev.OnContain106;
			Events.RoundStartEvent += ev.OnRoundStart;
		}

		public override void OnDisable()
		{
			ev = null;
		}

		public override void OnReload() { }

		public override string getName { get; } = "DCReplace";
	}
}
