using UnityEngine;
using System.Collections;

public class Proclaimer : MonoBehaviour {
	public Transform proclamationPrefab;
	
	public void Proclaim(string text, float duration) {
		Transform proclamation = Instantiate(proclamationPrefab) as Transform;
		foreach(GUIText guiText in proclamation.GetComponentsInChildren<GUIText>()) {
			if(guiText.name.Equals("Content")) {
				guiText.text = text;
			}
		}
	}
}