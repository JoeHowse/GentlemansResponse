using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public int amount;
	public int maxAmount;
	public string name;
	public Texture2D portrait;
	
	public bool TakeDamage(int amount){
		this.amount -= amount;
		if(this.amount <= 0){
			this.amount = 0;
			return true;
		}
		return false;
	}
}
