using UnityEngine;
using System.Collections;

public class EnemyShooting : MonoBehaviour {

	Transform shootingPoint;

	public GameObject bulletPrefab;
	GameObject targetObj;
	public float detectDistance = 5f;

	public float attackPower = 30f;
	float delayAttack;
	public float delayAttackOrigin = 1.5f;

	Vector3 shootingPosition;

	// Use this for initialization
	void Awake () {
		shootingPoint = transform.FindChild ("Shooting Point");
		targetObj = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (delayAttack < delayAttackOrigin)
			delayAttack += Time.deltaTime;
		
		Shot ();
	}

	void Shot () {
		if(Vector2.Distance(transform.position, targetObj.transform.position) < detectDistance){	//Player is comming...

			if (delayAttack >= delayAttackOrigin) {	//FIRE!!!
				if (shootingPoint != null)
					shootingPosition = shootingPoint.position;
				else
					shootingPosition = transform.position;

				transform.FindChild ("Animator").GetComponent<SoundMaker> ().SoundOn (3);
				GameObject bulletObj = Instantiate (bulletPrefab, shootingPosition, transform.rotation) as GameObject;
				bulletObj.GetComponent<BulletMotion> ().damage = attackPower;
				bulletObj.GetComponent<BulletMotion> ().initScale = transform.localScale * 0.5f;

				delayAttack = 0;
			}
		}
	}
}
