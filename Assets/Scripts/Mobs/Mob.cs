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
	public Direction Facing {get; set;}
	
	// perhaps have vector here that stores x and y of firing direction so that we can move projectiles in a clever way
	
	public float moveSpeed;
	protected double hostileDistance;
	protected int duration = 0;
	protected int a_duration = 0;
	protected Direction a_dir;
	
	// combat related
	public int HP;
	public int AttackPower;
	public bool Alive;
	public int attackDelay = 0;
	public int attackDelayTime = 50;
	protected bool CanAttack = false;
	protected double attackDistance = 0;
	
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
		
		Facing = Direction.S;
		mobState = MobState.Wander;
		Alive = true;
	}
	
	// *** OVERRIDE THESE IF MOBS NEED CUSTOM BEHAVIOR *** //
	public virtual bool WithinRangeOfPlayer(Mob player) 
	{
		return (getDistanceToPlayer(player) < hostileDistance);
	}
	
	public virtual bool CanAttackPlayer(Mob player) 
	{
		if (attackDelay <= 0 && CanAttack && getDistanceToPlayer(player) < attackDistance)
		{
			attackDelay = attackDelayTime;
			return true;
		}
		else
		{
			attackDelay--;
			return false;
		}
	}
	
	public virtual void Wander(Room room) 
	{
		Direction dir = Direction.None;
		
		// if we aren't moving
		if (duration <= 0) 
		{
			// select a random direction
			dir = (Direction)XeldaGame.rand.Next(0,4);
			
			// if collision, choose direction NOT in collision direction until we get a winner
			while (CollisionOccurred(dir, room)) dir = (Direction)XeldaGame.rand.Next(0,4);
			
			// select a random duration
			duration = XeldaGame.rand.Next(50,200);
			
			// set facing direction
			this.Facing = dir;
		}
		else // we're moving
		{
			// if we hit something, restart the wandering process
			if (CollisionOccurred(this.Facing, room))
			{
				duration = 0;
				return;
			}
			else
			{
				// move in that direction
				if (this.Facing == Direction.N) Move (0,1);
				if (this.Facing == Direction.S) Move (0,-1);
				if (this.Facing == Direction.W) Move (-1,0);
				if (this.Facing == Direction.E) Move (1,0);
				
				// decrement timer
				duration--;
			}
		}
	}
	
	// added a duration so that mob isn't gyrating diagonally
	public virtual void MoveTowardsPlayer(Mob player, Room room)
	{
		if (a_duration <= 0)
		{
			this.Facing = getDirectionTowardsPlayer(player, room);
			a_duration = 20;
		}		
		if (CollisionOccurred(this.Facing, room))
		{
			a_duration = 0;
			return;
		}
		else
		{
			if (this.Facing == Direction.N) Move (0,1);
			if (this.Facing == Direction.S) Move (0,-1);
			if (this.Facing == Direction.W) Move (-1,0);
			if (this.Facing == Direction.E) Move (1,0);
			
			a_duration--;
		}
		
	}
	
	public virtual void ResolveDamage(int damage)
	{
		this.HP -= damage;
		if (this.HP <= 0) this.Alive = false;
	}
	
	// *** OVERRIDE THESE IF MOBS NEED CUSTOM BEHAVIOR *** //
	
	public void Move (float X, float Y)
	{
		this.x += X;
		this.y += Y;
		this.box.x = this.x + 4;
		this.box.y = this.y + 4;
	}
	
	protected Direction getDirectionTowardsPlayer(Mob player, Room room)
	{
		float dx = this.x - player.x;
		float dy = this.y - player.y;
		
		// player is further east/west than north/south
		if (Math.Abs(dx) > Math.Abs(dy))
		{
			if (dx >= 0)
			{
				if (!CollisionOccurred(Direction.W, room)) return Direction.W;
				else if (dy >= 0)
				{
					if (!CollisionOccurred(Direction.S, room)) return Direction.S;
				}
				else
				{
					if (!CollisionOccurred(Direction.N, room)) return Direction.N;
				}
			}
			else
			{
				if (!CollisionOccurred(Direction.E, room)) return Direction.E;
				else if (dy >= 0)
				{
					if (!CollisionOccurred(Direction.S, room)) return Direction.S;
				}
				else
				{
					if (!CollisionOccurred(Direction.N, room)) return Direction.N;
				}
			}
		}
		else // player is further north/south than east/west
		{
			if (dy >= 0)
			{
				if (!CollisionOccurred(Direction.S, room)) return Direction.S;
				else if (dx >= 0)
				{
					if (!CollisionOccurred(Direction.W, room)) return Direction.W;
				}
				else
				{
					if (!CollisionOccurred(Direction.E, room)) return Direction.E;
				}
			}
			else
			{
				if (!CollisionOccurred(Direction.N, room)) return Direction.N;
				else if (dx >= 0)
				{
					if (!CollisionOccurred(Direction.W, room)) return Direction.W;
				}
				else
				{
					if (!CollisionOccurred(Direction.E, room)) return Direction.E;
				}
			}
		}
		return Direction.None;
	}
	
	protected double getDistanceToPlayer(Mob player)
	{
		return Math.Sqrt((this.x - player.x) * (this.x - player.x) + (this.y - player.y) * (this.y - player.y));
	}
	
	protected bool CollisionOccurred(Direction d, Room room)
	{
		Rect collisionRect = this.box;
		bool collision = false;
	
		if (d == Direction.N) collisionRect.y = collisionRect.y + moveSpeed;
		if (d == Direction.S) collisionRect.y = collisionRect.y - moveSpeed;
		if (d == Direction.W) collisionRect.x = collisionRect.x - moveSpeed;
		if (d == Direction.E) collisionRect.x = collisionRect.x + moveSpeed;
		
		// hit wall
		foreach(CollisionBox cbox in room.collisionBoxList)
		{
			if (collisionRect.CheckIntersect(cbox.box))
			{
				collision = true;
				break;
			}
		}
		
		// hit wall segment blocking passageway
		// we don't care if it's active or not because we don't want mobs to try to transition
		foreach(CollisionBox cbox in room.passageObjectBoxList)
		{
			if (collisionRect.CheckIntersect(cbox.box))
			{
				collision = true;
				break;
			}
		}
		
		return collision;
	}
}