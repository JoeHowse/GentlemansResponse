using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	
	public GUITexture backGround;
	public GUIText name;
	public GUITexture redPart;
	
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
			return;
		}
		else{
			backGround.enabled = true;
			name.enabled = true;
			redPart.enabled = true;
		}
		name.text = target.name;
		float percent = (float)target.amount / target.maxAmount;
		Rect inset = redPart.pixelInset;
		inset.width = (int)(fullRedWidth* percent);
		redPart.pixelInset = inset;
	}
}
