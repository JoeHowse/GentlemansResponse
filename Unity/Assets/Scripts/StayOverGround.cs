using UnityEngine;
using System.Collections;

public class StayOverGround : MonoBehaviour {
	Vector3 lastPosition;
	
	void Start () {
		lastPosition = transform.position;
	}
	
	void LateUpdate () {
		GroundController groundController = GameManager.Instance.groundController;
		if(groundController && !groundController.IsOverGround(transform.position)) {
			transform.position = lastPosition;
		} else {
			lastPosition = transform.position;
		}
	}
}