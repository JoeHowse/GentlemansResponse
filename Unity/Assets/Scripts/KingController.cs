using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KingController : MonoBehaviour {
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
	public EnemyState currentState = EnemyState.Idling;
	bool allowedFight = true;
	bool noControl = false;
	public string entranceText;
	public Transform walkTarget;
	public GUITexture screenFlash;
	public AudioManager audioManager;
	
	void Awake() {
		numInstances++;
		sprite = GetComponent<Sprite>();
		circlingDistance += Random.Range(-0.4f, 0.4f);
	}
	void Start(){
		StartCoroutine(KingIntroduction());	
	}
	
	IEnumerator KingIntroduction(){
		noControl = true;
		sprite.SetDirection(false);
		while(Vector2.Distance((Vector2)transform.position, (Vector2)walkTarget.position) > 0.2f){
			sprite.Play("nwalk");
			AttemptMove(walkTarget.position);
			yield return null;
		}
		
		float sTime = Time.time;
		while(Time.time - sTime < 3f){
			sprite.Play("laugh");
			yield return 1;
		}
		sprite.Play("transform");
		yield return new WaitForSeconds(1f);
		GameManager.Instance.proclaimer.Proclaim(entranceText, 3f);
		noControl = false;
	}
	
	void Update() {
		if(noControl){
			return;	
		}
		if(currentState != EnemyState.Fighting 
			&& numFighting < maxNumFighting 
			&& allowedFight) {
			// Acquire a fighting token.
			currentState = EnemyState.Fighting;
			numFighting++;
		}
		else{
//			Debug.Log(numFighting);	
		}
		switch(currentState){
			case EnemyState.Fighting:
				Fighting();
				break;
			case EnemyState.Circling:
				Circling();
				break;
			case EnemyState.Idling:
				Idle();
				break;
		}
		
		Transform player = GameManager.Instance.playerController.transform;
		Vector3 vectorToOpponent = (player.position - transform.position);
		// Face the opponent.
		if(vectorToOpponent.x < 0){
			sprite.SetDirection(false);
		}
		else{
			sprite.SetDirection(true);
		}
		
	}

	void Fighting (){
		Transform player = GameManager.Instance.playerController.transform;
		Vector3 vectorToOpponent = (player.position - transform.position);
		Vector3 flattened = -vectorToOpponent;
		flattened.y = 0;
		flattened.Normalize();
		Vector3 target = player.position + flattened * strikingDistance;
		float distance = Vector3.Distance(transform.position, target);
		if(distance < 0.1f){
			Attack();	
		}
		else{
			sprite.Play("walk");
			AttemptMove(target);	
		}
	}
	
	void Attack(){
		StartCoroutine(AttackThenBackOff());
	}
	
	
	float lastStateChange = 0;
	void Circling (){
		Transform player = GameManager.Instance.playerController.transform;
		Vector3 vectorToOpponent = (player.position - transform.position);
		float theta = Mathf.Atan2(-vectorToOpponent.y, -vectorToOpponent.x);
		float deltaTheta = movementSpeed*Time.deltaTime;
		if(circleClockwise) {
			deltaTheta *= -1;
		}
		theta += deltaTheta;
		Vector3 angleVec = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0f);
		sprite.Play("walk");
		if(!AttemptMove(player.position + circlingDistance *  angleVec)) {
			// An obstacle lies in the way of the current circling direction.
			// Reverse the circling direction.
			circleClockwise = !circleClockwise;
		}
		if(Time.time - lastStateChange > 2f){
			currentState = EnemyState.Idling;
			lastStateChange = Time.time;
		}
	}
	
	void Idle (){
		sprite.Play("idle");
		if(Time.time - lastStateChange > 0.7f){
			currentState = EnemyState.Circling;
			lastStateChange = Time.time;
		}
	}
	
	void OnDestroy() {
		if(currentState == EnemyState.Fighting) {
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
	
	IEnumerator KnockDownAndDie(){
		sprite.Play("kneel");
		yield return new WaitForSeconds(0.8f);
		GameManager.Instance.proclaimer.Proclaim("Good\nDay\n\nSir!", 3);
		StartCoroutine(Explosions());
		yield return new WaitForSeconds(4f);
		float startTime = Time.time;
		while(Time.time - startTime < 6){
			float u = (Time.time - startTime)/6f;
			screenFlash.color = new Color(1,1,1,u);
			yield return null;
		}
		Application.LoadLevel("WinGame");
		//Instantiate(gibs,transform.position,Quaternion.identity);
		//Destroy(gameObject);
	}
	
	IEnumerator Explosions(){
		float startTime = Time.time;
		while(Time.time - startTime < 12){
			sprite.Play("kneel");
			float t =Time.time - startTime;
			if(t > 1.5f){
				Vector3 rand = transform.position + Vector3.right * Random.Range(-0.3f,0.3f) + Vector3.up* Random.Range(0f, 1f);
				Instantiate(gibs, rand, Quaternion.identity);
				if( t > 3f){
					rand = Camera.main.transform.position + Vector3.forward + Vector3.right * Random.Range(-2f,2f) + Vector3.up* Random.Range(-1.75f, 1.75f);
					Instantiate(gibs, rand, Quaternion.identity);
				}
			}
			yield return null;
		}
	}
	
	IEnumerator AttackThenBackOff(){
		allowedFight = false;
		currentState = EnemyState.Circling;
		numFighting--;
		yield return new WaitForSeconds(0.2f);
		sprite.Play("attack");
		StartCoroutine(NoControl(1f));
			
			// add sound effect
			audioManager.playSFX("attack");
		yield return new WaitForSeconds(1f);
		allowedFight = true;
	}
	
	float lastHit = 0;
	public void Collision(object c){
		if(Time.time - lastHit < 0.25f)
			return;
		if(GetComponent<Health>().amount <= 0)
			return;
		lastHit = Time.time;
		CollisionInstance ci = c as CollisionInstance;
		bool dead = false;
		if(ci.action == "jab"){
			GameManager.Instance.enemyHealthBar.target = GetComponent<Health>();
			Cancel();
			
			// add sound effect
			audioManager.playSFX("hit");
			StartCoroutine(NoControl(0.2f));
			StartCoroutine(KnockBack(-0.02f));
			sprite.Play("stun");
			dead = GetComponent<Health>().TakeDamage(1);
		}
		if(ci.action == "hook"){
			GameManager.Instance.enemyHealthBar.target = GetComponent<Health>();
			Cancel();
			
			// add sound effect
			audioManager.playSFX("hit");
			StartCoroutine(NoControl(0.2f));
			StartCoroutine(KnockBack(0.5f));
			sprite.Play("stun");
			dead = GetComponent<Health>().TakeDamage(2);
		}
		if(ci.action == "cane"){
			GameManager.Instance.enemyHealthBar.target = GetComponent<Health>();
			Cancel();
			
			// add sound effect
			audioManager.playSFX("hit");
			StartCoroutine(NoControl(0.2f));
			StartCoroutine(KnockBack(1.5f));
			dead = GetComponent<Health>().TakeDamage(3);
		}
		
		if(dead){
			Cancel();
			StartCoroutine(KnockDownAndDie());
		}
	}
	public void Cancel(){
		StopAllCoroutines();
		allowedFight = true;
	}
}