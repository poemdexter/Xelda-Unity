using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Loot : FSprite
{
	public Rect Box;
	public bool Alive;
	
	public Loot (float x, float y, string name) : base(name + ".png")
	{
		this.x = x;
		this.y = y;
		this.anchorX = 0;
		this.anchorY = 0;
		Box = new Rect(this.x, this.y, this.width, this.height);
		Alive = true;
	}
}