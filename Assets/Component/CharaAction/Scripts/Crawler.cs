using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum enemyState{
	SEARCH,
	TARGET_PLAYER1,
	TARGET_PLAYER2,
	TARGET_STAFF,
	TARGET_CLOSEST,
	TARGET_RANDOM, 
	PAUSE
}
	
public class Crawler: MonoBehaviour {

	//AI states
	enemyState curState;

	//Basic parametrs
	public float moveSpeed = 2; // move speed
	private float turnSpeed = 90; // turning speed (degrees/second)
	private float lerpSpeed = 10; // smoothing speed
	private float gravity = 10; // gravity acceleration
	private float time;
	private float pause = 5.0f;

	//Ground and jumping
	private bool isGrounded = true;
	private bool jumping = false; 
	private float deltaGround = 0.2f; // character is grounded up to this distance
	private float jumpSpeed = 1; // vertical jump initial speed
	private float jumpRange = 1f; // range to detect target wall
	private float groundCastLength = 1;

	//Surface check
	private Vector3 surfaceNormal; // current surface normal
	private Vector3 myNormal; // character normal
	private float distGround; // distance from character position to ground

	//Self components
	private Transform myTransform;
	public BoxCollider boxCollider; 
	private Rigidbody rigidbody;

	//Targeting
	[SerializeField]
	private Transform[] targets;
	private int nowTarget;
	private Transform lastTarget;
	private float searchRange = 15.0f;
	private float timer;
	public float stopTimer = 0f;
	public bool patrolLoop;
	bool playerSpotted = false;
	public float agroDis = 30.0f;
	private float clingDis = 2.5f;
	private float closeEnough = 2.5f;

	//Players
	private Transform player1;
	private Transform player2;

	private void Start( ){
		rigidbody = GetComponent<Rigidbody> (); 						// get component
		boxCollider = GetComponent<BoxCollider> (); 					// get component
		myNormal = transform.up; 										// normal starts as character up direction
		myTransform = transform; 										// set transform
		rigidbody.freezeRotation = true; 								// disable physics rotation
		distGround = boxCollider.extents.y - boxCollider.center.y;  	// distance from transform.position to ground
		player1 = GameObject.FindGameObjectWithTag("Player1").transform;
	//	player2 = GameObject.FindGameObjectWithTag ("Player2").transform;
	}

	private void FixedUpdate(){
		// Gravity
		rigidbody.AddForce(-gravity*rigidbody.mass*myNormal);
		isGrounded = Physics.Raycast (transform.position, transform.up * -1, groundCastLength);
	}
		
	private void Update(){

		// SWITCH 
		switch (curState) {

		//Search	
		case enemyState.SEARCH:
			//Search ();
			break;
		
		//Target P1
		case enemyState.TARGET_PLAYER1:
			lastTarget = player1.transform;
			moveSpeed = 2.0f;
			myTransform.LookAt (player1);
			myTransform.position = Vector3.MoveTowards (myTransform.position, player1.position, moveSpeed * Time.deltaTime);
			Debug.Log ("Target : " + lastTarget); 
			break;
		
		//Target P2
		case enemyState.TARGET_PLAYER2:
			lastTarget = player2.transform;
			moveSpeed = 2.0f;
			myTransform.LookAt (player2);
			myTransform.position = Vector3.MoveTowards (myTransform.position, player2.position, moveSpeed * Time.deltaTime);
			Debug.Log ("Target : " + lastTarget); 
			break;

		//Target closest path
		case enemyState.TARGET_CLOSEST:
			break;
		
		//Target player's staff and vibrate controller
		case enemyState.TARGET_STAFF:
			break;

		//Target random path point
		case enemyState.TARGET_RANDOM:
			break;

		// Stop
		case enemyState.PAUSE:
			moveSpeed = 0f;
			break;

		}

		if (nowTarget < targets.Length) {
			Move ();
		} else if (patrolLoop) {
			nowTarget = 0;
		}

		// if state is null do Search
		if (curState == null) {
			curState = enemyState.SEARCH;
		}

	}

	/*private void Search(){
		Ray ray;
		RaycastHit hit;
		ray = new Ray (myTransform.position, myTransform.forward);
		Vector3 forward = transform.TransformDirection (Vector3.forward) * searchRange; // ray debug
		Debug.DrawRay (transform.position, forward, Color.green);						// ray debug
		// Get target
		if (Physics.Raycast (ray, out hit, searchRange) && hit.transform.tag == "Player1") {
			curState = enemyState.TARGET_PLAYER1;
		} else if (hit.transform.tag == "Player2") {
			curState = enemyState.TARGET_PLAYER2;
		} 
	}*/


	// Find closes target
	GameObject FindClosestTarget ( ){
		GameObject[] targets;
		targets = GameObject.FindGameObjectsWithTag ("NowTarget");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = myTransform.position;

		foreach (GameObject nowTarget in targets) {
			Vector3 diff = nowTarget.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = nowTarget;
				distance = curDistance;
			}
		}
		return closest;
	}

		
	// Move towards target
	private void Move(){

		// Patrolling
		Vector3 target = targets[nowTarget].position;
		Vector3 dir = targets[nowTarget].position - myTransform.position;
		myTransform.LookAt (targets [nowTarget]);
		myTransform.position = Vector3.MoveTowards (myTransform.position, target, moveSpeed * Time.deltaTime);

		//direction set
		//Vector3 direction = lastTarget.transform.position - myTransform.position;

		if (dir.magnitude < 0.5f) {
			if (timer == 0) {
				timer = Time.time;
			}
			if ((Time.time - timer) >= stopTimer) {
				nowTarget++;
				timer = 0;
			}
		}
			
		// abort Update while jumping to a wall
		if (jumping) {
			return;
		}

		//Rays
		Ray ray;
		RaycastHit hit;

		//if (Input.GetButtonDown("Jump")){ // MANUAL CONTROLS
		// new ray
		ray = new Ray(myTransform.position + transform.up * 5.75f, myTransform.forward);
		Vector3 forward = transform.TransformDirection (Vector3.forward) * jumpRange; // ray debug	
		Debug.DrawRay (transform.position, forward, Color.red); 					  // ray debug
		if (Physics.Raycast(ray, out hit, jumpRange)){ // wall check
			JumpToWall(hit.point, hit.normal); // init JumpToWall IF wall ahead
			rigidbody.velocity += jumpSpeed * myNormal;
		}
		/*else if (isGrounded){ // if grounded, jump up // HOPPING
				rigidbody.velocity += jumpSpeed * myNormal;
			}*/
		//}

		//myTransform.Rotate(0, Input.GetAxis("Horizontal")*turnSpeed*Time.deltaTime, 0); // MANUAL CONTROLS

		// update surface normal and isGrounded:
		ray = new Ray(myTransform.position, -myNormal); // cast ray downwards
		if (Physics.Raycast(ray, out hit)){ // use it to update myNormal and isGrounded
			isGrounded = hit.distance <= distGround + deltaGround;
			surfaceNormal = hit.normal;
		}
		else {
			isGrounded = false;
			// assume usual ground normal
			surfaceNormal = Vector3.up;
		}
		myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed*Time.deltaTime);
		// find forward direction with new myNormal:
		Vector3 myForward = Vector3.Cross(myTransform.right, myNormal);
		// align character to the new myNormal while keeping the forward direction:
		Quaternion targetRot = Quaternion.LookRotation(myForward, myNormal);
		myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRot, lerpSpeed*Time.deltaTime);

		// move the character forth/back with Vertical axis:
		//myTransform.Translate(0, 0, Input.GetAxis("Vertical")*moveSpeed*Time.deltaTime); // MANUAL CONTROL

	}

	private void JumpToWall(Vector3 point, Vector3 normal){
		// Jump to wall
		jumping = true; 
		rigidbody.isKinematic = true; // Disable physics while jumping
		Vector3 orgPos = myTransform.position;
		Quaternion orgRot = myTransform.rotation;
		Vector3 dstPos = point + normal * (distGround + 0.5f); // Will jump to 0.5 above wall
		Vector3 myForward = Vector3.Cross(myTransform.right, normal);
		Quaternion dstRot = Quaternion.LookRotation(myForward, normal);

		StartCoroutine (jumpTime (orgPos, orgRot, dstPos, dstRot, normal));
	}

	private IEnumerator jumpTime(Vector3 orgPos, Quaternion orgRot, Vector3 dstPos, Quaternion dstRot, Vector3 normal) {
		for ( float t = 0.0f; t < 1.0f; ){
			t += Time.deltaTime;
			myTransform.position = Vector3.Lerp(orgPos, dstPos, t);
			myTransform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
			yield return null; // return here next frame
		}
		myNormal = normal; // update myNormal
		rigidbody.isKinematic = false; // enable physics
		jumping = false; // jumping to wall finished
	}
}

