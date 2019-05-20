using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CanvasSetting : MonoBehaviour {

	GameObject player;
	PlayerStatus ps;

	Image gameOverImage, gameClearImage;
	Text scoreText;
	RectTransform hpRect, mpRect, rtGameOverImage, rtGameClearImage;

	float hpBarRectWidthInit, mpBarRectWidthInit;
	int score = 0;
	float hp, mp;

	[System.NonSerialized] public bool isMissionCleard = false;
	public string afterClearSceneName;
	public bool jumpSceneAfterClear = true;

	// Use this for initialization
	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player");

		if (player != null) {
			ps = player.GetComponent<PlayerStatus> ();
			scoreText = transform.FindChild ("HUD").FindChild ("Score Value").GetComponent<Text> ();
			hpRect = transform.FindChild ("HUD").FindChild ("HP Bar").GetComponent<RectTransform> ();
			mpRect = transform.FindChild ("HUD").FindChild ("MP Bar").GetComponent<RectTransform> ();

			rtGameOverImage = transform.FindChild ("Game Over Image").GetComponent<RectTransform> ();
			rtGameClearImage = transform.FindChild ("Game Clear Image").GetComponent<RectTransform> ();
			gameOverImage = transform.FindChild ("Game Over Image").GetComponent<Image> ();
			gameClearImage = transform.FindChild ("Game Clear Image").GetComponent<Image> ();



			rtGameOverImage.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0);
			rtGameOverImage.sizeDelta = new Vector3 (2, 1, 0) * (Screen.height * 0.3f);

			rtGameClearImage.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0);
			rtGameClearImage.sizeDelta = new Vector3 (2, 1, 0) * (Screen.height * 0.3f);



			score = ps.score;
			hp = ps.hp / ps.hp_Original;
			mp = ps.mp / ps.mp_Original;

			hpBarRectWidthInit = hpRect.sizeDelta.x;
			mpBarRectWidthInit = mpRect.sizeDelta.x;

			scoreText.text = score.ToString ();
			hpRect.sizeDelta = new Vector2 (hpBarRectWidthInit * hp, hpRect.sizeDelta.y);
			mpRect.sizeDelta = new Vector2 (mpBarRectWidthInit * mp, mpRect.sizeDelta.y);

			StartCoroutine (HudSetting ());
		}
	}

	IEnumerator HudSetting(){
		while (true) {
			if (score != ps.score) {
				score = ps.score;
				scoreText.text = score.ToString ();
			}

			if (hp != ps.hp / ps.hp_Original) {
				hp = ps.hp / ps.hp_Original;
				if (hp > 1)
					hp = 1;
				hpRect.sizeDelta = new Vector2 (hpBarRectWidthInit * hp, hpRect.sizeDelta.y);
			}

			if (mp != ps.mp / ps.mp_Original) {
				mp = ps.mp / ps.mp_Original;
				if (mp > 1)
					mp = 1;
				mpRect.sizeDelta = new Vector2 (mpBarRectWidthInit * mp, mpRect.sizeDelta.y);
			}

			if (ps.die)
				StartCoroutine (GameOverDisplay ());

			if (isMissionCleard && jumpSceneAfterClear)
				StartCoroutine (GameClearDisplay ());

			yield return new WaitForSeconds (0.1f);
		}
	}

	IEnumerator GameOverDisplay(){
		transform.FindChild ("Game Over Image").gameObject.SetActive (true);
		transform.GetComponent<AudioSource> ().Stop ();
		Color color = gameOverImage.color;
		while (color.a < 1) {
			color.a += 0.05f;
			gameOverImage.color = color;
			yield return new WaitForSeconds(0.05f);
		}
		transform.FindChild ("Game Over Image").FindChild ("Reset Button").gameObject.SetActive (true);
	}

	IEnumerator GameClearDisplay(){
		transform.FindChild ("Game Clear Image").gameObject.SetActive (true);
		transform.GetComponent<AudioSource> ().Stop ();
		Color color = gameClearImage.color;
		while (color.a < 1) {
			color.a += 0.05f;
			gameClearImage.color = color;
			yield return new WaitForSeconds(0.05f);
		}
		yield return new WaitForSeconds(5f);

		SceneManager.LoadScene (afterClearSceneName);
	}
}
