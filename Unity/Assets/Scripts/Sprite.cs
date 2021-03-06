using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Sprite : MonoBehaviour {
	public Frame curFrame;
	public string lastPlayedAnim;
	public List<FrameAnimation> frameAnimations;
	public Renderer spriteRenderer;
	private bool playing, facingRight;
	public string startFrame = "idle";
	public bool IsPlaying{
		get { return playing; }	
	}
	public bool FacingRight{
		get { return facingRight;}	
	}
	public void Start(){
		Play(startFrame);	
	}
	public void Update(){
		foreach(CollisionBox cb in curFrame.hitBoxes){
			GameManager.Instance.collisionManager.Add(this, cb);
		}
		Vector3 pos = transform.position;
		pos.z = pos.y;
		transform.position = pos;
	}
	
	public void SetDirection(bool right){
		if(right){
			spriteRenderer.transform.localRotation = Quaternion.LookRotation(-Vector3.up,-Vector3.forward);
		}
		else{
			spriteRenderer.transform.localRotation = Quaternion.LookRotation(-Vector3.up,Vector3.forward);
		}
		facingRight = right;
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
