using MEC;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using Exiled.Events.EventArgs;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Loader;
using System.Reflection;

namespace DCReplace
{
	class EventHandlers
	{
		private bool isContain106;
		private bool isRoundStarted = false;

		private Dictionary<Player, Vector3> PositionsToSpawn = new Dictionary<Player, Vector3>();

		private Player TryGet035()
		{
			Player scp035 = null;
			if (Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035") != null)
				scp035 = (Player)Loader.Plugins.First(pl => pl.Name == "scp035").Assembly.GetType("scp035.API.Scp035Data").GetMethod("GetScp035", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
			return scp035;
		}

		private List<Player> TryGetSH() 
		{
			List<Player> players = new List<Player>();
			if (Loader.Plugins.FirstOrDefault(pl => pl.Name == "SerpentsHand") != null)
				players = (List<Player>)Loader.Plugins.First(pl => pl.Name == "SerpentsHand").Assembly.GetType("SerpentsHand.API.SerpentsHand").GetMethod("GetSHPlayers", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
			return players;
		}

		private Dictionary<Player, bool> TryGetSpies()
		{
			Dictionary<Player, bool> players = new Dictionary<Player, bool>();
			if (Loader.Plugins.FirstOrDefault(pl => pl.Name == "CiSpy") != null)
				players = (Dictionary<Player, bool>)Loader.Plugins.First(pl => pl.Name == "CiSpy").Assembly.GetType("CISpy.API.SpyData").GetMethod("GetSpies", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
			return players;
		}

		private void TrySpawnSpy(Player player, Player dc, Dictionary<Player, bool> spies) 
		{
			if (Loader.Plugins.FirstOrDefault(pl => pl.Name == "CiSpy") != null)
			{
				Loader.Plugins.First(pl => pl.Name == "CiSpy").Assembly.GetType("CISpy.API.SpyData").GetMethod("MakeSpy", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { player, spies[dc], false });
			}
		}

		private void TrySpawnSH(Player player) 
		{
			if (Loader.Plugins.FirstOrDefault(pl => pl.Name == "SerpentsHand") != null)
			{
				Loader.Plugins.First(pl => pl.Name == "SerpentsHand").Assembly.GetType("SerpentsHand.API.SerpentsHand").GetMethod("SpawnPlayer", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { player, false });
			}
		} 
		private void TrySpawn035(Player player)
		{
			if (Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035") != null)
			{
				Loader.Plugins.First(pl => pl.Name == "scp035").Assembly.GetType("scp035.API.Scp035Data").GetMethod("Spawn035", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { player });
			}
		}

		public void OnRoundStart()
		{
			isContain106 = false;
			isRoundStarted = true;
			PositionsToSpawn.Clear();
		}

		public void OnRoundEnd() => isRoundStarted = false;

		public void OnContain106(ContainingEventArgs ev) => isContain106 = true;

		public void OnSpawning(SpawningEventArgs ev)
		{
			if (PositionsToSpawn.ContainsKey(ev.Player))
			{
				ev.Position = PositionsToSpawn[ev.Player];
				PositionsToSpawn.Remove(ev.Player);
			}
		}

		public void OnPlayerLeave(LeftEventArgs ev)
		{
			if (!isRoundStarted || ev.Player.Role == RoleType.Spectator || ev.Player.Position.y < -1997 || (ev.Player.CurrentRoom.Zone == ZoneType.LightContainment && Map.IsLczDecontaminated)) return;

			bool is035 = false;
			bool isSH = false;
			if (isContain106 && ev.Player.Role == RoleType.Scp106) return;
			Dictionary<Player, bool> spies = null;
			var role = Loader.Plugins.FirstOrDefault(pl => pl.Name == "EasyEvents")?.Assembly.GetType("EasyEvents.Util")?.GetMethod("GetRole")?.Invoke(null, new object[] {ev.Player});
			try
			{
				is035 = ev.Player.Id == TryGet035()?.Id;
			}
			catch (Exception x)
			{
				Log.Error(x);
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
				PositionsToSpawn.Add(player, ev.Player.Position);
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

				// save info
				var inventory = ev.Player.Items.Select(x => x.Type).ToList();
				float health = ev.Player.Health;
				Dictionary<ItemType, ushort> ammo = new Dictionary<ItemType, ushort>();
				foreach (ItemType ammoType in ev.Player.Ammo.Keys)
				{
					ammo.Add(ammoType, ev.Player.Ammo[ammoType]);
				}

				Timing.CallDelayed(0.3f, () =>
				{
					player.ResetInventory(inventory);
					player.Health = health;
					foreach (ItemType ammoType in ammo.Keys)
					{
						player.Inventory.UserInventory.ReserveAmmo[ammoType] = ammo[ammoType];
						player.Inventory.SendAmmoNextFrame = true;
					}
					player.Broadcast(5, "<i>You have replaced a player who has disconnected.</i>");
					if(role != null) Loader.Plugins.FirstOrDefault(pl => pl.Name == "EasyEvents")?.Assembly.GetType("EasyEvents.CustomRoles")?.GetMethod("ChangeRole")?.Invoke(null, new object[] {player, role});
				});
			}
		}
	}
}
