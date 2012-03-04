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
	
	void Start(){
		sprite = GetComponent<Sprite>();	
	}
	
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
		
			transform.position += (Vector3)moveDir * Time.deltaTime;
			if(Mathf.Approximately(0, moveDir.sqrMagnitude) ){
				if(!GetComponent<Sprite>().IsPlaying)
					GetComponent<Sprite>().Play("idle");
			}
			else{
				GetComponent<Sprite>().Play("walk");
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
			queuedpunch = false;
			string anim = comboNames[comboCount];
			GetComponent<Sprite>().Stop();
			GetComponent<Sprite>().Play(anim);
			StartCoroutine(NoControl());
			if(comboCount > 2){
				StartCoroutine(SlideHit());
			}
				
			if(++comboCount > comboNames.Count-1){
				comboCount = 0;	
			}
			else{	
				comboTimer = 0.6f;
			}
		}
	}
	
	IEnumerator NoControl(float time = 0.4f){
		noControl = true;
		yield return new WaitForSeconds(time);
		noControl = false;
	}
	IEnumerator SlideHit(){
		yield return new WaitForSeconds(0.2f);
		float speed = GetComponent<Sprite>().FacingRight? 2f: -2f;
		float startTime = Time.time;
		while(Time.time - startTime < 0.3f){
			float u = 1 - (Time.time - startTime) / 0.3f;	
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
				StartCoroutine(NoControl(0.5f));
				StartCoroutine(KnockBack(0.3f));
				sprite.Play("stun");
			}
			else{
				StartCoroutine(KnockDownAndDie());
			}
		}
	}
}
