using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crawler: MonoBehaviour {
		
	//Basic parametrs
	public float moveSpeed = 5.0f;     // move speed

	//Self components
	private Transform myTransform;
	private Rigidbody rbody;

	//Targeting
	[SerializeField]
	private Transform[] targets; // target array

	private int nowTarget; // current target

	// Time manager
	private float timer;
	public float stopTimer = 0f;

	// Path loop
	public bool patrolLoop;
	private bool playerSpotted = false; // Player spotted
	public float agroDis = 10.0f; // Player attack distance

	//Players
	private Transform player1;
	private Transform player2;

	private void Start( ){
		rbody = GetComponent<Rigidbody> (); 						     // RBody get
		myTransform = transform; 										 // Transform set
		player1 = GameObject.FindGameObjectWithTag("Player1").transform; // find Player1 position
//		player2 = GameObject.FindGameObjectWithTag ("Player2").transform;// find Player2 position
	}
		
	private void Update(){
	
		// Move along array of targets
		if (nowTarget < targets.Length) {
			Move ();
		} else if (patrolLoop) {
			nowTarget = 0;
		}

		/*// Attack player within range 
		if (Vector3.Distance (myTransform.position, player1.position) <= agroDis) {
			AttackPlayer1 ();
		} else if (Vector3.Distance (myTransform.position, player2.position) <= agroDis) {
			AttackPlayer2 ();
		} */
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
			moveSpeed = 5.0f;
		}

		// Rotate transform towards next target
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

		
  /*  // Player1 interaction
    private void AttackPlayer1() {
		moveSpeed = 5.0f;
		Vector3 target = player1.position;
		Vector3 dir = player1.position - myTransform.position;
		myTransform.position = Vector3.MoveTowards (myTransform.position, target, moveSpeed * Time.deltaTime);
    }

	// Player2 interaction
	private void AttackPlayer2() {
		moveSpeed = 5.0f;
		Vector3 target = player2.position;
		Vector3 dir = player2.position - myTransform.position;
		myTransform.position = Vector3.MoveTowards (myTransform.position, target, moveSpeed * Time.deltaTime);
	}*/
}

