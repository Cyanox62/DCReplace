using Exiled.API.Features;

namespace DCReplace
{
	public class DCReplace : Plugin<Config>
	{
		private EventHandlers ev;

		public override void OnEnabled()
		{
			base.OnEnabled();

			if (!Config.IsEnabled) return;

			ev = new EventHandlers();

			Exiled.Events.Handlers.Player.Left += ev.OnPlayerLeave;
			Exiled.Events.Handlers.Scp106.Containing += ev.OnContain106;
			Exiled.Events.Handlers.Server.RoundStarted += ev.OnRoundStart;
		}

		public override void OnDisabled()
		{
			base.OnDisabled();

			Exiled.Events.Handlers.Player.Left -= ev.OnPlayerLeave;
			Exiled.Events.Handlers.Scp106.Containing -= ev.OnContain106;
			Exiled.Events.Handlers.Server.RoundStarted -= ev.OnRoundStart;

			ev = null;
		}

		public override string Name => "DcReplace";
	}
}
