using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Skeleton : Mob
{
	public Skeleton (int x, int y) : base("man", x, y) {}
	
	public override void Wander(Map map) 
	{
		x += .1f;
	}
}