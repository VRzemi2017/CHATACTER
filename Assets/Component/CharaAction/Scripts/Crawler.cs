using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crawler: MonoBehaviour {
		
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
	private float jumpSpeed = 0.1f; // vertical jump initial speed
	private float jumpRange = 2f; // range to detect target wall
	private float groundCastLength = 1;

	//Surface check
	private Vector3 surfaceNormal; // current surface normal
	private Vector3 myNormal; // character normal
	private float distGround; // distance from character position to ground

	//Self components
	private Transform myTransform;
	private BoxCollider boxCollider; 
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
	private bool playerSpotted = false;
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
		//rigidbody.AddForce(-gravity*rigidbody.mass*myNormal);
	}
		
	private void Update(){
		Ray ray;
		RaycastHit hit;

		if (jumping) {
			return;
		}
			
		if (nowTarget < targets.Length) {
			Move ();
		} else if (patrolLoop) {
			nowTarget = 0;
		}

		//Update myNormal
		ray = new Ray (myTransform.position, myTransform.TransformDirection(-Vector3.up)); 							// cast ray downwards
		Vector3 down = myTransform.TransformDirection (-Vector3.up) * 0.5f; 		// ray debug	
		Debug.DrawRay (transform.position, down, Color.yellow); 					// ray debug
		if (Physics.Raycast (ray, out hit)) { 										// use it to update myNormal and isGrounded
			isGrounded = hit.distance <= distGround + deltaGround;
			surfaceNormal = hit.normal;
			myNormal = Vector3.Lerp (myNormal, surfaceNormal, lerpSpeed * Time.deltaTime);
			// find forward direction with new myNormal:
			Vector3 myForward = Vector3.Cross (myTransform.right, myNormal);
			// align character to the new myNormal while keeping the forward direction:
			Quaternion targetRot = Quaternion.LookRotation (myForward, myNormal);
			myTransform.rotation = Quaternion.Lerp (myTransform.rotation, targetRot, lerpSpeed * Time.deltaTime);
		} else {
			//isGrounded = false;
			// assume usual ground normal
			surfaceNormal = Vector3.up;
		}

	}
			

	// Find closes target
	/*GameObject FindClosestTarget ( ){
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
	}*/


	// Move towards target
	private void Move(){

		// Patrolling
		Vector3 target = targets[nowTarget].position;
		Vector3 dir = targets[nowTarget].position - myTransform.position;
		myTransform.LookAt (targets [nowTarget + 1]);
		myTransform.position = Vector3.MoveTowards (myTransform.position, target, moveSpeed * Time.deltaTime);
	
		//direction set
		//Vector3 direction = lastTarget.transform.position - myTransform.position;

		if (dir.magnitude < 1.5f) {
			if (timer == 0) {
				timer = Time.time;
			}
			if ((Time.time - timer) >= stopTimer) {
				nowTarget++;
				timer = 0;
			}
		}
	}

	private void OnTriggerStay (Collider other){
	
		Ray ray;
		RaycastHit hit;
		if (other.tag == "NowTarget") {
			Debug.Log ("Next target" + targets [nowTarget]);

			// JumpToWall
//			ray = new Ray (myTransform.position + myTransform.up * 0.5f, myTransform.forward);
//			Vector3 forward = transform.TransformDirection (Vector3.forward) * jumpRange; // ray debug	
//			Debug.DrawRay (transform.position, forward, Color.red); 					  // ray debug
//			if (Physics.Raycast (ray, out hit, jumpRange)) { // wall check
//				JumpToWall (hit.point, hit.normal); // init JumpToWall IF wall ahead
//				rigidbody.velocity += jumpSpeed * myNormal;
//			}
		}
	}

	/*private void JumpToWall(Vector3 point, Vector3 normal){
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
	}*/
}

