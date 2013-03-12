using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Gold : Loot
{
	public int Amount;
	
	public Gold (float x, float y) : base(x,y,"gold")
	{
		Amount = 1;
	}
}