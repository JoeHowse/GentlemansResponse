using UnityEngine;
using System.Collections;

public class BallroomMusic : MonoBehaviour {
	public AudioClip ballroom1, ballroom2;
	public AudioSource music;
	public KingController king;
	
	bool started = false;
	// Update is called once per frame
	void Start(){
		music = audio;
	}
	
	void Update () {
		if(!started && Camera.main.transform.position.x > transform.position.x){
			StartCoroutine(SwitchMusic());
			started = true;
		}
	}
	
	IEnumerator SwitchMusic(){
		float startTime = Time.time;
		while(Time.time - startTime < 2f){
			float u = (Time.time - startTime)/2f;
			audio.volume = 1-u;
			yield return null;
		}
		audio.loop = false;
		audio.clip = ballroom1;
		audio.Play();
		startTime = Time.time;
		while(Time.time - startTime < 2f){
			float u = (Time.time - startTime)/2f;
			audio.volume = u;
			yield return null;
		}
		audio.volume = 1;
		while(audio.isPlaying){
			yield return 1;	
		}
		audio.clip = ballroom2;
		audio.Play();
		audio.loop = true;
		king.enabled = true;
	}
}
