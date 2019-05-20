using UnityEngine;
using System.Collections;

public class SoundMaker : MonoBehaviour {

	public AudioClip[] audioFile;
	AudioSource myAudio;

	// Use this for initialization
	void Start () {
		myAudio = GetComponent<AudioSource> ();
	}
	
	public void SoundOn(int index){
		myAudio.PlayOneShot (audioFile[index]);
	}
}
