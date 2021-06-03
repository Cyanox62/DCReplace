using Exiled.API.Features;

namespace DCReplace
{
	public class DCReplace : Plugin<Config>
	{
		private EventHandlers _ev;

		public override void OnEnabled()
		{
			if (!Config.IsEnabled) return;

			_ev = new EventHandlers(this);

			Exiled.Events.Handlers.Player.Left += _ev.OnPlayerLeave;
			Exiled.Events.Handlers.Scp106.Containing += _ev.OnContain106;
			Exiled.Events.Handlers.Server.RoundStarted += _ev.OnRoundStart;
			
			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Player.Left -= _ev.OnPlayerLeave;
			Exiled.Events.Handlers.Scp106.Containing -= _ev.OnContain106;
			Exiled.Events.Handlers.Server.RoundStarted -= _ev.OnRoundStart;

			_ev = null;
			
			base.OnDisabled();
		}

		public override string Name => "DcReplace";
	}
}
