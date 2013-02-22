using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DebugMapPage : FContainer
{
	private FSprite _manSprite;
	private FSprite _floorSprite;
	
	//private FNode _cameraTarget = new FNode();
	
	private Dungeon _dungeon;
	
	public DebugMapPage()
	{
		_dungeon = new Dungeon(2);
		
		_floorSprite = new FSprite(_dungeon.CurrentMap.mapName + ".png");
		AddChild(_floorSprite);
		
		int mapSep = 250;
		foreach (Direction dir in _dungeon.CurrentMap.GetConnectedDirections())
		{
			switch(dir)
			{
			case Direction.N:
				FSprite tempSprite1 = new FSprite(_dungeon.MapList[1].mapName + ".png");
				tempSprite1.y += mapSep;
				AddChild (tempSprite1);
				break;
			case Direction.S:
				FSprite tempSprite2 = new FSprite(_dungeon.MapList[1].mapName + ".png");
				tempSprite2.y -= mapSep;
				AddChild (tempSprite2);
				break;
			case Direction.W:
				FSprite tempSprite3 = new FSprite(_dungeon.MapList[1].mapName + ".png");
				tempSprite3.x -= mapSep;
				AddChild (tempSprite3);
				break;
			case Direction.E:
				FSprite tempSprite4 = new FSprite(_dungeon.MapList[1].mapName + ".png");
				tempSprite4.x += mapSep;
				AddChild (tempSprite4);
				break;
			}
		}
		
		// *** debug to find collision boxes
		showCollisionsWithMen();
	}
	
	private void showCollisionsWithMen()
	{
		foreach(collisionBox box in _dungeon.CurrentMap.collisionBoxList)
		{
			FSprite cb = new FSprite("man.png");
			cb.x = box.box.x;
			cb.y = box.box.y;
			cb.anchorX = 0;
			cb.anchorY = 0;
			AddChild(cb);
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
		
	}
}


