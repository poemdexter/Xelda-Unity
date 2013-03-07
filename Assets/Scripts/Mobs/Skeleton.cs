using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Skeleton : Mob
{
	
	public Skeleton (int x, int y) : base("man", x, y) 
	{
		_moveSpeed = 2f;
		_hostileDistance = 100.0;
		_attackDistance = 30.0;
	}
}