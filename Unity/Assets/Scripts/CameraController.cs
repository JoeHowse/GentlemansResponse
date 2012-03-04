using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	public float xDistancePanning = 0f;
	public float xDistanceSticking = 1.5f;
	public float xDistanceSpawning = 2.3f;
	
	float playerLastX;
	bool stuck;
	string unstuckMessage;
	void Start() {
		Transform player = GameManager.Instance.playerController.transform;
		playerLastX = player.position.x;
	}
	
	void LateUpdate() {
		Transform player = GameManager.Instance.playerController.transform;
		
		if(transform.position.x - player.transform.position.x >= xDistanceSticking) {
			// The player is in the camera's left-hand sticking margin.
			// Force the player back in bounds.
			player.transform.position = new Vector3(transform.position.x - xDistanceSticking, player.transform.position.y, player.transform.position.z);
		}
		
		if(stuck) {
			if(EnemyController.NumInstances > 0) {
				if(player.transform.position.x - transform.position.x >= xDistanceSticking) {
					// The player is in the camera's right-hand sticking margin.
					// Force the player back in bounds.
					player.transform.position = new Vector3(transform.position.x + xDistanceSticking, player.transform.position.y, player.transform.position.z);
				}
				return;
			} else {
				// All the enemies here are dead.
				stuck = false;
				if(unstuckMessage != ""){
					GameManager.Instance.proclaimer.Proclaim(unstuckMessage, 3f);
					unstuckMessage = "";
				}
			}
		}
		
		if(player.position.x - transform.position.x >= xDistancePanning) {
			// The player is in the camera's right-hand panning margin.
			// Pan the camera in step with the player.
			float diff = (player.position.x - transform.position.x) - xDistancePanning;
			if(diff > Time.deltaTime * 2f){
				diff = Time.deltaTime * 2f;	
			}
			transform.position += Vector3.right *diff;
		}
		
		foreach(StickingPointController stickingPointController in StickingPointController.Instances) {
			if(stickingPointController.transform.position.x - transform.position.x <= 0f) {
				// The camera has panned to a sticking point.
				stuck = true;
				unstuckMessage = stickingPointController.proclamation;
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