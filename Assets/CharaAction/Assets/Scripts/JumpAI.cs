﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAI : MonoBehaviour {

	[SerializeField]
	Transform[] waypoint;

	[SerializeField]
	float initialAngle;

	GameObject enemy;
	Rigidbody rbody;

	private float curTime;
	private float gravity = 9.8f;
	private int currentWaypoint;
	private bool hitTarget;
	private bool isJumping;
	private bool isGrounded;

	public float rotate = 60.0f;
	public float force;
	public bool loop;
	public float pause = 0f;
	Vector3 torque = Vector3.up;

	void Awake(){
		this.enemy = GameObject.Find ("Enemy");
		rbody = GetComponent<Rigidbody> ();
	}

	void Start(){
		hitTarget = false; 
		isJumping = false;
		isGrounded = true;

		/*
		rbody.AddForce (transform.forward * force, ForceMode.Impulse);
		rbody.AddForce (transform.up * force, ForceMode.Impulse);
		rbody.AddTorque (torque, ForceMode.Force);*/

	}

	void Update(){
		if (currentWaypoint < waypoint.Length) {
			Jump ();
		} else if (loop) {
			currentWaypoint = 0;
		}
	}

	void FixedUpdate(){
		//Gravity
		rbody.AddForce ( Vector3.down * -gravity * rbody.mass );
	}

	/*Vector3 BallisticVelocity (Transform target, float angle){
	height = dir.y; // get height difference
	dir.y = 0; // retain only the horizontal difference
	float dist = dir.magnitude; // get horizontal direction
	float a = angle * Mathf.Deg2Rad; // Convert angle to radians
	dir.y = dist * Mathf.Tan(a); // set dir to the elevation angle.
	dist += height / Mathf.Tan(a); // Correction for small height differences

	// Calculate the velocity magnitude
	float velocity = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
	return velocity * dir.normalized; // Return a normalized vector. 
	}*/

	void Jump(){


		//move to target
		Vector3 target = waypoint[currentWaypoint].position;
		Vector3 dir = waypoint[currentWaypoint].position - transform.position;

		// Kinematic equation calculations
		/*
		float height = dir.y; // get height difference
		dir.y = 0; // retain only the horizontal difference
		float dist = dir.magnitude; // get horizontal direction
		float a = angle * Mathf.Deg2Rad; // Convert angle to radians
		dir.y = dist * Mathf.Tan(a); // set dir to the elevation angle.
		dist += height / Mathf.Tan(a); // Correction for small height differences

		// Calculate the velocity magnitude
		float velocity = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
		return velocity * dir.normalized; // Return a normalized vector.
		*/

		// Calculations //

			float gravity = Physics.gravity.magnitude;
		// Selected angle in radians
			float angle = initialAngle * Mathf.Deg2Rad;

		// Positions of this object and the target on the same plane
		Vector3 planarTarget = new Vector3 (target.x, 0, target.z);
		Vector3 planarPostion = new Vector3 (transform.position.x, 0, transform.position.z);

		// Planar distance between objects
			float distance = Vector3.Distance (planarTarget, planarPostion);
		// Distance along the y axis between objects
			float yOffset = transform.position.y - target.y;
			float initialVelocity = (1 / Mathf.Cos (angle)) * Mathf.Sqrt ((1.5f * gravity * Mathf.Pow (distance, 1.5f)) / (distance * Mathf.Tan (angle) + yOffset));
			Vector3 velocity = new Vector3 (0, initialVelocity * Mathf.Sin (angle), initialVelocity * Mathf.Cos (angle));

		// Rotate our velocity to match the direction between the two objects
			float angleBetweenObjects = Vector3.Angle (Vector3.forward, planarTarget - planarPostion);
			Vector3 finalVelocity = Quaternion.AngleAxis (angleBetweenObjects, Vector3.up) * velocity;

		// Pause
		if (dir.magnitude < 0.5f) {
			if (curTime == 0) {
				curTime = Time.time;
			}
			if ((Time.time - curTime) >= pause) {
				currentWaypoint++;
				curTime = 0;
			} 
		} else {
			// JUMP
			if (isGrounded) {
				transform.LookAt (waypoint [currentWaypoint]);
				rbody.velocity = finalVelocity; // JUMP
				isJumping = true;
			} else if (isJumping) {
				isGrounded = false;
			}
		}
	}

	void OnTriggerEnter( Collider other ){
		if (other.tag == "Target") {
			Debug.Log ("Reached target");
			transform.LookAt (waypoint [currentWaypoint + 1]);
			rbody.isKinematic = true;
			isJumping = false;
			hitTarget = true;
		}
	}
}