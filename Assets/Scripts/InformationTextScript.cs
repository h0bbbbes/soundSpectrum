using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class InformationTextScript : MonoBehaviour {
	Text headText;
	AudioClip clip;
	List<string> songNames;
	void Start(){
		headText = GameObject.FindObjectOfType<Text> ();
		headText.text = "https://github.com/dnyu";
		Invoke ("GetSongNames", .5f);
	}

	void GetSongNames(){
		songNames = AudioListenerCircle.allSongNames;
	}

	void Update(){

	}


}
