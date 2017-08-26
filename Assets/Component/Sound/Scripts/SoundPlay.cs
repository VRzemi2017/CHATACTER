using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour {

	public AudioSource _soundCube;

	// Use this for initialization
	void Start () {
		_soundCube = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			_soundCube.Play ();
			Debug.Log ( "ボタンを押しました！！" );
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			_soundCube.playOnAwake = false;
			Debug.Log ( "PlayOnAwakeをOFFにしました！！" );
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			_soundCube.playOnAwake = true;
			Debug.Log ( "PlayOnAwakeをONにしました！！" );
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			_soundCube.spatialBlend -= 0.1f;
			Debug.Log ( "3D→2D" );
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			_soundCube.spatialBlend += 0.1f;
			Debug.Log ( "2D→3D" );
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			_soundCube.spatialize = false;
			Debug.Log ( "空間化を無効にしました" );
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			_soundCube.spatialize = true;
			Debug.Log ( "空間化を有効にしました" );
		}
	}
}
