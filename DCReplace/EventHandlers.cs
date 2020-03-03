using EXILED;
using EXILED.Extensions;
using MEC;
using System.Linq;
using UnityEngine;
using scp035.API;
using System;

namespace DCReplace
{
	class EventHandlers
	{
		public void OnPlayerLeave(PlayerLeaveEvent ev)
		{
			bool is035 = false;
			bool isSH = false;
			try
			{
				is035 = ev.Player.queryProcessor.PlayerId == Scp035Data.GetScp035()?.queryProcessor.PlayerId;
			}
			catch (Exception x)
			{
				Log.Warn("SCP-035 is not installed, skipping method call...");
			}

			try
			{
				isSH = SerpentsHand.API.SerpentsHand.GetSHPlayers().Contains(ev.Player.queryProcessor.PlayerId);
			}
			catch (Exception x)
			{
				Log.Warn("SerpentsHand is not installed, skipping method call...");
			}

			Inventory.SyncListItemInfo items = ev.Player.inventory.items;
			RoleType role = ev.Player.GetRole();
			Vector3 pos = ev.Player.transform.position;
			int health = (int)ev.Player.playerStats.health;
			string ammo = ev.Player.ammoBox.amount;

			ReferenceHub player = Player.GetHubs().FirstOrDefault(x => x.GetRole() == RoleType.Spectator && x.characterClassManager.UserId != string.Empty && !x.GetOverwatch());
			if (player != null)
			{
				if (isSH) SerpentsHand.API.SerpentsHand.SpawnPlayer(player, false);
				else player.SetRole(role);
				if (is035) Scp035Data.Spawn035(player);
				Timing.CallDelayed(0.3f, () =>
				{
					player.SetPosition(pos);
					player.inventory.items.ToList().Clear();
					foreach (var item in items)
					{
						player.inventory.AddNewItem(item.id);
					}
					player.playerStats.health = health;
					player.ammoBox.Networkamount = ammo;
					player.Broadcast(5, "<i>You have replaced a player who has disconnected.</i>", false);
				});
			}
		}
	}
}
