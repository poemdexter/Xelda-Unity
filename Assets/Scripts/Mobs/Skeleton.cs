using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Skeleton : Mob
{
	private int duration = 0;
	private Direction dir;
	
	public Skeleton (int x, int y) : base("man", x, y) 
	{
		_moveSpeed = 2f;
	}
	
	public override void Wander(Map map) 
	{
		if (duration <= 0) {
			// select a random direction
			dir = (Direction)XeldaGame.rand.Next(0,4);
			
			// if collision, choose direction NOT in collision direction until we get a winner
			while (CollisionOccurred(dir, map)) dir = (Direction)XeldaGame.rand.Next(0,4);
			
			// select a random duration
			duration = 100; //XeldaGame.rand.Next(10,100);
		}
		else
		{
			if (CollisionOccurred(dir, map))
			{
				duration = 0;
				return;
			}
			else
			{
				// move in that direction
				if (dir == Direction.N) Move (0,1);
				if (dir == Direction.S) Move (0,-1);
				if (dir == Direction.W) Move (-1,0);
				if (dir == Direction.E) Move (1,0);
				
				// decrement
				duration--;
			}
		}
	}
}