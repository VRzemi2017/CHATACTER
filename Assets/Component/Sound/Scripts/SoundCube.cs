﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCube : MonoBehaviour {

	public float speed;
	public new KeyCode name;

	// Use this for initialization
	void Start () {

	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (name) && Input.GetKey (KeyCode.Keypad8)) {
			this.transform.position += this.transform.up * speed;
		}
		if (Input.GetKey (name) && Input.GetKey (KeyCode.Keypad2)) {
			this.transform.position -= this.transform.up * speed;
		}
	}
}
