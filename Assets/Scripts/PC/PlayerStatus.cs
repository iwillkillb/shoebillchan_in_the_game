using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {

	[System.NonSerialized]public bool die = false;
	[System.NonSerialized]public int score;
	[System.NonSerialized]public float hp_Original = 100;	//read
	[System.NonSerialized]public float hp;
	[System.NonSerialized]public float mp_Original = 100;	//read
	[System.NonSerialized]public float mp;
	[System.NonSerialized]public float mpAdd = 5;

	// Use this for initialization
	void Start () {
		//Status backup.
		score = PlayerPrefs.GetInt("score");
		hp = PlayerPrefs.GetFloat ("hp");
		mp = PlayerPrefs.GetFloat ("mp");

		StartCoroutine (DataSave ());
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!die) {
			//HP and DIE
			if (hp <= 0) {	//If you died...
				if(!die)	//bool die = true
					die = true;
				hp = 0;		//hp is ZERO
				//When you died, your score is reseted and hp/mp sets full.
				PlayerPrefs.SetInt ("scoreReset", 1);
				PlayerPrefs.SetFloat ("hp", hp_Original);
				PlayerPrefs.SetFloat ("mp", mp_Original);

			} else if (hp > hp_Original)	//Over HP -> Full HP
				hp = hp_Original;

			//SP
			if (mp < 0)
				mp = 0;
			else if (mp > mp_Original)
				mp = mp_Original;
			else {
				if (mp < mp_Original)
					mp += Time.deltaTime * mpAdd;
			}
		}
	}

	IEnumerator DataSave () {
		while (!die) {	//Don't save ZERO HP!
			PlayerPrefs.SetInt ("score", score);
			PlayerPrefs.SetFloat ("hp", hp);
			PlayerPrefs.SetFloat ("mp", mp);

			yield return new WaitForSeconds (1);
		}
	}
}
