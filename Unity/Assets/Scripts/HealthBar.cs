using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	
	public GUITexture backGround;
	public GUIText name;
	public GUITexture redPart;
	public GUITexture portrait;
	
	public Health target;
	
	int fullRedWidth;
	
	void Start(){
		fullRedWidth = (int)redPart.pixelInset.width;
	}
	// Update is called once per frame
	void Update () {
		if(!target){
			backGround.enabled = false;
			name.enabled = false;
			redPart.enabled = false;
			portrait.enabled = false;
			return;
		}
		else{
			backGround.enabled = true;
			name.enabled = true;
			redPart.enabled = true;
			portrait.enabled = true;
		}
		name.text = target.name;
		portrait.texture = target.portrait;
		float percent = (float)target.amount / target.maxAmount;
		Rect inset = redPart.pixelInset;
		inset.width = (int)(fullRedWidth* percent);
		redPart.pixelInset = inset;
	}
}