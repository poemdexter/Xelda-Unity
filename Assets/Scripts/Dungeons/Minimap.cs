using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Minimap : FContainer
{
	private string visitedPNG = "minimap_visited.png";
	//private string unvisitedPNG = "minimap_unvisited.png";
	
	private float centerX;
	private float centerY;
	
	private class MinimapNode : FSprite
	{
		public MinimapNode(string name, float x, float y) : base (name + ".png")
		{
			this.x = x;
			this.y = y;
		}
	}
	
	public Minimap (Dungeon dungeon)
	{
		// position this in top left of screen
		this.x = -Futile.screen.halfWidth + 24;
		this.y = Futile.screen.halfHeight - 16;
		
		AddMapNodes(dungeon.RoomList, dungeon.maxWidth, dungeon.maxHeight);
	}
	
	public void AddMapNodes(List<Room> roomList, int d_width, int d_height)
	{
		FSprite visitedNode = new FSprite(visitedPNG);
		//FSprite unvisitedNode = new FSprite(unvisitedPNG);
		
		centerX = (d_width / 2) * 24;
		centerY = -(d_height / 2) * 16;
			
		foreach(Room r in roomList)
		{
			AddChild(new MinimapNode("minimap_visited", centerX + (visitedNode.width * r.MinimapRoomCoordinates.x), centerY + (visitedNode.height * r.MinimapRoomCoordinates.y)));
			//AddChild(new MinimapNode("minimap_visited", centerX, centerY));
		}
		
		
		
		//AddChild(currentNode);
		//minimapNode2.x = minimapNode2.width*9;
		//minimapNode2.y = -(minimapNode2.height*5);
		//AddChild(minimapNode2);
	}
}
