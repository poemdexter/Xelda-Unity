using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackBox
{
	public Rect box;
	public int Damage;
	public Mob owner;
	public bool alive;
	public AttackType attackType;
}

public enum AttackType
{
	Melee,
	Ranged
}

public static class Combat_Manager
{
	public static void PlayerAttack(Player player, Room room)
	{
		// create collision box in room
		AttackBox abox = new AttackBox();
		abox.box = player.box;
		abox.Damage = player.AttackPower;
		abox.owner = player;
		abox.alive = true;
		abox.attackType = AttackType.Melee;
		room.attackBoxList.Add(abox);
	}
	
	public static void MobAttackPlayer(Mob mob, Player player, Room room)
	{
		// create collision box in room
		AttackBox abox = new AttackBox();
		abox.box = mob.box;
		abox.Damage = mob.AttackPower;
		abox.owner = mob;
		abox.alive = true;
		abox.attackType = AttackType.Melee;
		room.attackBoxList.Add(abox);
	}
	
	public static void CheckCombatCollisions(Player player, Room room)
	{
		// check collisions of attacks against mobs
		foreach (Mob mob in room.mobList)
		{
			foreach (AttackBox abox in room.attackBoxList)
			{
				// player attack hit mob
				if (abox.alive && abox.owner == player && abox.box.CheckIntersect(mob.box))
				{
					mob.HP -= abox.Damage;
					if (mob.HP <= 0) mob.Alive = false;
					abox.alive = false;
				}
			}
		}
		
		// check collisions of attacks against player
		foreach (AttackBox abox in room.attackBoxList)
		{
			// mob attack hits player
			if (abox.alive && abox.owner != player && abox.box.CheckIntersect(player.box))
			{
				player.HP -= abox.Damage;
				if (player.HP <= 0) player.Alive = false;
				abox.alive = false;
			}
		}
	}
	
	public static void CheckForDeadAttackBoxes(Room room)
	{
		for(int x = room.attackBoxList.Count - 1; x >= 0; x--)
		{
			// if attack is dead or melee, get rid of it.  melee attacks only last 1 frame atm
			if (!room.attackBoxList[x].alive || room.attackBoxList[x].attackType == AttackType.Melee)
			{
				room.attackBoxList.Remove(room.attackBoxList[x]);
			}
		}
	}
	
	public static void CheckForDeadMobs(Room room)
	{
		for(int x = room.mobList.Count - 1; x >= 0; x--)
		{
			if (!room.mobList[x].Alive)
			{
				room.RemoveChild(room.mobList[x]);
				room.mobList.Remove(room.mobList[x]);
			}
		}
	}
}