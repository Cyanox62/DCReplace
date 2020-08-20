using MEC;
using System.Linq;
using UnityEngine;
using scp035.API;
using System;
using CISpy.API;
using System.Collections.Generic;
using Exiled.Events.EventArgs;
using Exiled.API.Features;
using Exiled.API.Enums;

namespace DCReplace
{
	class EventHandlers
	{
		private bool isContain106;
		private bool isRoundStarted = false;

		private Player TryGet035() => Scp035Data.GetScp035();

		private List<Player> TryGetSH() => SerpentsHand.API.SerpentsHand.GetSHPlayers();

		private Dictionary<Player, bool> TryGetSpies() => SpyData.GetSpies();

		private void TrySpawnSpy(Player player, Player dc, Dictionary<Player, bool> spies) => SpyData.MakeSpy(player, spies[dc], false);

		private void TrySpawnSH(Player player) => SerpentsHand.API.SerpentsHand.SpawnPlayer(player, false);

		private void TrySpawn035(Player player) => Scp035Data.Spawn035(player);

		public void OnRoundStart()
		{
			isContain106 = false;
			isRoundStarted = true;
		}

		public void OnRoundEnd() => isRoundStarted = false;

		public void OnContain106(ContainingEventArgs ev) => isContain106 = true;

		public void OnPlayerLeave(LeftEventArgs ev)
		{
			if (!isRoundStarted || ev.Player.Role == RoleType.Spectator) return;

			bool is035 = false;
			bool isSH = false;
			if (isContain106 && ev.Player.Role == RoleType.Scp106) return;
			Dictionary<Player, bool> spies = null;
			try
			{
				is035 = ev.Player.Id == TryGet035()?.Id;
			}
			catch (Exception x)
			{
				Log.Debug("SCP-035 is not installed, skipping method call...");
			}

			try
			{
				isSH = TryGetSH().Contains(ev.Player);
			}
			catch (Exception x)
			{
				Log.Debug("Serpents Hand is not installed, skipping method call...");
			}

			try
			{
				spies = TryGetSpies();
			}
			catch (Exception x)
			{
				Log.Debug("CISpy is not installed, skipping method call...");
			}

			Player player = Player.List.FirstOrDefault(x => x.Role == RoleType.Spectator && x.UserId != string.Empty && x.UserId != ev.Player.UserId && !x.IsOverwatchEnabled);
			if (player != null)
			{
				if (isSH)
				{
					try
					{
						TrySpawnSH(player);
					}
					catch (Exception x)
					{
						Log.Debug("Serpents Hand is not installed, skipping method call...");
					}
				}
				else player.SetRole(ev.Player.Role);
				if (spies != null && spies.ContainsKey(ev.Player))
				{
					try
					{
						TrySpawnSpy(player, ev.Player, spies);
					}
					catch (Exception x)
					{
						Log.Debug("CISpy is not installed, skipping method call...");
					}
				}
				if (is035)
				{
					try
					{
						TrySpawn035(player);
					}
					catch (Exception x)
					{
						Log.Debug("SCP-035 is not installed, skipping method call...");
					}
				}
				player.Position = ev.Player.Position;
				player.ClearInventory();
				player.ResetInventory(ev.Player.Inventory.items.Select(x => x.id).ToList());
				player.Health = ev.Player.Health;
				player.Ammo[(int)AmmoType.Nato556] = ev.Player.Ammo[(int)AmmoType.Nato556];
				player.Ammo[(int)AmmoType.Nato762] = ev.Player.Ammo[(int)AmmoType.Nato762];
				player.Ammo[(int)AmmoType.Nato9] = ev.Player.Ammo[(int)AmmoType.Nato9];
				player.Broadcast(5, "<i>You have replaced a player who has disconnected.</i>");
			}
		}
	}
}
