using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {

	public string sceneName;
	PlayerStatus ps;

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player Body") {
			ps = col.transform.parent.GetComponentInParent<PlayerStatus> ();
			if (ps.die == false) {
				PlayerPrefs.SetInt ("score", ps.score);
				PlayerPrefs.SetFloat ("hp", ps.hp);
				PlayerPrefs.SetFloat ("mp", ps.mp);

				ChangeScene ();
			}
		}
	}

	public void ChangeScene () {
		SceneManager.LoadScene (sceneName);
	}
	public void ResetScene () {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
