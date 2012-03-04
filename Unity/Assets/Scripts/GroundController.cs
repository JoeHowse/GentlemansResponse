using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundController : MonoBehaviour {
	public List<Rect> rects;
	
	public bool IsOverGround(Vector2 position) {
		Vector2 origin = new Vector2(transform.position.x, transform.position.y);
		foreach(Rect rect in rects) {
			
			if(rect.Contains(origin + position)) {
				return true;
			}
		}
		return false;
	}
	
	public bool IsOverGround(Vector3 position) {
		return IsOverGround(new Vector2(position.x, position.y));
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos() {
		// Draw the edges of the rects.
		Vector3 origin = transform.position;
		foreach(Rect rect in rects) {
			Vector3 ul = origin + new Vector3(rect.xMin, rect.yMax, 0f);
			Vector3 ur = origin + new Vector3(rect.xMax, rect.yMax, 0f);
			Vector3 lr = origin + new Vector3(rect.xMax, rect.yMin, 0f);
			Vector3 ll = origin + new Vector3(rect.xMin, rect.yMin, 0f);
			Gizmos.DrawLine(ul, ur);
			Gizmos.DrawLine(ur, lr);
			Gizmos.DrawLine(lr, ll);
			Gizmos.DrawLine(ll, ul);
		}
	}
#endif
}