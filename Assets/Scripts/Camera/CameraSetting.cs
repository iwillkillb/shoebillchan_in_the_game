using UnityEngine;
using System.Collections;

public class CameraSetting : MonoBehaviour {

	Camera cam;
	public float camSpeed = 5f;
	public bool enableZoomControl = true;
	public float maxCamSize = 7.5f;
	public Vector2 positionMin = new Vector2 (-100f, -100f);
	public Vector2 positionMax = new Vector2 (100f, 100f);
	public bool chaseX = true, chaseY = true;
	GameObject chasedObject;
	Vector3 chasedPosition;

	bool isShaking = false;
	public float shakeTime = 0.5f, shakePower = 1;

	// Use this for initialization
	void Awake () {
		cam = GetComponent<Camera> ();
		chasedObject = GameObject.FindGameObjectWithTag ("Player");
		chasedPosition.z = transform.position.z;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (enableZoomControl) {
			if (cam.orthographic) {
				if (1 < cam.orthographicSize && cam.orthographicSize < maxCamSize)
					cam.orthographicSize -= Input.GetAxisRaw ("Mouse ScrollWheel");
				else if (cam.orthographicSize >= maxCamSize)
					cam.orthographicSize = maxCamSize - 0.1f;
				else
					cam.orthographicSize = 1.1f;
			} else {
				if (chasedPosition.z < -1 && chasedPosition.z > maxCamSize * -2)
					chasedPosition.z += Input.GetAxisRaw ("Mouse ScrollWheel");
				else if (chasedPosition.z <= maxCamSize * -2)
					chasedPosition.z = (maxCamSize * -2) + 0.1f;
				else
					chasedPosition.z = -1.1f;
			}
		}
		if (chasedObject != null) {
			if (chaseX && positionMin.x <= chasedObject.transform.position.x && chasedObject.transform.position.x <= positionMax.x) {
				chasedPosition.x = chasedObject.transform.position.x;
			} else
				chasedPosition.x = transform.position.x;

			if (chaseY && positionMin.y <= chasedObject.transform.position.y && chasedObject.transform.position.y <= positionMax.y) {
				chasedPosition.y = chasedObject.transform.position.y;
			} else
				chasedPosition.y = transform.position.y;

			//Camera Position Setting.
			if (isShaking) {	//CAMERA SHAKE !!
				transform.localPosition += Random.insideUnitSphere * shakePower;
			}
			transform.position = Vector3.Lerp (transform.position, chasedPosition, camSpeed * Time.deltaTime);
		}
	}



	public void CameraShake(){
		StartCoroutine (Shake ());
	}
	IEnumerator Shake () {	//This just controls bool value "isShaking".
		float shakingTime = 0;

		while (shakingTime < shakeTime) {
			if(isShaking == false)
				isShaking = true;
			shakingTime += Time.deltaTime;
			yield return null;
		}
		if(isShaking == true)
			isShaking = false;
	}
}
