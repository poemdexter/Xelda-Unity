using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Combat_Manager
{
	public static void HandleProjectileMovement (Room currentRoom)
	{
		foreach(Projectile p in currentRoom.projectileList)
		{
			if (p.Facing == Direction.N) p.Move(0,1);
			if (p.Facing == Direction.S) p.Move(0,-1);
			if (p.Facing == Direction.W) p.Move(-1,0);
			if (p.Facing == Direction.E) p.Move(1,0);
		}
	}
	
	public static void PlayerAttack(Player player, Room room)
	{
		// create projectile box in room
		Projectile proj = new Projectile(player);
		room.AddProjectile(proj);
	}
	
	public static void MobAttackPlayer(Mob mob, Player player, Room room)
	{
		// create collision box in room
		Projectile proj = new Projectile(mob);
		room.AddProjectile(proj);
	}
	
	public static void CheckCombatCollisions(Player player, Room room)
	{
		// check collisions of projectiles
		foreach (Projectile p in room.projectileList)
		{
			// mob attack hits player
			if (p.Alive && p.Owner != player && p.Box.CheckIntersect(player.box))
			{
				player.ResolveDamage(p.Damage);
				p.Alive = false;
			}
				
			foreach (Mob mob in room.mobList)
			{	
				// player attack hit mob
				if (p.Alive && p.Owner == player && p.Box.CheckIntersect(mob.box))
				{
					mob.ResolveDamage(p.Damage);
					p.Alive = false;
				}
			}
			
			// projectile hit wall or other non mob entity
			foreach (CollisionBox box in room.NonMobCollidables)
			{
				if (p.Alive && box.active && p.Box.CheckIntersect(box.box)) p.Alive = false;
			}
		}
		
		// check collisions player to mob (point blank)
		foreach (Mob mob in room.mobList)
		{
			if (mob.Alive && mob.box.CheckIntersect(player.box))
			{
				mob.ResolveDamage(1);
				player.ResolveDamage(1);
				
				// get vector2 of collision
				Vector2 colV = (new Vector2(mob.x - player.x, mob.y - player.y));
				colV.Normalize();
				
				// push both in opposite directions of collision
				int power = 15;
				mob.Move(colV.x * power, colV.y * power);
				player.Move(-(colV.x * power), -(colV.y * power));
			}
		}
		
		CheckForDeadProjectiles(room);
		CheckForDeadMobs(room);
	}
	
	private static void CheckForDeadProjectiles(Room room)
	{
		for(int i = room.projectileList.Count - 1; i >= 0; i--)
		{
			// if projectile is dead, remove it
			if (!room.projectileList[i].Alive)
			{
				room.RemoveProjectile(room.projectileList[i]);
			}
		}
	}
	
	private static void CheckForDeadMobs(Room room)
	{
		for(int i = room.mobList.Count - 1; i >= 0; i--)
		{
			if (!room.mobList[i].Alive)
			{
				Loot_Manager.DropLootOnMobDeath(room, room.mobList[i]);
				room.RemoveChild(room.mobList[i]);
				room.mobList.Remove(room.mobList[i]);
			}
		}
	}
}