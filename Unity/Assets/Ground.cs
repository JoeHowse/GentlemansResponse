using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ground : MonoBehaviour {
	public List<Rect> rects;
	
	public bool IsOverGround(Vector2 position) {
		Vector2 origin = new Vector2(transform.position.x, transform.position.z);
		foreach(Rect rect in rects) {
			Vector2 halfRectDimensions = new Vector2(rect.width * 0.5f, rect.height * 0.5f);
			if(rect.Contains(origin + position + halfRectDimensions)) {
				return true;
			}
		}
		return false;
	}
	
	public bool IsOverGround(Vector3 position) {
		return IsOverGround(new Vector2(position.x, position.z));
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos() {
		// Draw the edges of the rects.
		Vector3 origin = transform.position;
		foreach(Rect rect in rects) {
			Vector3 halfRectDimensions = new Vector3(rect.width * 0.5f, 0f, rect.height * 0.5f);
			Vector3 ul = origin + new Vector3(rect.xMin, 0f, rect.yMax) - halfRectDimensions;
			Vector3 ur = origin + new Vector3(rect.xMax, 0f, rect.yMax) - halfRectDimensions;
			Vector3 lr = origin + new Vector3(rect.xMax, 0f, rect.yMin) - halfRectDimensions;
			Vector3 ll = origin + new Vector3(rect.xMin, 0f, rect.yMin) - halfRectDimensions;
			Gizmos.DrawLine(ul, ur);
			Gizmos.DrawLine(ur, lr);
			Gizmos.DrawLine(lr, ll);
			Gizmos.DrawLine(ll, ul);
		}
	}
#endif
}