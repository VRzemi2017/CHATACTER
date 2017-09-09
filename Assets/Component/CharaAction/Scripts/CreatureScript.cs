using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureScript: MonoBehaviour {
		
	//Basic parametrs
	public float moveSpeed = 15.0f;     // move speed

	//Self components
	private Transform myTransform;
	Animator anim;

	//Targeting
	[SerializeField]
	private Transform[] targets; // target array
	private int nowTarget; // current target

	// Time manager
	private float timer;
	private float stopTimer = 0f;

	private float timer2;

	[Range(1f, 10f)]
	public float attackTimer = 4f;

	[Range(1f, 100f)]
	public float damage = 1f;

	public bool patrolLoop; 			// path loop
	private bool playerSpotted = false; // Player spotted

	private bool onGround = false;
	private bool isJumping = false;

	//Players
	private Transform player1;
	private Transform player2;
	private Transform mainCamera;

	private void Start( ){
		myTransform = transform; 										 // Transform set
		player1 = GameObject.FindGameObjectWithTag("Player").transform; // find Player1 position
//		player2 = GameObject.FindGameObjectWithTag ("Player2").transform;	// find Player2 position
		mainCamera = GameObject.FindGameObjectWithTag ( "MainCamera" ).transform;
		anim = GetComponent<Animator> ();
	}
		
	private void Update(){
		// Move along array of targets
		if (nowTarget < targets.Length) {
			Move ();
		} else if (patrolLoop) {
			nowTarget = 0; // loop
		} 

		Animate ();
	}
		
	// Move towards target
	private void Move(){

		// Path patrol
		Vector3 target = targets[nowTarget].position;
		Vector3 dir = targets[nowTarget].position - myTransform.position;
		myTransform.position = Vector3.MoveTowards (myTransform.position, target, moveSpeed * Time.deltaTime);

		// Targeting
		if (dir.magnitude < 0.5f) {
			if (timer == 0) {
				timer = Time.time;
			}
			if ((Time.time - timer) >= stopTimer ) {
				nowTarget++;
				timer = 0; 
			}
		} else if (playerSpotted) { // Attack player
			target = player1.position;
			dir = player1.position - myTransform.position;
			myTransform.position = Vector3.MoveTowards (myTransform.position, target, moveSpeed * Time.deltaTime);
			myTransform.LookAt (mainCamera);
			timer2 += Time.deltaTime;
			//leave player alone after 4 seconds
			if (timer2 >= attackTimer) {
				timer2 = 0;
				playerSpotted = false;
			}
		}
	}

	//ENTER COLLIDER
	private void OnTriggerEnter( Collider other ) {
		if (other.CompareTag("SpeedUp")) { 			 // speed boost to simulate jumping ( blue spheres )
			moveSpeed = 15.0f;
			stopTimer = 0f;
			myTransform.localRotation = Quaternion.identity;
			isJumping = true;
			onGround = false;
		} /*else if (other.tag == "SpeedNormal") { // reset speed to normal ( yellow spheres ) !! buggy, try not to use
			stopTimer = 3.0f;
			moveSpeed = 5.0f;
		}*/ else if (other.CompareTag("Wait") || other.CompareTag("Spawn")) {		 // wait before moving / jumping ( orange spheres )
			stopTimer = 3.0f;
			moveSpeed = 15.0f;
			myTransform.localRotation = Quaternion.identity;
			isJumping = false;
			onGround = true;
		} 
			
		// Rotate transform towards next target
		if (other.CompareTag ("SpeedUp") || other.CompareTag ("Wait")) {
			myTransform.LookAt (targets [nowTarget + 1].position);
		} else if (other.CompareTag ("Spawn")) {
			myTransform.LookAt (targets [nowTarget + 1].position);
		}

		// Spot player
		if (other.CompareTag("Player")) {
			playerSpotted = true;
			moveSpeed = 15.0f;
		}
	}

	// EXIT COLLIDER
	private void OnTriggerExit ( Collider other ) {
		// reset timer to normal
		if (other.CompareTag("Wait") || other.CompareTag("SpeedUp")) {
			stopTimer = 0.5f;
		}

		// Lose player
		if (other.CompareTag("Player")) {
			playerSpotted = false;
		}
			
		// Destroy game object if it leaves Playzone ( DEBUG )
		if (other.CompareTag("Playzone")) {
			Destroy (gameObject);
		}
	}

	private void Animate(){
		if (isJumping == true || playerSpotted == true ) {
			anim.SetTrigger ("Wait");
		} else if (isJumping == false) {
			anim.SetTrigger ("Jump");
		}
	}
		
}

