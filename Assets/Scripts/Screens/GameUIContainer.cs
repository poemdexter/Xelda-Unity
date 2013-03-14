using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameUIContainer : FContainer
{
	public GameUIContainer ()
	{
		FSprite minimapNode = new FSprite("minimap_visited.png");
		minimapNode.x = -Futile.screen.halfWidth + 32;
		minimapNode.y = Futile.screen.halfHeight - 16;
		AddChild(minimapNode);
	}
}
