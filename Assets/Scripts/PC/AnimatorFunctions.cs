using UnityEngine;
using System.Collections;

public class AnimatorFunctions : MonoBehaviour {
	
	[System.NonSerialized]public bool atkInputEnabled = false;
	[System.NonSerialized]public bool atkInputNow = false;
	Animator anim;

	//Animations control this.
	public float attackPower = 10f;

	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator> ();
	}

	public void EnableAttackInput(){			// If this activated, You can attack.
		atkInputEnabled = true;
	}
	public void SetNextAttack(string name){		// Go to the next State.
		if (atkInputNow == true) {
			atkInputNow = false;
			anim.Play (name);
		}
	}
}
