using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameUIContainer : FContainer
{
	public GameUIContainer (Dungeon dungeon)
	{
		AddChild(dungeon.minimap);
	}
}
