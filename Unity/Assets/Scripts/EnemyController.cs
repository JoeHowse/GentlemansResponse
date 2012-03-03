using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {
	public static uint maxNumFighting = 3;
	
	static uint numInstances = 0;
	public static uint NumInstances { get { return numInstances; } }
	
	static uint numFighting = 0;
	
	public float strikingDistance = 0.1f;
	public float circlingDistance = 1.5f;
	public float movementSpeed = 0.1f;
	public bool circleClockwise = false;
	
	bool fighting = false;
	
	void Awake() {
		numInstances++;
	}
	
	void Update() {
		if(!fighting && numFighting < maxNumFighting) {
			// Acquire a fighting token.
			fighting = true;
			numFighting++;
		}
		
		Transform player = GameManager.Instance.playerController.transform;
		Vector3 vectorToOpponent = player.position - transform.position;
		float distanceToOpponent = vectorToOpponent.magnitude;
		if(fighting) {
			if(distanceToOpponent <= strikingDistance) {
				// Attempt to strike the opponent.
				// TODO
			} else {
				// Attempt to approach the opponent.
				AttemptMove(player.position);
			}
		} else {
			// Attempt to circle the opponent.
			float theta = Mathf.Atan2(vectorToOpponent.y, vectorToOpponent.x);
			float deltaTheta = movementSpeed / circlingDistance;
			if(circleClockwise) {
				deltaTheta *= -1;
			}
			theta += deltaTheta;
			if(!AttemptMove(player.position + circlingDistance * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0f))) {
				// An obstacle lies in the way of the current circling direction.
				// Reverse the circling direction.
				circleClockwise = !circleClockwise;
			}
		}
		
		// Face the opponent.
		transform.localRotation = Quaternion.LookRotation
		(
			vectorToOpponent.x < 0 ?
				Vector3.left
			:
				Vector3.right
		);
	}
	
	void OnDestroy() {
		if(fighting) {
			// Release a fighting token.
			numFighting--;
		}
		
		numInstances--;
	}
	
	bool AttemptMove(Vector3 destination) {
		GroundController groundController = GameManager.Instance.groundController;
		float movementDistance = Time.deltaTime * movementSpeed;
		Vector3 vectorToDestination = destination - transform.position;
		if(vectorToDestination.magnitude <= movementSpeed) {
			if(groundController.IsOverGround(destination)) {
				// Move directly to the destination.
				transform.position = destination;
				// Report success.
				return true;
			}
			// The destination is within movement range but impassible.
			// Report failure.
			return false;
		}
		// Attempt a waypoint on the direct route.
		Vector3 waypoint = transform.position + movementSpeed * vectorToDestination;
		if(!groundController.IsOverGround(waypoint)) {
			// Attempt a waypoint that closes the x distance.
			waypoint = transform.position + movementDistance * new Vector3(vectorToDestination.x, 0f, 0f);
			if(!groundController.IsOverGround(waypoint)) {
				// Attempt a waypoint that closes the y distance.
				waypoint = transform.position + movementDistance * new Vector3(0f, vectorToDestination.y, 0f);
				if(!groundController.IsOverGround(waypoint)) {
					// No suitable waypoint was found.
					// Report failure.
					return false;
				}
			}
		}
		// Move to the waypoint.
		transform.position = waypoint;
		// Report success.
		return true;
	}
}