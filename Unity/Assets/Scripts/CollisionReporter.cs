using UnityEngine;
using System.Collections;

public class CollisionReporter : MonoBehaviour {

	// Use this for initialization
	void Collision(object collision){
		CollisionInstance ci = collision as CollisionInstance;
		Debug.Log(string.Format("collision '{0}' on {1} with {2}",ci.action, transform.name, ci.sender.transform.name));
	}
}
