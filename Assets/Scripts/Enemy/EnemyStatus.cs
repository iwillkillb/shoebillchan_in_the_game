using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour {

	SpriteRenderer sr;

	[System.NonSerialized]public bool die = false;
	//Ability.
	public bool deleteAfterDie = true;
	public int score = 10;
	public float hp_Original = 100;	//read
	[System.NonSerialized]public float hp;

	// Use this for initialization
	void Start () {
		sr = GetComponentInChildren<SpriteRenderer> ();
		//Status backup.
		hp = hp_Original;
	}

	// Update is called once per frame
	void FixedUpdate () {

		//HP and DIE
		if (hp <= 0) {
			hp = 0;
			if (!die) {
				die = true;
				if(deleteAfterDie)
					StartCoroutine (Die ());
			}
		} else if (hp > hp_Original)
			hp = hp_Original;
	}

	IEnumerator Die () {
		Color color = sr.color;
		transform.FindChild ("Animator").GetComponent<SoundMaker> ().SoundOn (2);
		while (color.a > 0) {
			color.a -= 0.05f;
			sr.color = color;
			yield return new WaitForSeconds(0.05f);
		}
		Destroy (gameObject);
	}
}
