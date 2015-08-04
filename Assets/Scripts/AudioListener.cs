using UnityEngine;
using System.Collections;

public class AudioListener : MonoBehaviour {
	public GameObject cube;
	public AudioSource song;
	private int numOfCubes;
	GameObject[] cubes;
	// Use this for initialization
	void Start () {
		numOfCubes = 60;
		for (int i = 0; i < numOfCubes; i++) {
			float angle = i * Mathf.PI * 2 / numOfCubes;
			float sinPosForY = Mathf.Sin(angle);
			Vector3 spawnPos = new Vector3(1, sinPosForY, i);
			Instantiate(cube, spawnPos, Quaternion.identity);
		}

		cubes = GameObject.FindGameObjectsWithTag ("Cube");
	}
	
	// Update is called once per frame
	void Update () {
		float[] spectrum = song.GetSpectrumData (1024, 0, FFTWindow.Hamming);

		for (int k = 0; k < 59; k++) {
			float angle = k % Mathf.PI * 2;
			float sinPosForY = spectrum[k] * 100 *  Mathf.Log(100) * Mathf.Sin(angle  * Time.time);
			Vector3 spawnPos = new Vector3(1, sinPosForY, k);
			cubes[k].transform.localPosition = spawnPos;
			Vector3 prevPos = cubes[k].transform.localScale;
			prevPos.y = spectrum[k] * 60;
			cubes[k].transform.localScale = prevPos;
		}
		Vector3 lastPrevPos = cubes[59].transform.localScale;
		lastPrevPos.y = FindHighestFloat (59, 1023, spectrum); 
		cubes [59].transform.localScale = lastPrevPos;

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
