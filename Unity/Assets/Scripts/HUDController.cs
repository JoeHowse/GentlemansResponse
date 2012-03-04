using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	public GameObject healthBar;
	public GameObject player;
	public GUITexture healthBarOverlay;
	public GUITexture healthBarBG;
	public GUITexture healthFill;
	public int scale;

	// Use this for initialization
	void Start () {
		healthBarOverlay.transform.position = new Vector3(1.0F, 1.0F, 0.0F);
		healthBarBG.transform.position = new Vector3(1.0F, 1.0F, 0.0F);
		healthFill.transform.position = new Vector3(1.0F, 1.0F, 0.0F);
	}

    void OnGUI()
    {
        //=========================================================================================
        // Health Bar Section
        //=========================================================================================
        // Get the player's health
        int hp = player.GetComponent<PlayerController>().health;
		int maxhp = player.GetComponent<PlayerController>().maxHealth;
		
		// Show the health bar
        healthBar.SetActiveRecursively(true);
		
		int healthWidth = (hp / maxhp) * (int)healthFill.GetScreenRect().width;
		
		healthFill.pixelInset = new Rect (healthFill.GetScreenRect().x * scale, healthFill.GetScreenRect().y * scale, 
										  healthWidth * scale, healthFill.GetScreenRect().height * scale);
    }
}
