using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {
	public static uint maxNumFighting = 1;
	
	static uint numInstances = 0;
	public static uint NumInstances { get { return numInstances; } }
	
	static uint numFighting = 0;
	
	public float strikingDistance = 0.1f;
	public float circlingDistance = 1.5f;
	public float movementSpeed = 0.1f;
	public bool circleClockwise = false;
	public Sprite sprite;
	public GameObject gibs;
	bool fighting = false;
	bool allowedFight = true;
	bool noControl = false;
	
	void Awake() {
		numInstances++;
		sprite = GetComponent<Sprite>();
	}
	
	void Update() {
		if(!fighting && numFighting < maxNumFighting && allowedFight) {
			// Acquire a fighting token.
			fighting = true;
			numFighting++;
		}
		if(noControl){
			return;	
		}
		Transform player = GameManager.Instance.playerController.transform;
		Vector3 vectorToOpponent = (player.position - transform.position);
	
		float distanceToOpponent = vectorToOpponent.magnitude;
		if(fighting) {
			if(distanceToOpponent <= strikingDistance) {
				// Attempt to strike the opponent.
				
				StartCoroutine(AttackThenBackOff());
			} else {
				// Attempt to approach the opponent.
				sprite.Play("walk");
				AttemptMove(player.position);
			}
		} else {
			// Attempt to circle the opponent.
			float theta = Mathf.Atan2(-vectorToOpponent.y, -vectorToOpponent.x);
			float deltaTheta = movementSpeed*Time.deltaTime;
			if(circleClockwise) {
				deltaTheta *= -1;
			}
			theta += deltaTheta;
			Vector3 angleVec = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0f);
//			Debug.Log(angleVec);
			sprite.Play("walk");
			if(!AttemptMove(player.position + circlingDistance *  angleVec)) {
				// An obstacle lies in the way of the current circling direction.
				// Reverse the circling direction.
				circleClockwise = !circleClockwise;
			}
		}
		
		// Face the opponent.
		if(vectorToOpponent.x < 0){
			sprite.SetDirection(false);
		}
		else{
			sprite.SetDirection(true);
		}
		
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
		Vector3 vectorToDestination = (destination - transform.position);
		if(vectorToDestination.magnitude <= movementDistance) {
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
		Vector3 waypoint = transform.position + movementDistance * vectorToDestination.normalized;
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
	
	IEnumerator NoControl(float time){
		noControl = true;
		yield return new WaitForSeconds(time);
		noControl = false;
	}
	
	IEnumerator KnockBack(float s, float t = 0.3f){
		float speed = GetComponent<Sprite>().FacingRight? -s: s;
		float startTime = Time.time;
		while(Time.time - startTime < t){
			float u = 1 - (Time.time - startTime) / t;	
			speed = u*speed;
			transform.position += Vector3.right * speed * Time.deltaTime;
			yield return null;
		}
	}
	
	IEnumerator KnockDown(){
		StartCoroutine(NoControl(3));
		sprite.Play("fall");
		StartCoroutine(KnockBack(3f, 0.6f));
		yield return new WaitForSeconds(0.5f);
		sprite.Play("down");
		yield return null;
	}
	
	IEnumerator KnockDownAndDie(){
		yield return StartCoroutine(KnockDown());
		yield return new WaitForSeconds(1f);
		Instantiate(gibs,transform.position,Quaternion.identity);
		Destroy(gameObject);
	}
	
	IEnumerator AttackThenBackOff(){
		allowedFight = false;
		yield return new WaitForSeconds(0.3f);
		sprite.Play("attack");
		StartCoroutine(NoControl(1f));
		if(fighting) {
			numFighting--;
			fighting = false;
		}
		yield return new WaitForSeconds(1f);
		allowedFight = true;
	}
	float lastHit = 0;
	public void Collision(object c){
		if(Time.time - lastHit < 0.25f)
			return;
		lastHit = Time.time;
		CollisionInstance ci = c as CollisionInstance;
		bool dead = false;
		if(ci.action == "jab"){
			GameManager.Instance.enemyHealthBar.target = GetComponent<Health>();
			StopAllCoroutines();
			allowedFight = true;
			StartCoroutine(NoControl(0.3f));
			StartCoroutine(KnockBack(-0.02f));
			sprite.Play("stun");
			dead = GetComponent<Health>().TakeDamage(1);
		}
		if(ci.action == "hook"){
			GameManager.Instance.enemyHealthBar.target = GetComponent<Health>();
			StopAllCoroutines();
			allowedFight = true;
			StartCoroutine(NoControl(1f));
			StartCoroutine(KnockBack(0.5f));
			sprite.Play("stun");
			dead = GetComponent<Health>().TakeDamage(2);
		}
		if(ci.action == "cane"){
			GameManager.Instance.enemyHealthBar.target = GetComponent<Health>();
			StopAllCoroutines();
			allowedFight = true;
			StartCoroutine(KnockDown());
			dead = GetComponent<Health>().TakeDamage(3);
		}
		
		if(dead){
			StopAllCoroutines();
			allowedFight = true;
			StartCoroutine(KnockDownAndDie());
		}
	}
}
