using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum MobState
{
	Wander,
	Aggressive
}

public class Mob : FSprite
{
	public Rect box;
	public string name;
	public MobState mobState;
	protected float _moveSpeed;
	
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
		
		mobState = MobState.Wander;
	}
	
	public virtual bool WithinRangeOfPlayer(Mob player) { return false; }
	public virtual bool CanAttackPlayer(Mob player) { return false; }
	public virtual void MoveTowardsPlayer(Mob player, Map map) {}
	public virtual void AttackPlayer(Mob player) {}
	public virtual void Wander(Map map) {}
	
	public void Move (float X, float Y)
	{
		this.x += X;
		this.y += Y;
		this.box.x = this.x + 4;
		this.box.y = this.y + 4;
	}
	
	protected bool CollisionOccurred(Direction d, Map map)
	{
		Rect collisionRect = this.box;
		bool collision = false;
	
		if (d == Direction.N) collisionRect.y = collisionRect.y + _moveSpeed;
		if (d == Direction.S) collisionRect.y = collisionRect.y - _moveSpeed;
		if (d == Direction.W) collisionRect.x = collisionRect.x - _moveSpeed;
		if (d == Direction.E) collisionRect.x = collisionRect.x + _moveSpeed;
		
		// hit wall
		foreach(collisionBox cbox in map.collisionBoxList)
		{
			if (collisionRect.CheckIntersect(cbox.box))
			{
				Debug.Log("collision:)");
				collision = true;
				break;
			}
		}
		
		// hit wall segment blocking passageway
		// we don't care if it's active or not because we don't want mobs to try to transition
		foreach(collisionBox cbox in map.passageObjectBoxList)
		{
			if (collisionRect.CheckIntersect(cbox.box))
			{
				Debug.Log("collision:)");
				collision = true;
				break;
			}
		}
		
		return collision;
	}
}