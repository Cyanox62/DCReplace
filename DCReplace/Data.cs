using System.Collections.Generic;
using UnityEngine;

namespace DCReplace
{
	public class Data
	{
		public RoleType role;
		public Vector3 pos;
		public Vector3 rot;
		public List<Inventory.SyncItemInfo> items = new List<Inventory.SyncItemInfo>();
		public float health;
		public uint ammo1;
		public uint ammo2;
		public uint ammo3;
	}
}
