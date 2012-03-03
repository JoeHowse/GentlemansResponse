using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundController : MonoBehaviour {
	public List<Rect> rects;
	
	public bool IsOverGround(Vector2 position) {
		Vector2 origin = new Vector2(transform.position.x, transform.position.y);
		foreach(Rect rect in rects) {
			Vector2 halfRectDimensions = new Vector2(rect.width * 0.5f, rect.height * 0.5f);
			if(rect.Contains(origin + position + halfRectDimensions)) {
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
			Vector3 halfRectDimensions = new Vector3(rect.width * 0.5f, rect.height * 0.5f, 0f);
			Vector3 ul = origin + new Vector3(rect.xMin, rect.yMax, 0f) - halfRectDimensions;
			Vector3 ur = origin + new Vector3(rect.xMax, rect.yMax, 0f) - halfRectDimensions;
			Vector3 lr = origin + new Vector3(rect.xMax, rect.yMin, 0f) - halfRectDimensions;
			Vector3 ll = origin + new Vector3(rect.xMin, rect.yMin, 0f) - halfRectDimensions;
			Gizmos.DrawLine(ul, ur);
			Gizmos.DrawLine(ur, lr);
			Gizmos.DrawLine(lr, ll);
			Gizmos.DrawLine(ll, ul);
		}
	}
#endif
}