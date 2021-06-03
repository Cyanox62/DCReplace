using MEC;
using System.Linq;
using System;
using Exiled.Events.EventArgs;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Loader;

namespace DCReplace
{
	class EventHandlers
	{
		private bool _isContain106;
		private bool _isRoundStarted;

		private readonly DCReplace _plugin;

		public EventHandlers(DCReplace plugin)
		{
			_plugin = plugin;
		}

		public void OnRoundStart()
		{
			_isContain106 = false;
			_isRoundStarted = true;
		}

		public void OnRoundEnd() => _isRoundStarted = false;

		public void OnContain106(ContainingEventArgs ev) => _isContain106 = true;

		public void OnPlayerLeave(LeftEventArgs ev)
		{
			if (!_isRoundStarted || ev.Player.Role == RoleType.Spectator || ev.Player.Position.y < -1997 || (ev.Player.CurrentRoom.Zone == ZoneType.LightContainment && Map.IsLCZDecontaminated)) return;

			var is035 = false;
			var isSH = false;
			
			if (_isContain106 && ev.Player.Role == RoleType.Scp106) return;
			var role = Loader.Plugins.FirstOrDefault(pl => pl.Name == "EasyEvents")?.Assembly.GetType("EasyEvents.Util")?.GetMethod("GetRole")?.Invoke(null, new object[] {ev.Player});
			try
			{
				is035 = Scp035.API.IsScp035(ev.Player);
			}
			catch (Exception x)
			{
				Log.Debug($"SCP-035 is not installed, skipping method call... {x}");
			}

			try
			{
				isSH = SerpentsHand.API.IsSerpent(ev.Player);
			}
			catch (Exception x)
			{
				Log.Debug($"Serpents Hand is not installed, skipping method call... {x}");
			}

			var player = Player.List.FirstOrDefault(x => x.Role == RoleType.Spectator && x.UserId != string.Empty && x.UserId != ev.Player.UserId && !x.IsOverwatchEnabled);
			if (player == null) return;
			{
				if (isSH)
				{
					try
					{
						SerpentsHand.API.SpawnPlayer(player);
					}
					catch (Exception x)
					{
						Log.Debug($"Serpents Hand is not installed, skipping method call... {x}");
					}
				}
				else player.SetRole(ev.Player.Role);
				if (is035)
				{
					try
					{
						Scp035.API.Spawn035(player);
					}
					catch (Exception x)
					{
						Log.Debug($"SCP-035 is not installed, skipping method call... {x}");
					}
				}

				// save info
				var pos = ev.Player.Position;
				var inventory = ev.Player.Inventory.items.Select(x => x.id).ToList();
				var health = ev.Player.Health;
				var ammo1 = ev.Player.Ammo[(int)AmmoType.Nato556];
				var ammo2 = ev.Player.Ammo[(int)AmmoType.Nato762];
				var ammo3 = ev.Player.Ammo[(int)AmmoType.Nato9];

				Timing.CallDelayed(0.3f, () =>
				{
					player.Position = pos;
					player.ClearInventory();
					player.ResetInventory(inventory);
					player.Health = health;
					player.Ammo[(int)AmmoType.Nato556] = ammo1;
					player.Ammo[(int)AmmoType.Nato762] = ammo2;
					player.Ammo[(int)AmmoType.Nato9] = ammo3;

					if (_plugin.Config.UseHints)
					{
						player.ShowHint(_plugin.Config.ReplaceMessage, _plugin.Config.MessageDuration);
					}
					else
					{
						player.Broadcast(_plugin.Config.MessageDuration, _plugin.Config.ReplaceMessage);
					}

					if(role != null) Loader.Plugins.FirstOrDefault(pl => pl.Name == "EasyEvents")?.Assembly.GetType("EasyEvents.CustomRoles")?.GetMethod("ChangeRole")?.Invoke(null, new object[] {player, role});
				});
			}
		}
	}
}
