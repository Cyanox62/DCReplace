using EXILED;
using EXILED.Extensions;
using MEC;
using System.Linq;
using UnityEngine;
using scp035.API;
using System;
using CISpy.API;
using System.Collections.Generic;

namespace DCReplace
{
	class EventHandlers
	{
		private bool isContain106;

		private ReferenceHub TryGet035()
		{
			return Scp035Data.GetScp035();
		}

		private List<int> TryGetSH()
		{
			return SerpentsHand.API.SerpentsHand.GetSHPlayers();
		}

		private Dictionary<ReferenceHub, bool> TryGetSpies()
		{
			return SpyData.GetSpies();
		}

		private void TrySpawnSpy(ReferenceHub player, ReferenceHub dc, Dictionary<ReferenceHub, bool> spies)
		{
			SpyData.MakeSpy(player, spies[dc], false);
		}

		private void TrySpawnSH(ReferenceHub player)
		{
			SerpentsHand.API.SerpentsHand.SpawnPlayer(player, false);
		}

		private void TrySpawn035(ReferenceHub player)
		{
			Scp035Data.Spawn035(player);
		}

		public void OnRoundStart()
		{
			isContain106 = false;
		}

		public void OnContain106(Scp106ContainEvent ev)
		{
			isContain106 = true;
		}

		public void OnPlayerLeave(PlayerLeaveEvent ev)
		{
			if (ev.Player.GetTeam() != Team.RIP)
			{
				bool is035 = false;
				bool isSH = false;
				Dictionary<ReferenceHub, bool> spies = null;
				try
				{
					is035 = ev.Player.queryProcessor.PlayerId == TryGet035()?.queryProcessor.PlayerId;
				}
				catch (Exception x)
				{
					Log.Debug("SCP-035 is not installed, skipping method call...");
				}

				try
				{
					isSH = TryGetSH().Contains(ev.Player.queryProcessor.PlayerId);
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

				Inventory.SyncListItemInfo items = ev.Player.inventory.items;
				RoleType role = ev.Player.GetRole();
				Vector3 pos = ev.Player.transform.position;
				int health = (int)ev.Player.playerStats.health;
				string ammo = ev.Player.ammoBox.amount;

				ReferenceHub player = Player.GetHubs().FirstOrDefault(x => x.GetRole() == RoleType.Spectator && x.characterClassManager.UserId != string.Empty && !x.GetOverwatch());
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
					else player.SetRole(role);
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
					if (isContain106 && ev.Player.GetRole() == RoleType.Scp106) return;
					Timing.CallDelayed(0.3f, () =>
					{
						player.SetPosition(pos);
						player.inventory.items.ToList().Clear();
						foreach (var item in items) player.inventory.AddNewItem(item.id);
						player.playerStats.health = health;
						player.ammoBox.Networkamount = ammo;
						player.Broadcast(5, "<i>You have replaced a player who has disconnected.</i>", false);
					});
				}
			}
		}
	}
}
