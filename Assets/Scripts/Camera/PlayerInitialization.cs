using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerInitialization : MonoBehaviour {

	GameObject player;
	PlayerStatus ps;
	public bool hpSpReset = false;

	//This code sets PlayerPrefs and inits player object.
	//USE ALL STAGE IN MAIN CAMERA.

	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player");

		if (player != null) {
			ps = player.GetComponent<PlayerStatus> ();

			if (ps != null) {
				PlayerPrefs.SetInt ("stage" + SceneManager.GetActiveScene ().name, 1);	//The Name of Scene that Player can jump
				if (PlayerPrefs.GetInt ("scoreReset") == 1) {
					PlayerPrefs.SetInt ("scoreReset", 0);
					PlayerPrefs.SetInt ("score", 0);
				}
				if (hpSpReset) {
					PlayerPrefs.SetFloat ("hp", ps.hp_Original);
					PlayerPrefs.SetFloat ("mp", ps.mp_Original);
				}
			}
		}

	}
}
