using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class collisionBox
{
	public Rect box;
	public string name;
	public string tip;
	public bool active;
}

public enum Direction
{
	N,
	S,
	W,
	E,
	None
}

public class Map : FContainer
{
	private int _tileSize;
	private int _mapWidth;
	private int _mapHeight;
	public string mapName;
	public Vector2 DebugMapPosition;
	
	private collisionBox _cbox;
	public List<collisionBox> collisionBoxList = new List<collisionBox>();
	public List<collisionBox> passageBoxList = new List<collisionBox>();
	public List<collisionBox> passageObjectBoxList = new List<collisionBox>();
	
	public int connected_N = -1;
	public int connected_S = -1;
	public int connected_W = -1;
	public int connected_E = -1;
	
	private FSprite _floorSprite;
	
	public Map (String mapFile, Direction linkedMapDirection, int linkedMapListIndex) : this(mapFile)
	{
		ConnectToParentMap(linkedMapDirection, linkedMapListIndex);
	}
	
	public Map (String mapFile)
	{
		TextAsset dataAsset = (TextAsset) Resources.Load (mapFile, typeof(TextAsset));
		
		if(!dataAsset) Debug.Log("missing map txt file.");
		
		Dictionary<string,object> hash = dataAsset.text.dictionaryFromJson();
		
		// Map Metadata
		_mapWidth = int.Parse(hash["width"].ToString());
		_mapHeight = int.Parse(hash["height"].ToString());
		_tileSize = int.Parse(hash["tilewidth"].ToString());
		
		//Debug.Log(_mapWidth +"||"+ _mapHeight +"||"+ _tileSize);
		
		//List<object> tilesetsList = (List<object>)hash["tilesets"];
		//Dictionary<string,object> tileset = (Dictionary<string,object>)tilesetsList[0];
		
		//string elementPath = tileset["image"].ToString();
		//string [] pathSplit = elementPath.Split(new Char [] {'/'});
		//string _tilesetElementName = pathSplit[pathSplit.Length-1];
		
		string[] pathSplit = mapFile.Split(new Char[] {'/'});
		mapName = pathSplit[pathSplit.Length -1];
		
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
						_cbox = new collisionBox();
						_cbox.name = objHash["name"].ToString();
						_cbox.box.x = int.Parse(objHash["x"].ToString()) - (GetMapWidth() / 2);
						_cbox.box.y = -(int.Parse(objHash["y"].ToString()) - (GetMapHeight() / 2));
						_cbox.box.width = int.Parse(objHash["width"].ToString());
						_cbox.box.height = int.Parse(objHash["height"].ToString());
						_cbox.box.y = _cbox.box.y - _cbox.box.height;
						collisionBoxList.Add(_cbox);
					}
					
					if (objHash["type"].ToString().ToUpper().Equals("PASSAGE"))
					{
						_cbox = new collisionBox();
						_cbox.name = objHash["name"].ToString();
						_cbox.box.x = int.Parse(objHash["x"].ToString()) - (GetMapWidth() / 2);
						_cbox.box.y = -(int.Parse(objHash["y"].ToString()) - (GetMapHeight() / 2));
						_cbox.box.width = (int.Parse(objHash["width"].ToString()) == 0) ? 1 : int.Parse(objHash["width"].ToString());
						_cbox.box.height = (int.Parse(objHash["height"].ToString()) == 0) ? 1 : int.Parse(objHash["height"].ToString());
						_cbox.box.y = _cbox.box.y - _cbox.box.height;
						passageBoxList.Add(_cbox);
					}
					
					if (objHash["type"].ToString().ToUpper().Equals("PASSAGE_OBJECT"))
					{
						_cbox = new collisionBox();
						_cbox.name = objHash["name"].ToString();
						_cbox.box.x = int.Parse(objHash["x"].ToString()) - (GetMapWidth() / 2);
						_cbox.box.y = -(int.Parse(objHash["y"].ToString()) - (GetMapHeight() / 2));
						_cbox.box.width = int.Parse(objHash["width"].ToString());
						_cbox.box.height = int.Parse(objHash["height"].ToString());
						_cbox.box.y = _cbox.box.y - _cbox.box.height;
						_cbox.active = true;
						passageObjectBoxList.Add(_cbox);
					}
				}
			}
		}
		
		// non json related initiotion
		_floorSprite = new FSprite(mapName + ".png");
		AddChild(_floorSprite);
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
	
	void HandleUpdate() {}
	
	private void RemoveWallForPassage(String direction)
	{
		int index = passageObjectBoxList.FindIndex(x => x.name==direction);
		passageObjectBoxList[index].active = false;
	}
	
	public void ConnectToParentMap(Direction parentMapDirection, int parentMapIndex)
	{
		switch (parentMapDirection)
		{
		case Direction.N:
			connected_S = parentMapIndex;
			RemoveWallForPassage("SOUTH");
			break;
		case Direction.S:
			connected_N = parentMapIndex;
			RemoveWallForPassage("NORTH");
			break;
		case Direction.W:
			connected_E = parentMapIndex;
			RemoveWallForPassage("EAST");
			break;
		case Direction.E:
			connected_W = parentMapIndex;
			RemoveWallForPassage("WEST");
			break;
		}
	}
	
	public void ConnectThisToNewMap(Direction linkedMapDirection, int linkedMapListIndex)
	{	
		switch (linkedMapDirection)
		{
		case Direction.N:
			connected_N = linkedMapListIndex;
			RemoveWallForPassage("NORTH");
			break;
		case Direction.S:
			connected_S = linkedMapListIndex;
			RemoveWallForPassage("SOUTH");
			break;
		case Direction.W:
			connected_W = linkedMapListIndex;
			RemoveWallForPassage("WEST");
			break;
		case Direction.E:
			connected_E = linkedMapListIndex;
			RemoveWallForPassage("EAST");
			break;
		}
	}
	
	public void AddPassageWalls()
	{
		foreach(collisionBox cb in passageObjectBoxList)
		{
			if (cb.active)
			{
				// draw a wall!
				FSprite wall = new FSprite("wall_segment.png");
				wall.x = cb.box.x;
				wall.y = cb.box.y;
				wall.anchorX = 0;
				wall.anchorY = 0;
				AddChild(wall);
			}
		}
	}
		
	public int GetMapHeight()
	{
		return _mapHeight * _tileSize;
	}
	
	public int GetMapWidth()
	{
		return _mapWidth * _tileSize;
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
		System.Random rand = new System.Random(System.DateTime.Now.Millisecond);
		int r = rand.Next(GetPossibleConnectionCount());
		return GetPossibleConnectionDirections()[r];
	}
}