using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawningPointController : MonoBehaviour {
	static List<SpawningPointController> instances = new List<SpawningPointController>();
	public static List<SpawningPointController> Instances { get { return instances; } }
	
	public Transform enemyPrefab;
	public string spawnName;
	public string proclimation;
	
	public Transform Spawn() {
		Transform t = Instantiate(enemyPrefab, transform.position, Quaternion.identity) as Transform;
		t.GetComponent<Health>().name = spawnName;
		if(proclimation != ""){
			GameManager.Instance.proclaimer.Proclaim(proclimation, 3);	
		}
		return t;
	}
	
	void Awake() {
		instances.Add(this);
	}
	
	void OnDestroy() {
		instances.Remove(this);
	}
}