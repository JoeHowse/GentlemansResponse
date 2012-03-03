using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawningPointController : MonoBehaviour {
	static List<SpawningPointController> instances = new List<SpawningPointController>();
	public static List<SpawningPointController> Instances { get { return instances; } }
	
	public Transform enemyPrefab;
	
	public Transform Spawn() {
		return (Transform)Instantiate(enemyPrefab, transform.position, Quaternion.identity);
	}
	
	void Awake() {
		instances.Add(this);
	}
	
	void OnDestroy() {
		instances.Remove(this);
	}
}