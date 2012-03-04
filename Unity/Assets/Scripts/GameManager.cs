using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	static GameManager instance;
	public static GameManager Instance { get { return instance; } }
	
	public CameraController cameraController;
	public CollisionManager collisionManager;
	public GroundController groundController;
	public PlayerController playerController;
	public HealthBar enemyHealthBar;
	public Proclaimer proclaimer;
	
	void Awake() {
		instance = this;
	}
}
