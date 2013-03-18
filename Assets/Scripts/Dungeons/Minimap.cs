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
	
	private MinimapNode playerNode;
	
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
		}
		
		//Add initial player node
		playerNode = new MinimapNode("minimap_marker", centerX, centerY);
		AddChild(playerNode);
	}
	
	public void ChangePlayerIconPosition(Direction dir)
	{
		switch(dir)
		{
		case (Direction.N):
			playerNode.y += 12;
			break;
		case (Direction.S):
			playerNode.y -= 12;
			break;
		case (Direction.W):
			playerNode.x -= 24;
			break;
		case (Direction.E):
			playerNode.x += 24;
			break;
		}
	}
}
