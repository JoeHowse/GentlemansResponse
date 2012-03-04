using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
	
	public AudioSource audioManager;
	public AudioClip hit;
	public AudioClip attack;
	public AudioClip explode;
	public AudioClip blocked;
	public AudioClip electric;
	public AudioClip landing;
	public AudioClip run;
	
	Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

	// Use this for initialization
	void Start () {
		loadResources();
	}
	
	public void playSFX (string clip) {
		audioManager.clip = audioClips[clip];
		audioManager.loop = false;
		audioManager.Play ();
		
	}
	
	void loadResources()
	{
		audioClips.Add("hit", hit);
		audioClips.Add("attack", attack);
		audioClips.Add("explode", explode);
		audioClips.Add("blocked", blocked);
		audioClips.Add("electric", electric);
		audioClips.Add("landing", landing);
		audioClips.Add("run", run);
	}
}
