using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : Mob
{
	public Player (int x, int y) : base("man", x, y) 
	{
		moveSpeed = 2f;
		hostileDistance = 100.0;
		attackDistance = 30.0;
		HP = 3;
		AttackPower = 1;
	}
	
	public AttackBox UseWeapon(Room room)
	{
		// get weapon type
		// do weapon attack
		AttackBox aBox = new AttackBox();
		aBox.box.x = this.x + 4;
		aBox.box.y = this.y + 4;
		aBox.box.width = this.width - 8;
		aBox.box.height = this.height - 8;
		aBox.Damage = AttackPower;
		return aBox;
	}
}