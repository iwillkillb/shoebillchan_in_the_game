using UnityEngine;
using System.Collections;

public class EnemySwitch : MonoBehaviour {

	public bool switchAfterCollision = true;	//If collide with player, switch set like this.
	[System.NonSerialized]public bool eSwitch = false;


	void OnTriggerEnter2D (Collider2D col) {
		if (col.tag == "Player Body") {
			eSwitch = switchAfterCollision;
			StartCoroutine (EnemiesActivate());
		}
	}

	IEnumerator EnemiesActivate () {	//Activate Enemys(Child Objects).
		for (int a = 0; a < transform.childCount; a++) {
			if (transform.GetChild (a) != null) {
				transform.GetChild (a).gameObject.SetActive (eSwitch);
			}
			
			yield return null;
		}
	}
}
