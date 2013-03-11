using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Projectile : FSprite
{
	public Rect Box;
	public int Damage {get;set;}
	public Mob Owner {get;set;}
	public bool Alive {get;set;}
	public Direction Facing {get;set;}
	public int MoveSpeed {get;set;}
	
	public Projectile (Mob mob) : base("bullet.png")
	{
		Damage = mob.AttackPower;
		Owner = mob;
		Alive = true;
		Facing = mob.Facing;
		MoveSpeed = 3;
		
		// sprite stuff
		this.x = mob.x;
		this.y = mob.y;
		this.anchorX = 0;
		this.anchorY = 0;
		
		Box = new Rect(this.x, this.y, this.width, this.height);
	}
	
		public void Move (float X, float Y)
	{
		this.x += X * MoveSpeed;
		this.y += Y * MoveSpeed;
		this.Box.x = this.x;
		this.Box.y = this.y;
	}
}