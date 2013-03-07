using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class CollisionBox : ObjectBox
{
	public bool active;
}

public class ObjectBox
{
	public Rect box;
	public string name;
}

public class AttackBox
{
	public Rect box;
	public int Damage;
}

public enum Direction
{
	N,
	S,
	W,
	E,
	None
}

public class Room : FContainer
{
	private int _tileSize;
	private int _roomWidth;
	private int _roomHeight;
	public string roomName;
	public Vector2 DebugRoomPosition;
	
	public List<CollisionBox> collisionBoxList = new List<CollisionBox>();
	public List<CollisionBox> passageBoxList = new List<CollisionBox>();
	public List<CollisionBox> passageObjectBoxList = new List<CollisionBox>();
	public List<ObjectBox> enemySpawnBoxList = new List<ObjectBox>();
	public ObjectBox playerSpawnBox;
	public List<Mob> mobList = new List<Mob>();
	public List<AttackBox> attackBoxList = new List<AttackBox>();
	
	public int connected_N = -1;
	public int connected_S = -1;
	public int connected_W = -1;
	public int connected_E = -1;
	
	private FSprite _floorSprite;
	
	public Room (String roomFile, Direction linkedRoomDirection, int linkedRoomListIndex) : this(roomFile)
	{
		ConnectToParentRoom(linkedRoomDirection, linkedRoomListIndex);
	}
	
	public Room (String roomFile)
	{
		TextAsset dataAsset = (TextAsset) Resources.Load (roomFile, typeof(TextAsset));
		
		if(!dataAsset) Debug.Log("missing room txt file.");
		
		Dictionary<string,object> hash = dataAsset.text.dictionaryFromJson();
		
		// Room Metadata
		_roomWidth = int.Parse(hash["width"].ToString());
		_roomHeight = int.Parse(hash["height"].ToString());
		_tileSize = int.Parse(hash["tilewidth"].ToString());
		
		//Debug.Log(_roomWidth +"||"+ _roomHeight +"||"+ _tileSize);
		
		//List<object> tilesetsList = (List<object>)hash["tilesets"];
		//Dictionary<string,object> tileset = (Dictionary<string,object>)tilesetsList[0];
		
		//string elementPath = tileset["image"].ToString();
		//string [] pathSplit = elementPath.Split(new Char [] {'/'});
		//string _tilesetElementName = pathSplit[pathSplit.Length-1];
		
		string[] pathSplit = roomFile.Split(new Char[] {'/'});
		roomName = pathSplit[pathSplit.Length -1];
		
		List<object> layersList = (List<object>)hash["layers"];
		
		for (int i=0; i < layersList.Count; i++)
		{
			Dictionary<string,object> layerHash = (Dictionary<string,object>)layersList[i];
			//int layerWidth = int.Parse (layerHash["width"].ToString());
			//int layerHeight = int.Parse (layerHash["height"].ToString());
			//int xOffset = int.Parse (layerHash["x"].ToString());
			//int yOffset = int.Parse (layerHash["y"].ToString());
			
			if (layerHash["name"].ToString().Equals("Objects"))
			{
				// Load object data if it exists...
				List<object> objectList = (List<object>)layerHash["objects"];
				
				for (int j=0; j < objectList.Count; j++)
				{
					Dictionary<string,object> objHash = (Dictionary<string,object>)objectList[j];
					
					if (objHash["type"].ToString().ToUpper().Equals("COLLISION"))
					{
						CollisionBox _cbox = new CollisionBox();
						_cbox.name = objHash["name"].ToString();
						_cbox.box.x = int.Parse(objHash["x"].ToString()) - (GetRoomWidth() / 2);
						_cbox.box.y = -(int.Parse(objHash["y"].ToString()) - (GetRoomHeight() / 2));
						_cbox.box.width = int.Parse(objHash["width"].ToString());
						_cbox.box.height = int.Parse(objHash["height"].ToString());
						_cbox.box.y = _cbox.box.y - _cbox.box.height;
						collisionBoxList.Add(_cbox);
					}
					
					if (objHash["type"].ToString().ToUpper().Equals("PASSAGE"))
					{
						CollisionBox _cbox = new CollisionBox();
						_cbox.name = objHash["name"].ToString();
						_cbox.box.x = int.Parse(objHash["x"].ToString()) - (GetRoomWidth() / 2);
						_cbox.box.y = -(int.Parse(objHash["y"].ToString()) - (GetRoomHeight() / 2));
						_cbox.box.width = (int.Parse(objHash["width"].ToString()) == 0) ? 1 : int.Parse(objHash["width"].ToString());
						_cbox.box.height = (int.Parse(objHash["height"].ToString()) == 0) ? 1 : int.Parse(objHash["height"].ToString());
						_cbox.box.y = _cbox.box.y - _cbox.box.height;
						passageBoxList.Add(_cbox);
					}
					
					if (objHash["type"].ToString().ToUpper().Equals("PASSAGE_OBJECT"))
					{
						CollisionBox _cbox = new CollisionBox();
						_cbox.name = objHash["name"].ToString();
						_cbox.box.x = int.Parse(objHash["x"].ToString()) - (GetRoomWidth() / 2);
						_cbox.box.y = -(int.Parse(objHash["y"].ToString()) - (GetRoomHeight() / 2));
						_cbox.box.width = int.Parse(objHash["width"].ToString());
						_cbox.box.height = int.Parse(objHash["height"].ToString());
						_cbox.box.y = _cbox.box.y - _cbox.box.height;
						_cbox.active = true;
						passageObjectBoxList.Add(_cbox);
					}
					
					if (objHash["type"].ToString().ToUpper().Equals("ENEMY_SPAWN"))
					{
						ObjectBox obox = new ObjectBox();
						obox.name = objHash["name"].ToString();
						obox.box.x = int.Parse(objHash["x"].ToString()) - (GetRoomWidth() / 2);
						obox.box.y = -(int.Parse(objHash["y"].ToString()) - (GetRoomHeight() / 2));
						obox.box.width = (int.Parse(objHash["width"].ToString()) == 0) ? 1 : int.Parse(objHash["width"].ToString());
						obox.box.height = (int.Parse(objHash["height"].ToString()) == 0) ? 1 : int.Parse(objHash["height"].ToString());
						obox.box.y = obox.box.y - obox.box.height;
						enemySpawnBoxList.Add(obox);
					}
					
					if (objHash["type"].ToString().ToUpper().Equals("PLAYER_SPAWN"))
					{
						ObjectBox spawn = new ObjectBox();
						spawn.name = objHash["name"].ToString();
						spawn.box.x = int.Parse(objHash["x"].ToString()) - (GetRoomWidth() / 2);
						spawn.box.y = -(int.Parse(objHash["y"].ToString()) - (GetRoomHeight() / 2));
						spawn.box.width = (int.Parse(objHash["width"].ToString()) == 0) ? 1 : int.Parse(objHash["width"].ToString());
						spawn.box.height = (int.Parse(objHash["height"].ToString()) == 0) ? 1 : int.Parse(objHash["height"].ToString());
						spawn.box.y = spawn.box.y - spawn.box.height;
						playerSpawnBox = spawn;
					}
				}
			}
		}
		
		// non json related initiotion
		_floorSprite = new FSprite(roomName + ".png");
		AddChild(_floorSprite);
		
		// add mobs
		foreach(ObjectBox spawner in enemySpawnBoxList)
		{
			int mx = (int)spawner.box.x;
			int my = (int)spawner.box.y;
			Skeleton skeleton = new Skeleton(mx, my);
			mobList.Add(skeleton);
			AddChild(skeleton);
		}
	}
	
	override public void HandleAddedToStage()
	{
		Futile.instance.SignalUpdate += HandleUpdate;
		base.HandleAddedToStage();
	}
	
	override public void HandleRemovedFromStage()
	{
		Futile.instance.SignalUpdate -= HandleUpdate;
		base.HandleRemovedFromStage();
	}
	
	void HandleUpdate() 
	{
		// check if mobs are getting hit with collision boxes of player attacks
		CheckMobCollisionWithPlayerAttacks();
		
		// make sure mobs are still alive else remove them.
		CheckForDeadMobs();
	}
	
	private void CheckMobCollisionWithPlayerAttacks()
	{
		foreach(Mob mob in mobList)
		{
			foreach(AttackBox abox in attackBoxList)
			{
				if (abox.box.CheckIntersect(mob.box)) mob.TakeDamage(abox.Damage);
			}
		}
	}
	
	private void CheckForDeadMobs()
	{
		for(int x = mobList.Count - 1; x >= 0; x--)
		{
			if (!mobList[x].Alive)
			{
				RemoveChild(mobList[x]);
				mobList.Remove(mobList[x]);
			}
		}
	}
	
	private void RemoveWallForPassage(String direction)
	{
		int index = passageObjectBoxList.FindIndex(x => x.name==direction);
		passageObjectBoxList[index].active = false;
	}
	
	public void ConnectToParentRoom(Direction parentRoomDirection, int parentRoomIndex)
	{
		switch (parentRoomDirection)
		{
		case Direction.N:
			connected_S = parentRoomIndex;
			RemoveWallForPassage("SOUTH");
			break;
		case Direction.S:
			connected_N = parentRoomIndex;
			RemoveWallForPassage("NORTH");
			break;
		case Direction.W:
			connected_E = parentRoomIndex;
			RemoveWallForPassage("EAST");
			break;
		case Direction.E:
			connected_W = parentRoomIndex;
			RemoveWallForPassage("WEST");
			break;
		}
	}
	
	public void ConnectThisToNewRoom(Direction linkedRoomDirection, int linkedRoomListIndex)
	{	
		switch (linkedRoomDirection)
		{
		case Direction.N:
			connected_N = linkedRoomListIndex;
			RemoveWallForPassage("NORTH");
			break;
		case Direction.S:
			connected_S = linkedRoomListIndex;
			RemoveWallForPassage("SOUTH");
			break;
		case Direction.W:
			connected_W = linkedRoomListIndex;
			RemoveWallForPassage("WEST");
			break;
		case Direction.E:
			connected_E = linkedRoomListIndex;
			RemoveWallForPassage("EAST");
			break;
		}
	}
	
	public void AddPassageWalls()
	{
		foreach(CollisionBox cb in passageObjectBoxList)
		{
			if (cb.active)
			{
				// draw a wall!
				FSprite wall;
				if (cb.name == "NORTH" || cb.name == "SOUTH") 
					wall = new FSprite("wall_segment_NS.png");
				else 
					wall = new FSprite("wall_segment_EW.png");
				
				wall.x = cb.box.x;
				wall.y = cb.box.y;
				wall.anchorX = 0;
				wall.anchorY = 0;
				AddChild(wall);
			}
		}
	}
		
	public int GetRoomHeight()
	{
		return _roomHeight * _tileSize;
	}
	
	public int GetRoomWidth()
	{
		return _roomWidth * _tileSize;
	}
	
	public int GetPossibleConnectionCount()
	{
		int amt = 0;
		amt += (connected_N != -1) ? 0 : 1;
		amt += (connected_S != -1) ? 0 : 1;
		amt += (connected_W != -1) ? 0 : 1;
		amt += (connected_E != -1) ? 0 : 1;
		return amt;
	}
	
	public List<Direction> GetPossibleConnectionDirections()
	{
		List<Direction> dList = new List<Direction>();
		if (connected_N == -1) dList.Add(Direction.N);
		if (connected_S == -1) dList.Add(Direction.S);
		if (connected_W == -1) dList.Add(Direction.W);
		if (connected_E == -1) dList.Add(Direction.E);
		return dList;
	}
	
	public List<Direction> GetConnectedDirections()
	{
		List<Direction> dList = new List<Direction>();
		if (connected_N != -1) dList.Add(Direction.N);
		if (connected_S != -1) dList.Add(Direction.S);
		if (connected_W != -1) dList.Add(Direction.W);
		if (connected_E != -1) dList.Add(Direction.E);
		return dList;
	}
	
	public Direction GetRandomDirectionForConnection()
	{
		int r = XeldaGame.rand.Next(GetPossibleConnectionCount());
		return GetPossibleConnectionDirections()[r];
	}
}