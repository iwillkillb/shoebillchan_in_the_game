using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;	//for List
using UnityEngine.SceneManagement;

public class TitleFunctions : MonoBehaviour {

	public string startStageSceneName;	//New Game -> What is First scene's name?
	public string[] stageSceneNames;	//You need input Scene names to Inspector Tab.
	List <string> stageList = new List <string> ();	//stages[] -> stageList[]
	int selectedStageIndex = -1;
	Text stageSelectButtonText;

	void Awake () {
		stageSelectButtonText = transform.FindChild ("Stage Select").GetComponentInChildren<Text> ();
	}

	void Start () {
		//THIS PROCESS IS REQUIRED "SelectStage()" AND "SelectedStageChange()".
		//public string[] stageSceneNames -> stageList


		if (stageList.Count < 1) {	//First Scene(Start Scene) Insert!
			stageList.Add (startStageSceneName);
		}
		for (int a = 0; a < stageSceneNames.Length; a++) {
			if (PlayerPrefs.GetInt ("stage" + stageSceneNames[a]) == 1) {
				stageList.Add (stageSceneNames[a]);
			}
		}
		PlayerPrefs.SetInt ("scoreReset", 0);	//Score Setting Default : NO RESET!!
	}

	public void NewGame () {
		for (int a = 0; a < stageSceneNames.Length; a++) {
			if (PlayerPrefs.GetInt ("stage" + stageSceneNames[a]) == 1) {
				PlayerPrefs.DeleteKey ("stage" + stageSceneNames[a]);
			}
		}
		PlayerPrefs.SetInt ("scoreReset", 1);	//This sets Player's score to ZERO.
		SceneManager.LoadScene (startStageSceneName);
	}

	public void StageSelect () {
		if(0<=selectedStageIndex && selectedStageIndex < stageList.Count)
			SceneManager.LoadScene (stageSelectButtonText.text);
	}

	public void ExitGame () {
		Application.Quit ();
	}

	public void SelectedStageChange (bool isIncrease) {	//For selecting stage Left/Right Arrow Buttons (Side of Stage Select Button)

		if (isIncrease) {
			if (selectedStageIndex < stageList.Count)	//Increase Button Input
				selectedStageIndex += 1;
		} else {
			if (selectedStageIndex >= 0)	//Decrease Button Input
				selectedStageIndex -= 1;
		}

		if (selectedStageIndex >= stageList.Count)	//Keep Array's Length
			selectedStageIndex = stageList.Count - 1;
		else if (selectedStageIndex < 0)
			selectedStageIndex = 0;

		stageSelectButtonText.text = stageList [selectedStageIndex];	//Button's Text change
		if (selectedStageIndex == -1)
			stageSelectButtonText.text = "Stage Select";
		else
			stageSelectButtonText.text = stageList [selectedStageIndex];
	}
}
