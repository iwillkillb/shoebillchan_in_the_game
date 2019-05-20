using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CutsceneAnimatorFunctions : MonoBehaviour {

	RectTransform rtBox, rtText, rtSkip;
	Text text;
	public string sceneName;
	public int margin = 5;
	[TextArea] public string [] script;

	// Use this for initialization
	void Awake () {
		rtBox = transform.FindChild ("Canvas").transform.FindChild ("TextBox").GetComponent<RectTransform> ();
		rtText = rtBox.FindChild ("Text").GetComponent<RectTransform> ();
		rtSkip = transform.FindChild ("Canvas").transform.FindChild ("Skip Button").GetComponent<RectTransform> ();
		text = rtText.GetComponent<Text> ();

		rtBox.sizeDelta = new Vector2 (Screen.width, Screen.height * 0.25f);
		rtBox.position = new Vector3 (Screen.width * 0.5f, rtBox.sizeDelta.y * 0.5f, 0);

		rtText.sizeDelta = new Vector2 (rtBox.sizeDelta.x - margin * 2, rtBox.sizeDelta.y - margin * 2);
		rtText.position = rtBox.position;

		rtSkip.position = new Vector3 (Screen.width, 0, 0);
	}
	
	// Update is called once per frame
	public void ScriptControl (int scriptIndex) {
		if(scriptIndex > 0 && scriptIndex < script.Length)
			text.text = script [scriptIndex];
		else
			text.text = script [0];
	}

	public void ChangeScene () {
		SceneManager.LoadScene (sceneName);
	}
}
