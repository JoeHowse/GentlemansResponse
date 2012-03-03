using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CollisionManager : MonoBehaviour {
	public List<CollisionBoxInfo> activeCollisionBoxes = new List<CollisionBoxInfo>();
	
	public void Add(Transform owner, CollisionBox box){
		activeCollisionBoxes.Add(new CollisionBoxInfo{owner = owner, box = box});
	}
	
	public void LateUpdate(){
		var collisions =(
						from x	in activeCollisionBoxes
						from y	in activeCollisionBoxes
						where 	x.owner != y.owner
								&& x.box.passCollision 
								&& y.box.receiveCollision
								&& Overlapping(x,y)
						select new {action = x.box.passCollisionName, receiver = y.owner, sender = x.owner}
						).Distinct();
		foreach(var collision in collisions){
			collision.receiver.SendMessage("Collision", 
				new CollisionInstance{
					sender = collision.sender, 
					action = collision.action
				}
			);
		}
		activeCollisionBoxes.Clear();
	}
	
	public static bool Overlapping(CollisionBoxInfo a, CollisionBoxInfo b){
		if (a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin){
			return true;
		}
		return false;
	}
}

public class CollisionBoxInfo{
	public Transform owner;
	public CollisionBox box;
}

public class CollisionInstance{
	public Transform sender;
	public string action;
}

