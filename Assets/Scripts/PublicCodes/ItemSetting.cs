using UnityEngine;
using System.Collections;

public class ItemSetting : MonoBehaviour {

	ParticleSystem pStar, pLight;
	SpriteRenderer sr;

	/*
	public int weaponKind = 0;	//0-No Weapon, 1-Knife, 2-Sword
	public string HowWeaponKindSet = "0-Nothing, 1-Knife, 2-Sword";	//Inspector Tab Comment. useless...
	public int weaponDamage = 0;
	*/
	public int score = 10;
	public float hpPlus = 0, spPlus = 0;	//If you take this item, how much restored HP or SP?

	bool isUsed = false;	//Only ONE AFFACT!

	// Use this for initialization
	void Awake () {
		pStar = transform.FindChild ("Effect Star").GetComponent<ParticleSystem> ();
		pLight = transform.FindChild ("Effect Light").GetComponent<ParticleSystem> ();
		sr = GetComponent<SpriteRenderer> ();
	}

	void OnTriggerStay2D(Collider2D col){
		if (col.tag == "Player Body" && isUsed == false) {
			isUsed = true;

			//Item Effect
			col.transform.parent.parent.GetComponentInParent<PlayerStatus>().score += score;
			col.transform.parent.parent.GetComponentInParent<PlayerStatus>().hp += hpPlus;
			col.transform.parent.parent.GetComponentInParent<PlayerStatus>().mp += spPlus;

			/*
			PlayerUpperMotion pum = col.transform.FindChild ("Hip").FindChild ("Body Upper").GetComponent<PlayerUpperMotion> ();
			if(weaponKind > 0)	//if this item is weapon...
				pum.WeaponChange (sr.sprite, weaponKind, weaponDamage);
				*/

			StartCoroutine (PlayerGet ());
		}
	}

	IEnumerator PlayerGet () {	//Used item is deleted.
		
		//Particle Setting
		if(pStar != null)
			pStar.Play ();
		if (pLight != null)
			pLight.Stop ();
		
		Color color = sr.color;
		GetComponent<AudioSource> ().Play ();
		while (color.a > 0) {
			color.a -= 0.1f;
			sr.color = color;
			yield return new WaitForSeconds(0.1f);
		}
		Destroy (gameObject);
	}
}
