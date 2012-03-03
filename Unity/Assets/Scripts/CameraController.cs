using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	public float xWhereCameraStartsPanning;
	public List<Transform> stickingPoints;
	
	float playerLastX;
	bool stuck;
	
	void Start() {
		Transform player = GameManager.Instance.playerController.transform;
		playerLastX = player.position.x;
	}
	
	void Update() {
		if(stuck) {
			if(EnemyController.NumInstances > 0) {
				return;
			} else {
				// All the enemies here are dead.
				// Remove the current sticking points.
				stickingPoints = stickingPoints.FindAll(IsFutureStickingPoint);
			}
		}
		
		Transform player = GameManager.Instance.playerController.transform;
		
		if(player.position.x - transform.position.x >= xWhereCameraStartsPanning) {
			// The player is in the camera's right-hand panning margin.
			// Pan the camera in step with the player.
			transform.position += new Vector3(player.position.x - playerLastX, 0f, 0f);
		}
		
		foreach(Transform stickingPoint in stickingPoints) {
			if(stickingPoint.position.x - transform.position.x <= 0f) {
				// The camera has panned to a sticking point.
				stuck = true;
				break;
			}
		}
		
		playerLastX = player.position.x;
	}
	
	bool IsFutureStickingPoint(Transform stickingPoint) {
		return stickingPoint.position.x - transform.position.x > 0f;
	}
}