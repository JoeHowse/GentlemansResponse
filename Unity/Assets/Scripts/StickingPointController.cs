using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StickingPointController : MonoBehaviour {
	static List<StickingPointController> instances = new List<StickingPointController>();
	public static List<StickingPointController> Instances { get { return instances; } }
	
	void Awake() {
		instances.Add(this);
	}
	
	void OnDestroy() {
		instances.Remove(this);
	}
}