using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHUDSetting : MonoBehaviour {

	public GameObject bossObj;
	EnemyStatus es;
	RectTransform nameRect, hpBarRect;
	float hpBarRectWidthInit, hp;

	// Use this for initialization
	void Awake () {
		if (bossObj == null)
			gameObject.SetActive (false);
		else {
			es = bossObj.GetComponent<EnemyStatus> ();
			nameRect = transform.FindChild ("Name Text").GetComponent<RectTransform> ();
			hpBarRect = transform.FindChild ("HP Bar").GetComponent<RectTransform> ();

			nameRect.position = new Vector3 (Screen.width * 0.5f - nameRect.sizeDelta.x * 0.5f, Screen.height * 0.15f + nameRect.sizeDelta.y * 0.5f, 0);
			hpBarRect.position = new Vector3 (Screen.width * 0.5f - hpBarRect.sizeDelta.x * 0.5f, Screen.height * 0.15f - hpBarRect.sizeDelta.y * 0.5f, 0);

			hp = es.hp / es.hp_Original;
			hpBarRectWidthInit = hpBarRect.sizeDelta.x;

			transform.FindChild ("Name Text").GetComponent<Text> ().text = bossObj.name;
			hpBarRect.sizeDelta = new Vector2 (hpBarRectWidthInit * hp, hpBarRect.sizeDelta.y);

			StartCoroutine (HudSetting ());
		}
	}

	IEnumerator HudSetting(){
		while (true) {
			if (hp > 1)
				hp = 1;
			else if (hp < 0)
				hp = 0;
			if (hp != es.hp / es.hp_Original) {
				hp = es.hp / es.hp_Original;
				hpBarRect.sizeDelta = new Vector2 (hpBarRectWidthInit * hp, hpBarRect.sizeDelta.y);
			}

			if (es.die) {
				transform.GetComponentInParent<CanvasSetting> ().isMissionCleard = true;
				Destroy (gameObject);
			}

			yield return new WaitForSeconds (0.1f);
		}
	}
}
