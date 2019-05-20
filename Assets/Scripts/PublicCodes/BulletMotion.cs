using UnityEngine;
using System.Collections;

public class BulletMotion : MonoBehaviour {

	Transform spriteObj;
	float rotZ;

	//Setting in Inspector Tab
	float lifeTime = 0;
	public float lifeTimeSetting = 5f;
	public float speed = 10f;
	public float spriteRotationValue = -5f;

	//Setting in Shooter's Prefab
	[System.NonSerialized]public float damage = 0;	//Take shooter's attackPower -> Damage to Player
	[System.NonSerialized]public Vector3 initScale;	//Take shooter's transform.scale. -> sprite and direction Flip.

	void Start () {
		spriteObj = transform.FindChild ("Sprite");
		transform.localScale = initScale;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (lifeTime >= lifeTimeSetting)
			Destroy (gameObject);
		else
			lifeTime += Time.deltaTime;
		transform.Translate (Vector3.right * speed * Time.deltaTime);
		
		if (rotZ >= 360 || rotZ <= -360)
			rotZ = 0;
		else
			rotZ += spriteRotationValue;
		spriteObj.Rotate (new Vector3 (0, 0, rotZ));
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Untagged" || col.tag == "Player Body") {
			Destroy (gameObject);
		}
	}
}
