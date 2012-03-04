using UnityEngine;
using System.Collections;

public class Proclamation : MonoBehaviour {
	public float duration = 3f;
	IEnumerator Start () {
		float startTime = Time.realtimeSinceStartup;
		Time.timeScale = 0;
		while(Time.realtimeSinceStartup - startTime < duration){
			yield return null;	
		}
		Time.timeScale = 1;
		Destroy(gameObject);
	}
}
