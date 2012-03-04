using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Sprite))]
public class IntroSequenceController : MonoBehaviour { 
	public float movementSpeed = 0.1f;
	
	IEnumerator Start() {
		yield return null;
		
		PlayerController playerController = GameManager.Instance.playerController;
		Sprite playerSprite = playerController.gameObject.GetComponent<Sprite>();
		Sprite kingSprite = GetComponent<Sprite>();
		Proclaimer proclaimer = GameManager.Instance.proclaimer;
		float startTime;
		
		playerController.noControl = true;
		
		kingSprite.SetDirection(true);
		
		proclaimer.Proclaim("My eigth\ncousin,\nthe King,\nis passing!", 3f);
		
		startTime = Time.time;
		while(Time.time - startTime < 4f){
			kingSprite.Play("nwalk");
			playerSprite.Play("idle");
			transform.position += new Vector3(Time.deltaTime * movementSpeed, 0f, 0f);
			yield return null;
		}
		
		proclaimer.Proclaim("Your\nMajesty!", 3f);
		
		startTime = Time.time;
		while(Time.time - startTime < 7f){
			kingSprite.Play("nwalk");
			playerSprite.Play("idle");
			transform.position += new Vector3(Time.deltaTime * movementSpeed, 0f, 0f);
			yield return null;
		}
		
		proclaimer.Proclaim("Hello?\nYour\nMajesty?", 3f);
		
		startTime = Time.time;
		while(Time.time - startTime < 7f){
			kingSprite.Play("nwalk");
			playerSprite.Play("idle");
			transform.position += new Vector3(Time.deltaTime * movementSpeed, 0f, 0f);
			yield return null;
		}
		
		proclaimer.Proclaim("Tyrant! He\nslighted me!", 3f);
		
		startTime = Time.time;
		while(Time.time - startTime < 4f){
			kingSprite.Play("nwalk");
			playerSprite.Play("idle");
			transform.position += new Vector3(Time.deltaTime * movementSpeed, 0f, 0f);
			yield return null;
		}
		
		proclaimer.Proclaim("I shall\npay him his\njust dues!", 3f);
		
		playerController.noControl = false;
		Destroy(gameObject);
	}
}
