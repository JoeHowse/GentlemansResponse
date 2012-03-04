using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
	
	public AudioSource audioManager;
	public AudioClip hit;
	public AudioClip attack;
	public AudioClip explode;
	
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
	}
}
