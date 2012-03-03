using UnityEngine;
using System.Collections;

public class StayOverGround : MonoBehaviour {
	public Ground ground;
	
	Vector3 lastPosition;
	
	void Start () {
		lastPosition = transform.position;
	}
	
	void LateUpdate () {
		if(ground && !ground.IsOverGround(transform.position)) {
			transform.position = lastPosition;
		} else {
			lastPosition = transform.position;
		}
	}
}