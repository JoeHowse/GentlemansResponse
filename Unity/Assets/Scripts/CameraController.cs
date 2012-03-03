using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	public float xDistancePanning;
	public float xDistanceSpawning;
	
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
				stuck = false;
			}
		}
		
		Transform player = GameManager.Instance.playerController.transform;
		
		if(player.position.x - transform.position.x >= xDistancePanning) {
			// The player is in the camera's right-hand panning margin.
			// Pan the camera in step with the player.
			transform.position += new Vector3(player.position.x - playerLastX, 0f, 0f);
		}
		
		foreach(StickingPointController stickingPointController in StickingPointController.Instances) {
			if(stickingPointController.transform.position.x - transform.position.x <= 0f) {
				// The camera has panned to a sticking point.
				stuck = true;
				Destroy(stickingPointController.gameObject);
			}
		}
		
		foreach(SpawningPointController spawningPointController in SpawningPointController.Instances) {
			if(spawningPointController.transform.position.x - transform.position.x <= xDistanceSpawning) {
				spawningPointController.Spawn();
				Destroy(spawningPointController);
			}
		}
		
		playerLastX = player.position.x;
	}
}