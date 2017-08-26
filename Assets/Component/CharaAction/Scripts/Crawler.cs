using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crawler: MonoBehaviour {
		
	//Basic parametrs
	public float moveSpeed = 5.0f;     // move speed

	//Self components
	private Transform myTransform;
	//private Rigidbody rbody;

	//Targeting
	[SerializeField]
	private Transform[] targets; // target array

	private int nowTarget; // current target

	// Time manager
	private float timer;
	public float stopTimer = 0f;

	public bool patrolLoop; 			// path loop
	private bool playerSpotted = false; // Player spotted
	public float agroDis = 10.0f; 		// Player attack distance

	//Players
	private Transform player1;
	private Transform player2;

	private void Start( ){
		//rbody = GetComponent<Rigidbody> (); 						     // RBody get
		myTransform = transform; 										 // Transform set
		player1 = GameObject.FindGameObjectWithTag("Player").transform; // find Player1 position
//		player2 = GameObject.FindGameObjectWithTag ("Player2").transform;// find Player2 position
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
		} else if (playerSpotted) { // Attack player
			target = player1.position;
			dir = player1.position - myTransform.position;
			myTransform.position = Vector3.MoveTowards (myTransform.position, target, moveSpeed * Time.deltaTime);
			myTransform.LookAt (target);
		}
	}

	//Change speed to simulate jump 
	private void OnTriggerEnter( Collider other ) {
		if (other.tag == "SpeedUp") { 			 // speed boost to simulate jumping ( blue spheres )
			moveSpeed = 15.0f;
			stopTimer = 0f;
		} else if (other.tag == "SpeedNormal") { // reset speed to normal ( yellow spheres ) !! buggy
			stopTimer = 3.0f;
			moveSpeed = 5.0f;
		} else if (other.tag == "Wait") {		 // wait before moving / jumping ( orange spheres )
			stopTimer = 3.0f;
			moveSpeed = 15.0f;
		} else {
			moveSpeed = 5.0f;                    // usual speed
		}

		// Rotate transform towards next target
		if (other.tag == "SpeedUp" || other.tag == "SpeedNormal" || other.tag == "nowTarget" || other.tag == "Wait") {
				myTransform.LookAt (targets [nowTarget + 1]);
		}

		// Spot player
		if (other.tag == "Player") {
			playerSpotted = true;
			moveSpeed = 15.0f;
		}
	}

	// Reset wait time to normal
	private void OnTriggerExit ( Collider other ) {
		// reset timer to normal
		if (other.tag == "Wait" || other.tag == "SpeedUp" ) {
			stopTimer = 0.5f;
		}

		// Lose player
		if (other.tag == "Player") {
			playerSpotted = false;
		}

		// Destroy object if it leaves playzone ( DEBUG )
		if (other.tag == "Playzone") {
			Destroy (gameObject);
		}
	}
		
}

