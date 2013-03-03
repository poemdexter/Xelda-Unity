using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Mob : FSprite
{
	public Rect box;
	public string name;
	
	public Mob (string Name, int X, int Y) : base(Name + ".png")
	{
		name = Name;
		this.x = X;
		this.y = Y;
		this.anchorX = 0;
		this.anchorY = 0;
		
		box.x = this.x + 4;
		box.y = this.y + 4;
		box.width = this.width - 8;
		box.height = this.height - 8;
	}
	
	public void Move (float X, float Y)
	{
		this.x += X;
		this.y += Y;
		box.x = this.x + 4;
		box.y = this.y + 4;
	}
}