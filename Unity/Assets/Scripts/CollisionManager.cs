using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CollisionManager : MonoBehaviour {
	public List<CollisionBoxInfo> activeCollisionBoxes = new List<CollisionBoxInfo>();
	
	public void Add(Sprite owner, CollisionBox box){
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
	}
	
	public static bool Overlapping(CollisionBoxInfo a, CollisionBoxInfo b){
		if (a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin){
			return true;
		}
		return false;
	}
	
	public void OnDrawGizmos(){
		foreach(CollisionBoxInfo cbi in activeCollisionBoxes){
			if(cbi.box.passCollision){
				Gizmos.color = Color.red;	
			}
			Gizmos.DrawWireCube((Vector3)cbi.rect.center, new Vector3(cbi.rect.width,cbi.rect.height, 0.1f));
			Gizmos.color = Color.white;
		}
		activeCollisionBoxes.Clear();
	}
}

public class CollisionBoxInfo{
	public Sprite owner;
	public CollisionBox box;
	
	public Rect rect {
		get{
			return new Rect(xMin,yMin, xMax - xMin, yMax - yMin);				
		}
	}
				
	public float xMin{
		get {
			return box.rect.xMin/64f + owner.transform.position.x - 0.5f;	
		}
	}
	public float xMax{
		get {
			return box.rect.xMax/64f + owner.transform.position.x- 0.5f;	
		}
	}
	public float yMin{
		get {
			return -box.rect.yMax/64f + owner.transform.position.y + 1- owner.spriteRenderer.transform.localPosition.y;	
		}
	}
	public float yMax{
		get {
			return -box.rect.yMin/64f + owner.transform.position.y + 1- owner.spriteRenderer.transform.localPosition.y;	
		}
	}
}

public class CollisionInstance{
	public Sprite sender;
	public string action;
}

