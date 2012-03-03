using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	
	public int comboCount = 0;
	public bool noControl = false;
	
	public float comboTimer = 0;
	public List<string> comboNames = new List<string>();
	public bool queuedpunch = false;
	
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
	
	IEnumerator NoControl(){
		noControl = true;
		yield return new WaitForSeconds(0.4f);
		noControl = false;
	}
	IEnumerator SlideHit(){
		yield return new WaitForSeconds(0.2f);
		float speed = 2f;
		float startTime = Time.time;
		while(Time.time - startTime < 0.3f){
			float u = 1 - (Time.time - startTime) / 0.3f;	
			speed = u*speed;
			transform.position += Vector3.right * speed * Time.deltaTime;
			yield return null;
		}
	}
}
