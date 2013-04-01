using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : Mob
{
	public int GoldCount = 0;
	
	public Direction AnimationFacing;
	
	public Player (int x, int y) : base("man", x, y) 
	{
		moveSpeed = 2f;
		hostileDistance = 100.0;
		attackDistance = 30.0;
		HP = 3;
		AttackPower = 3;
		CanAttack = true;
		AnimationFacing = Direction.None;
	}
	
	public void ModifyGoldTotal(int amount)
	{
		GoldCount += amount;
		if (GoldCount < 0) GoldCount = 0;
	}
}