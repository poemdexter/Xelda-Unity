using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Skeleton : Mob
{
	public Skeleton (int x, int y) : base("skeleton", x, y) 
	{
		moveSpeed = 2f;
		hostileDistance = 100.0;
		HP = 3;
		AttackPower = 1;
	}
}