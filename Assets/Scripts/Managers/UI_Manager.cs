using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class UI_Manager
{
	public static FContainer GameUIContainer;
	private static FLabel playerGold = new FLabel("gamefont", "");
	
	public static FContainer getGameUIContainer(GamePage game)
	{
		if (GameUIContainer == null)
		{
			GameUIContainer = new FContainer();
			
			// add minimap
			GameUIContainer.AddChild(game._dungeon.minimap);
			
			// add gold count
			playerGold.text = "player gold: " + game.player.GoldCount;
			playerGold.x += 50;
			playerGold.y += Futile.screen.halfHeight - 20;
			GameUIContainer.AddChild(playerGold);
			
			return GameUIContainer;
		}
		else return GameUIContainer;
	}
	
	public static void changePlayerGoldUI(int goldCount)
	{
		playerGold.text = "player gold: " + goldCount;
	}
}