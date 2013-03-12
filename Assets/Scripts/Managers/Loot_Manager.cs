using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Loot_Manager
{
	// mob drop golds
	public static void DropLootOnMobDeath(Room room, Mob mob)
	{
		Gold gold = new Gold(mob.x, mob.y);
		room.lootList.Add(gold);
		room.AddChild(gold);
	}

	public static void CheckForPlayerLootCollisions (Player player, Room room)
	{
		foreach(Loot l in room.lootList)
		{
			if (l.Alive && l.Box.CheckIntersect(player.box))
			{
				if (l.Name == "gold")
				{
					player.ModifyGoldTotal((l as Gold).Amount);
					Debug.Log("golds: " + player.GoldCount);
				}
				l.Alive = false;
			}
		}
		CheckForDeadLoot(room);
	}
	
	private static void CheckForDeadLoot(Room room)
	{
		for(int i = room.lootList.Count - 1; i >= 0; i--)
		{
			if (!room.lootList[i].Alive)
			{
				room.RemoveChild(room.lootList[i]);
				room.lootList.Remove(room.lootList[i]);
			}
		}
	}
}