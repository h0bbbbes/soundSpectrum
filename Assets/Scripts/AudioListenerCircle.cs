using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AudioListenerCircle : MonoBehaviour {
	public GameObject cube;
	public GameObject cubeLineObj;
	public AudioSource song;
	public GameObject  menu;
	public string songName;

	private Vector3 velocityOfCircle;
	private float prevVel;

	private int numOfCubes;
	private string nameOfSong;
	public static List<string> allSongNames;
	private AudioSource clipHolder;
	private GameObject[] cubes;
	private GameObject[] cubeLine;
	private int numOfCubesInCubeLine;
	private Renderer cubeRenderer;
	private Color cubeColor;
	private bool isPaused;
	private bool openMenu;


	// Use this for initialization
	void Start () {

		//Initialize CubesCircle
		int numOfCubesFirstCircle = 16;
		int numOfCubesSecondCircle = 20;
		int numOfCubesThirdCircle = 30;
		int numOfCubesFourthCircle = 40;
		for (int i = 0; i < numOfCubesFirstCircle; i++) {
			float angle = i * Mathf.PI * 2 / numOfCubesFirstCircle;
			float sinPosForZ = Mathf.Sin(angle);
			float cosPosForX = Mathf.Cos (angle);
			Vector3 spawnPos = new Vector3(cosPosForX * 5f, 10, sinPosForZ * 6.5f);
			Instantiate(cube, spawnPos, Quaternion.identity);
		}
		for (int i = 0; i < numOfCubesSecondCircle; i++) {
			float angle = i * Mathf.PI * 2 / numOfCubesSecondCircle;
			float sinPosForZ = Mathf.Sin(angle);
			float cosPosForX = Mathf.Cos (angle);
			Vector3 spawnPos = new Vector3(cosPosForX * 8f, 20, sinPosForZ * 8f);
			Instantiate(cube, spawnPos, Quaternion.identity);
		}
		for (int i = 0; i < numOfCubesThirdCircle; i++) {
			float angle = i * Mathf.PI * 2 / numOfCubesThirdCircle;
			float sinPosForZ = Mathf.Sin(angle);
			float cosPosForX = Mathf.Cos (angle);
			Vector3 spawnPos = new Vector3(cosPosForX * 11f, 0, sinPosForZ * 11f);
			Instantiate(cube, spawnPos, Quaternion.identity);
		}
		for (int i = 0; i < numOfCubesFourthCircle; i++) {
			float angle = i * Mathf.PI * 2 / numOfCubesFourthCircle;
			float sinPosForZ = Mathf.Sin(angle);
			float cosPosForX = Mathf.Cos (angle);
			Vector3 spawnPos = new Vector3(cosPosForX * 14f, 0, sinPosForZ * 14f);
			Instantiate(cube, spawnPos, Quaternion.identity);
		}
	
		cubes = GameObject.FindGameObjectsWithTag ("Cube");

		//Initialize CubeLine

		numOfCubesInCubeLine = 60;
		for (int i = 0; i < numOfCubesInCubeLine; i++) {
			float angle = i * Mathf.PI / numOfCubesInCubeLine;
			float sinPosForY = Mathf.Sin(angle);
			Vector3 spawnPos = new Vector3(23 * Mathf.Cos(angle), 0, (10 * Mathf.Sin(angle)) + 9);
			Instantiate(cubeLineObj, spawnPos, Quaternion.identity);
		}
		
		cubeLine = GameObject.FindGameObjectsWithTag ("CubeLine");

		//Initialize Clip
		DirectoryInfo pathToSongs = new DirectoryInfo ("Assets/Resources/Music");
		FileInfo[] infoOfSongs = pathToSongs.GetFiles ("*.*");
		allSongNames = stripNamesFromFolder (infoOfSongs);

		nameOfSong = "Music/" + songName;
		clipHolder = (AudioSource) FindObjectOfType(typeof(AudioSource));
		clipHolder.clip = Resources.Load (nameOfSong) as AudioClip;
		clipHolder.Play ();

		
		//etc 
		isPaused = new bool();
		openMenu = new bool ();
		velocityOfCircle = new Vector3 (0, 1, 0);
		prevVel = -1;
	}
	
	// Update is called once per frame
	void Update () {
		float[] spectrum = song.GetSpectrumData (1024, 0, FFTWindow.Hamming);

		if(Input.GetKeyDown("space")){
			if(isPaused){
				clipHolder.UnPause();
				velocityOfCircle.y = prevVel;
				isPaused = false;
			}
			else{
				clipHolder.Pause();
				prevVel = (float) velocityOfCircle.y;
				velocityOfCircle.y = 0;
				isPaused = true;
			}
		}
		if(Input.GetKeyDown("m")){
			if(openMenu){
				menu.SetActive(openMenu);
				openMenu = false;

			}
			else{
				menu.SetActive(openMenu);
				openMenu = true;
			}
		}

		ColorizeYo (36, 66, spectrum, "b", 1f);
		ColorizeYo (66, 106, spectrum, "b", .5f);
		ColorizeYo (0, 16, spectrum, "r", 1f);
		ColorizeYo (16, 36, spectrum, "g", 1f);
		ColorizeLine (spectrum);
	}

	void OnGui(){
		if (openMenu) {
			if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
				print("You clicked the button!");
		}
	}

	//Make things pretty and shit
	void ColorizeYo(int firstIndex, int lastIndex, float[] spectrum, string whichScales, float opacity){

		for (int k = firstIndex; k < lastIndex; k++) {

			Vector3 prevSize = cubes[k].transform.localScale;
			prevSize.y = spectrum[k] * 60;
			cubes[k].transform.localScale = prevSize;

			cubeRenderer = cubes[k].GetComponent<MeshRenderer>();
			switch(whichScales){
			case "r": cubeColor = new Color(spectrum[k] * 100, .300f, .700f,  opacity); break;
			case "g": cubeColor = new Color(.100f, spectrum[k] * 100, .300f,  opacity); break;		
			case "b": cubeColor = new Color(.100f, .700f, spectrum[k] * 100, opacity); break;
			}
			cubeRenderer.material.color = cubeColor;
		}
	}

	//Make things move to the beat and shit
	void MoveYo(int firstIndex, int lastIndex, float[] spectrum){

		for (int k = firstIndex; k < lastIndex; k++) {
			float angle = (k) * Mathf.PI * 2/ (lastIndex - firstIndex + 1);
			Vector3 prevPos = cubes[k].transform.localPosition;
			prevPos.y += Mathf.Sin(angle * Time.time)/60; 
			cubes[k].transform.localPosition = prevPos;
		}
	}

	//Make the line pretty and shit
	void ColorizeLine(float[] spectrum){
		for (int k = 0; k < 59; k++) {
			float angle = k * Mathf.PI / numOfCubesInCubeLine;

			float sinPosForY = spectrum[k] * Mathf.Sin(angle  * Time.time);
			Vector3 spawnPos = new Vector3(-1 * 23 * Mathf.Cos(angle), sinPosForY * 100, (10 * Mathf.Sin(angle)) + 9);
			cubeLine[k].transform.localPosition = spawnPos;
			Vector3 prevPos = cubeLine[k].transform.localScale;
			prevPos.y = spectrum[k] * 60;
			cubeLine[k].transform.localScale = prevPos;
		}
		Vector3 lastPrevPos = cubes[59].transform.localScale;
		lastPrevPos.y = FindHighestFloat (59, 1023, spectrum); 
		cubes [59].transform.localScale = lastPrevPos;
	
	}

	//Get raw names as strings per song in song folder
	List<string> stripNamesFromFolder (FileInfo[] filesOfSongs){
		string fileName = "";
		int indexOfPeriod = 0;
		List<string> returnList = new List<string> ();
		foreach(FileInfo song in filesOfSongs){
			if(!song.Name.Contains("meta")){
				indexOfPeriod = song.Name.IndexOf(".mp3");
				fileName = song.Name.Substring(0,indexOfPeriod);
				returnList.Add(fileName);
			}
		}
		return returnList;
	}

	float FindHighestFloat(int firstIndex, int lastIndex, float[] spectrum){
		float retVal = 0.0f;
		for(int i = firstIndex; i < lastIndex+1; i++){
			if(spectrum[i] > retVal){
				retVal = spectrum[i];
			}
		}
		return retVal;
	}
}
