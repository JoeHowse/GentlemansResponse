using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Sprite : MonoBehaviour {
	public Frame curFrame;
	public string lastPlayedAnim;
	public List<FrameAnimation> frameAnimations;
	public Renderer spriteRenderer;
	public CollisionManager collisionManager;
	private bool playing;
	public bool IsPlaying{
		get { return playing; }	
	}
	public void Start(){
		Play("idle");	
	}
	public void Update(){
		foreach(CollisionBox cb in curFrame.hitBoxes){
			collisionManager.Add(transform, cb);
		}	
	}
	public void Play(string anim){
		if(playing){
			if(lastPlayedAnim == anim)
				return;
			StopAllCoroutines();
		}
		StartCoroutine(PlayInternal(anim));
	}
	
	public IEnumerator PlayInternal(string anim){
		playing = true;
		lastPlayedAnim = anim;
		var frameAnimation = (from x in frameAnimations where x.name == anim select x).First();
		float timePerFrame = frameAnimation.time / frameAnimation.frames.Count;
		Material m = spriteRenderer.material;
		m.mainTexture = frameAnimation.texture;
		for(int i = 0; i < frameAnimation.frames.Count; i++){
			Frame frame = frameAnimation.frames[i];
			
			m.mainTextureOffset = new Vector2(frame.texCoords.x / frameAnimation.texture.width,
											frame.texCoords.y / frameAnimation.texture.height);
			m.mainTextureScale = new Vector2(frame.texCoords.width / frameAnimation.texture.width,
											frame.texCoords.height / frameAnimation.texture.height);
			spriteRenderer.material = m;
			curFrame = frame;
			yield return new WaitForSeconds(timePerFrame);
		}
		playing = false;
		yield break;
	}
	
	public void Stop(){
		if(playing){
			StopAllCoroutines();
			lastPlayedAnim = "";
		}
	}
}
