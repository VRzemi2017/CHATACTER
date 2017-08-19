using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crawler: MonoBehaviour {
		
	//Basic parametrs
	public float moveSpeed = 2;     // move speed
	private float disToGround = 2.5f;

	//Self components
	private Transform myTransform;
	private Rigidbody rbody;

	//Targeting
	[SerializeField]
	private Transform[] targets; // target array

	private int nowTarget; // current target
	private Transform lastTarget;

	// Time manager
	private float timer;
	public float stopTimer = 0f;

	// Path loop
	public bool patrolLoop;
	private bool playerSpotted = false; // Player spotted
	public float agroDis = 30.0f; // Player spot distance

	//Players
	private Transform player1;
	private Transform player2;

	private void Start( ){
		rbody = GetComponent<Rigidbody> (); 						     // RBody get
		myTransform = transform; 										 // Transform set
		player1 = GameObject.FindGameObjectWithTag("Player1").transform; // find Player1 position
	//	player2 = GameObject.FindGameObjectWithTag ("Player2").transform;// find Player2 position
	}
		
	private void Update(){
	
		// Move along array of targets
		if (nowTarget < targets.Length) {
			Move ();
		} else if (patrolLoop) {
			nowTarget = 0;
		}
	}
		
	// Move towards target
	private void Move(){

		// Path patrol
		Vector3 target = targets[nowTarget].position;
		Vector3 dir = targets[nowTarget].position - myTransform.position;
		myTransform.position = Vector3.MoveTowards (myTransform.position, target, moveSpeed * Time.deltaTime);

		// Target swtich
		if (dir.magnitude < 0.5f) {
			if (timer == 0) {
				timer = Time.time;
			}
			if ((Time.time - timer) >= stopTimer) {
				nowTarget++;
				timer = 0;
			}
		}
	}

	//Change speed to simulate jump 
	private void OnTriggerEnter( Collider other ) {
		if (other.tag == "SpeedUp") {
			moveSpeed = 15.0f;
			stopTimer = 0f;
		} else if (other.tag == "SpeedNormal") {
			stopTimer = 3.0f;
			moveSpeed = 2.0f;
		} else if (other.tag == "Wait") {
			stopTimer = 3.0f;
			moveSpeed = 15.0f;
		} else {
			moveSpeed = 2.0f;
		}

		// DEBUG LOG NEXT TARGET CHECK
		if (other.tag == "SpeedUp" || other.tag == "SpeedNormal" || other.tag == "nowTarget" || other.tag == "Wait" ) {
			myTransform.LookAt (targets [nowTarget + 1]);
		}
	}

	// Reset wait time to normal
	private void OnTriggerExit ( Collider other ) {
		if (other.tag == "Wait" || other.tag == "SpeedUp" ) {
			stopTimer = 0.5f;
		}
	}
}

