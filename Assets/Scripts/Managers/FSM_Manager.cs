using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public static class FSM_Manager
{
	// *** CHECK OUT MY TERRIBLE FINITE STATE MACHINE HEH *** //
	public static void HandleMobAI(Player player, Room room)
	{
		foreach (Mob mob in room.mobList)
		{
			switch (mob.mobState)
			{
			case MobState.Wander:
				if (mob.WithinRangeOfPlayer(player))
				{
					mob.mobState = MobState.Aggressive;
					break;
				}
				else 
				{
					mob.Wander(room);
					break;
				}
			case MobState.Aggressive:
				if (mob.CanAttackPlayer(player))
				{
					Combat_Manager.MobAttackPlayer(mob, player, room);
					break;
				}
				else if (!mob.WithinRangeOfPlayer(player))
				{
					mob.mobState = MobState.Wander;
					break;
				}
				else
				{
					mob.MoveTowardsPlayer(player, room);
					break;
				}
			}
		}
	}
}