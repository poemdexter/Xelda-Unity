using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct collisionBox
{
	public Rect box;
	public string name;
	public string tip;
}

public class Map
{
	private int _tileSize;
	private int _mapWidth;
	private int _mapHeight;
	
	private collisionBox _cbox;
	public List<collisionBox> collisionBoxList = new List<collisionBox>();
	
	public Map ()
	{
		TextAsset dataAsset = (TextAsset) Resources.Load ("Maps/xelda_map", typeof(TextAsset));
		if(!dataAsset) 
			Debug.Log("missing map txt file.");
		
		Dictionary<string,object> hash = dataAsset.text.dictionaryFromJson();
		
		// Map Metadata
		_mapWidth = int.Parse(hash["width"].ToString());
		_mapHeight = int.Parse(hash["height"].ToString());
		_tileSize = int.Parse(hash["tilewidth"].ToString());
		
		//Debug.Log(_mapWidth +"||"+ _mapHeight +"||"+ _tileSize);
		
		List<object> tilesetsList = (List<object>)hash["tilesets"];
		Dictionary<string,object> tileset = (Dictionary<string,object>)tilesetsList[0];
		
		string elementPath = tileset["image"].ToString();
		string [] pathSplit = elementPath.Split(new Char [] {'/'});
		string _tilesetElementName = pathSplit[pathSplit.Length-1];
		
		List<object> layersList = (List<object>)hash["layers"];
		
		for (int i=0; i < layersList.Count; i++)
		{
			Dictionary<string,object> layerHash = (Dictionary<string,object>)layersList[i];
			int layerWidth = int.Parse (layerHash["width"].ToString());
			int layerHeight = int.Parse (layerHash["height"].ToString());
			int xOffset = int.Parse (layerHash["x"].ToString());
			int yOffset = int.Parse (layerHash["y"].ToString());
			
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
						_cbox.box.x = int.Parse(objHash["x"].ToString());
						_cbox.box.y = int.Parse(objHash["y"].ToString());
						_cbox.box.width = int.Parse(objHash["width"].ToString());
						_cbox.box.height = int.Parse(objHash["height"].ToString());
						_cbox.box.y = GetMapHeight() - _cbox.box.y - _cbox.box.height;
						//Debug.Log("h: " + _mapHeight);
						Debug.Log("got collision box");
						collisionBoxList.Add(_cbox);
					}
				}
			}
		}
		//Debug.Log(_tilesetElementName);
	}
	
		
	int GetMapHeight()
	{
		return _mapHeight * _tileSize;
	}
}