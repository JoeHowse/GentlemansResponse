using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Frame{
	public Rect texCoords;
	public Vector2 anchor;
	public List<CollisionBox> hitBoxes = new List<CollisionBox>();
}
