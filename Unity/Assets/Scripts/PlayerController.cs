using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	
	public int comboCount = 0;
	public bool noControl = false;
	public int health = 15;
	public int maxHealth = 15;
	
	public float comboTimer = 0;
	public List<string> comboNames = new List<string>();
	public bool queuedpunch = false;
	public Sprite sprite;
	bool dashing = false;
	
	public AudioManager audioManager;
	
	void Start(){
		sprite = GetComponent<Sprite>();
		sprite.SetDirection(true);
	}
	
	KeyCode recentDashKey;
	float recentDashKeyTime;
	
	void Update () {
		if(comboTimer <= 0){
			comboCount = 0;	
		}
		else{
			comboTimer -= Time.deltaTime;	
		}
		Vector2 moveDir = Vector2.zero;
		if(!noControl){
			moveDir += Input.GetAxis("Horizontal") * Vector2.right;
			moveDir += Input.GetAxis("Vertical") * Vector2.up;
			
			if(Input.GetKeyDown(recentDashKey) && Time.time - recentDashKeyTime < 0.2f){
					dashing = true;
					Debug.Log("Dashing!");
					
			}
			if(Input.GetKeyDown(KeyCode.D)){
				recentDashKey = KeyCode.D;
				recentDashKeyTime = Time.time;
			}
			if(Input.GetKeyDown(KeyCode.A)){
				recentDashKey = KeyCode.A;	
				recentDashKeyTime = Time.time;
			}
			if(Input.GetKeyDown(KeyCode.RightArrow)){
				recentDashKey = KeyCode.RightArrow;
				recentDashKeyTime = Time.time;
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow)){
				recentDashKey = KeyCode.LeftArrow;	
				recentDashKeyTime = Time.time;
			}
			if(Input.GetKeyUp(recentDashKey)){
				dashing = false;
			}
			if(dashing)
				moveDir *= 2;
			transform.position += (Vector3)moveDir * Time.deltaTime;
			if(Mathf.Approximately(0, moveDir.sqrMagnitude)){
				dashing = false;	
				GetComponent<Sprite>().Play("idle");
			}
			else{
				GetComponent<Sprite>().Play(dashing ? "dash" :"walk");
				if(moveDir.x < 0){
					GetComponent<Sprite>().SetDirection(false);
				}
				else if(moveDir.x > 0){
					GetComponent<Sprite>().SetDirection(true);
				}
			}
			TryPunch();
		}
		else{
			if(Input.GetButtonDown("Fire1")){
				queuedpunch = true;	
			}
		}
		
	}
	
	void TryPunch(){
		if(Input.GetButtonDown("Fire1") || queuedpunch){
			if(dashing){
				comboCount = comboNames.Count -1;
			}
			queuedpunch = false;
			string anim = comboNames[comboCount];
			GetComponent<Sprite>().Stop();
			GetComponent<Sprite>().Play(anim);
			if(dashing)
				StartCoroutine(NoControl(1));
			else
				StartCoroutine(NoControl());
			if(comboCount > 2){
				if(dashing)
					StartCoroutine(SlideHit(4,1));
				else
					StartCoroutine(SlideHit(2));
			}
				
			if(++comboCount > comboNames.Count-1){
				comboCount = 0;	
			}
			else{	
				comboTimer = 0.6f;
			}
			dashing = false;
		}
	}
	
	IEnumerator NoControl(float time = 0.4f){
		noControl = true;
		yield return new WaitForSeconds(time);
		noControl = false;
	}
	IEnumerator SlideHit(float amount, float time = 0.3f){
		yield return new WaitForSeconds(0.2f);
		float speed = GetComponent<Sprite>().FacingRight? amount: -amount;
		float startTime = Time.time;
		while(Time.time - startTime < time){
			float u = 1 - (Time.time - startTime) / time;	
			speed = u*speed;
			transform.position += Vector3.right * speed * Time.deltaTime;
			yield return null;
		}
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
		StartCoroutine(NoControl(10));
		sprite.Play("fall");
		StartCoroutine(KnockBack(3f, 1f));
		
		// add sound effect
		audioManager.playSFX("explode");
		yield return new WaitForSeconds(0.5f);
		transform.position -= Vector3.up * 0.2f;
		yield return new WaitForSeconds(5f);
		Application.LoadLevel("LostGame");
		yield return null;
	}
	
	float lastHit = 0;
	public void Collision(object c){
		if(Time.time - lastHit < 0.2f){
			return;
		}
		lastHit = Time.time;
		CollisionInstance ci = c as CollisionInstance;
		if(ci.action == "attack"){
			bool dead = GetComponent<Health>().TakeDamage(1);
			if(!dead){
			// add sound effect
			audioManager.playSFX("hit");
				StartCoroutine(NoControl(0.5f));
				StartCoroutine(KnockBack(0.3f));
				sprite.Play("stun");
			}
			else{
				StartCoroutine(KnockDownAndDie());
			}
		}
		if(ci.action == "slice"){
			bool dead = GetComponent<Health>().TakeDamage(2);
			if(!dead){
				StartCoroutine(NoControl(0.6f));
				StartCoroutine(KnockBack(1.5f, 0.6f));
				sprite.Play("stun");
			}
			else{
				StartCoroutine(KnockDownAndDie());
			}
		}
	}
}
