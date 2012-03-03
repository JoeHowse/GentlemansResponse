using UnityEngine;
using System.Collections;

[System.Serializable]
public class CollisionBox{
	public Rect rect;
	public bool receiveCollision;
	public bool passCollision;
	public string passCollisionName;
}
