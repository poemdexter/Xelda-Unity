using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public class Minimap : FContainer
{
	private string visitedPNG = "minimap_visited";
	private string unvisitedPNG = "minimap_unvisited";
	
	private float originX;
	private float originY;
	
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
		this.y = Futile.screen.halfHeight;
		
		// set origin to bottom, left corner
		originX = 0;
		originY = -(dungeon.maxHeight * nodeHeight) - (nodeHeight / 2);
		
		AddMapNodes(dungeon.RoomList, dungeon.CurrentRoom.MinimapRoomCoordinates, dungeon.CurrentRoom);
	}
	
	public void AddMapNodes(List<Room> roomList, Vector2 currentRoomCoords, Room currentRoom)
	{	
		// debug draw entire possible rooms in dungeon
		for(int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				AddChild(
					new MinimapNode(
						"minimap_visited", new Vector2(0,0), originX + (nodeWidth * i), originY + (nodeHeight * j)
					)
				);
			}
		}
		
		foreach(Room r in roomList)
		{
			string nodeName = (r.Visited) ? visitedPNG : unvisitedPNG;
			Vector2 pos = CalculateMinimapNodePosition(r);
			MinimapNode node = new MinimapNode(nodeName, r.MinimapRoomCoordinates, pos.x, pos.y);
			nodeList.Add(node);
			AddChild(node);
		}
		
		//Add initial player node
		Vector2 player_pos = CalculateMinimapNodePosition(currentRoom);
		playerNode = new MinimapNode("minimap_marker", player_pos.x, player_pos.y);
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
			originX + (nodeWidth  * room.MinimapRoomCoordinates.x), 
			originY + (nodeHeight * room.MinimapRoomCoordinates.y)
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
