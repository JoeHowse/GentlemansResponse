using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		Vector2 moveDir = Vector2.zero;
		moveDir += Input.GetAxis("Horizontal") * Vector2.right;
		moveDir += Input.GetAxis("Vertical") * Vector2.up;
		
		transform.position += (Vector3)moveDir * Time.deltaTime;
		
		if(Mathf.Approximately(0, moveDir.sqrMagnitude)){
			GetComponent<Sprite>().Play("idle");
		}
		else{
			GetComponent<Sprite>().Play("walk");
		}
	}
}
