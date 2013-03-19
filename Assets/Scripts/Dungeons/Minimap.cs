using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public class Minimap : FContainer
{
	private string visitedPNG = "minimap_visited";
	private string unvisitedPNG = "minimap_unvisited";
	
	private float centerX;
	private float centerY;
	
	private float nodeWidth;
	private float nodeHeight;
	
	private MinimapNode playerNode;
	
	private List<MinimapNode> nodeList;
	
	private class MinimapNode : FSprite
	{	
		public Vector2 coordinates;
		
		public MinimapNode(string name, Vector2 coords, float x, float y) : base (name + ".png")
		{
			this.x = x;
			this.y = y;
			this.coordinates = coords;
		}
		
		public MinimapNode(string name, float x, float y) : base (name + ".png")
		{
			this.x = x;
			this.y = y;
		}
	}
	
	public Minimap (Dungeon dungeon)
	{
		FSprite visitedNode = new FSprite(visitedPNG + ".png");
		nodeWidth = visitedNode.width;
		nodeHeight = visitedNode.height;
		
		nodeList = new List<MinimapNode>();
		
		// position this in top left of screen
		this.x = -Futile.screen.halfWidth + nodeWidth;
		this.y = Futile.screen.halfHeight - nodeHeight;
		
		centerX = (dungeon.maxWidth / 2) * nodeWidth;
		centerY = -(dungeon.maxHeight / 2) * nodeHeight;
		
		AddMapNodes(dungeon.RoomList);
	}
	
	public void AddMapNodes(List<Room> roomList)
	{	
		foreach(Room r in roomList)
		{
			string nodeName = (r.Visited) ? visitedPNG : unvisitedPNG;
			Vector2 pos = CalculateMinimapNodePosition(r);
			MinimapNode node = new MinimapNode(nodeName, r.MinimapRoomCoordinates, pos.x, pos.y);
			nodeList.Add(node);
			AddChild(node);
		}
		
		//Add initial player node
		playerNode = new MinimapNode("minimap_marker", centerX, centerY);
		AddChildAtIndex(playerNode,99);
	}
	
	public void UpdateMinimap(Dungeon dungeon)
	{
		dungeon.CurrentRoom.Visited = true;
		MinimapNode currentNode = nodeList.First(x => x.coordinates == dungeon.CurrentRoom.MinimapRoomCoordinates);
		RemoveChild(currentNode);
		Vector2 pos = CalculateMinimapNodePosition(dungeon.CurrentRoom);
		AddChild(new MinimapNode(visitedPNG, dungeon.CurrentRoom.MinimapRoomCoordinates,pos.x, pos.y));
		this.RemoveChild(playerNode);
		AddChildAtIndex(playerNode,99);
	}
	
	public Vector2 CalculateMinimapNodePosition(Room room)
	{	
		return new Vector2(
			centerX + (nodeWidth  * room.MinimapRoomCoordinates.x), 
			centerY + (nodeHeight * room.MinimapRoomCoordinates.y)
		);
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
