using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	Rigidbody rb;
	Transform target;
	Vector3 dir;

	// Use this for initialization
	void Start () {
		rb = GetComponent< Rigidbody > ();
		target = GameObject.FindWithTag ( "Player" ).transform;
	}
	
	// Update is called once per frame
	void Update () {
		if ( transform != target ){
			;
		}
	}
}
