using UnityEngine;
using System.Collections;

public class SelfDelete : MonoBehaviour {

	public float lifeTime = 1f;
	float lifeDelay = 0;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (lifeDelay < lifeTime) {
			lifeDelay += Time.deltaTime;
		} else {
			Destroy (gameObject);
		}
	}
}
