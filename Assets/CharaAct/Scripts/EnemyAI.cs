using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum enemyState{
	WAIT,
	JUMP,
	ATTACK
}

public class EnemyAI : MonoBehaviour {

	enemyState myEnemyState;

	private float moveSpeed = 5; // move forward speed
	private float turnSpeed = 90; // turning speed (degrees/second)
	private float lerpSpeed = 10; // smoothing speed
	private float gravity = 20; // gravity acceleration

	private bool isGrounded;
	private float deltaGround = 0.2f; // character is grounded up to this distance
	private float jumpSpeed = 2.5f; // vertical jump initial speed
	private float jumpRange = 1.5f; // range to detect target wall

	private float timer = 0.0f; // pause between jumps

	private Vector3 surfaceNormal; // current surface normal
	private Vector3 myNormal; // character normal
	private float distGround; // distance from character position to ground

	private bool jumping = false; 
	//private float vertSpeed = 0; // vertical jump current speed

	private Transform myTransform;
	public BoxCollider boxCollider; 
	Rigidbody rigidbody;

	public Transform target; 

	private void Start( ){
		rigidbody = GetComponent<Rigidbody> ();
		myNormal = transform.up; // normal starts as character up direction
		myTransform = transform;
		rigidbody.freezeRotation = true; // disable physics rotation
		// distance from transform.position to ground
		distGround = boxCollider.extents.y - boxCollider.center.y;
	}

	private void FixedUpdate(){
		// apply constant weight force according to character normal:
		rigidbody.AddForce(-gravity*rigidbody.mass*myNormal);
	}

	// UPDATE //

	private void Update(){

		// AI CONTROL START //

		switch (myEnemyState) {
		case enemyState.WAIT:
			//Search for target
			myTransform.LookAt (target);

			if (target != null) {
				myEnemyState = enemyState.JUMP;
			}
			break;

		case enemyState.JUMP:
			
			//Raycast
			Ray ray;
			RaycastHit hit;

			//Jump to target
			Vector3 localPos = target.transform.position - myTransform.position; // Target local position
			localPos = localPos.normalized;
	
			//Move towards target
			myTransform.Translate (localPos.x * Time.deltaTime * moveSpeed, localPos.y * Time.deltaTime * moveSpeed, localPos.z * Time.deltaTime * moveSpeed);
			
			//Shoots ray from front to detect walls
			ray = new Ray (myTransform.position, myTransform.forward);
			Debug.DrawRay (myTransform.position, myTransform.forward);
			if (Physics.Raycast (ray, out hit, jumpRange) ) { // wall ahead?
				JumpToWall (hit.point, hit.normal); // yes: jump to the wall
			} else if (isGrounded) { // no: if grounded, jump up
				rigidbody.velocity += jumpSpeed * myNormal;
		  		myTransform.LookAt (target);
			  }

			//Shoots ray down to detect floor
			ray = new Ray(myTransform.position, -myNormal);
			if (Physics.Raycast(ray, out hit)){ // use it to update myNormal and isGrounded
				isGrounded = hit.distance <= distGround + deltaGround;
				surfaceNormal = hit.normal;
			} else {
				isGrounded = false;
				// assume usual ground normal to avoid "falling forever"
				surfaceNormal = Vector3.up;
			}

			//rotate self to new myNormal;
			myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed*Time.deltaTime);
			// find forward direction with new myNormal:
			Vector3 myForward = Vector3.Cross(myTransform.right, myNormal);
			// align character to the new myNormal while keeping the forward direction:
			Quaternion targetRot = Quaternion.LookRotation(myForward, myNormal);
			myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRot, lerpSpeed*Time.deltaTime);

			//if(target within range){ enemyState.ATTACK }
			break;
		
		
		case enemyState.ATTACK:
			//attack player
			//if(target out of range) { enemyState.JUMP }
			break;
		}

		// AI CONTROL END //

		// jump code - jump to wall or simple jump
		if (jumping) {
			return;
		} // abort Update while jumping to a wall


		// MANUAL CONTROL START
		/*
		Ray ray;
		RaycastHit hit;

		if (Input.GetButtonDown("Jump")){ // jump pressed:
			ray = new Ray(myTransform.position, myTransform.forward);
			if (Physics.Raycast(ray, out hit, jumpRange)){ // wall ahead?
				JumpToWall(hit.point, hit.normal); // yes: jump to the wall
			}
			else if (isGrounded){ // no: if grounded, jump up
				rigidbody.velocity += jumpSpeed * myNormal;
			}
		}

		// movement code - turn left/right with Horizontal axis:
		myTransform.Rotate(0, Input.GetAxis("Horizontal")*turnSpeed*Time.deltaTime, 0);
		// update surface normal and isGrounded:
		ray = new Ray(myTransform.position, -myNormal); // cast ray downwards
		if (Physics.Raycast(ray, out hit)){ // use it to update myNormal and isGrounded
			isGrounded = hit.distance <= distGround + deltaGround;
			surfaceNormal = hit.normal;
		}
		else {
			isGrounded = false;
			// assume usual ground normal to avoid "falling forever"
			surfaceNormal = Vector3.up;
		}
		myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed*Time.deltaTime);
		// find forward direction with new myNormal:
		Vector3 myForward = Vector3.Cross(myTransform.right, myNormal);
		// align character to the new myNormal while keeping the forward direction:
		Quaternion targetRot = Quaternion.LookRotation(myForward, myNormal);
		myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRot, lerpSpeed*Time.deltaTime);
		// move the character forth/back with Vertical axis:
		myTransform.Translate(0, 0, Input.GetAxis("Vertical")*moveSpeed*Time.deltaTime);*/

		//MANUAL CONTROL END
	}
		

	private void JumpToWall(Vector3 point, Vector3 normal){
		// jump to wall
		jumping = true; // signal it's jumping to wall
		rigidbody.isKinematic = true; // disable physics while jumping
		Vector3 orgPos = myTransform.position;
		Quaternion orgRot = myTransform.rotation;
		Vector3 dstPos = point + normal * (distGround + 0.5f); // will jump to 0.5 above wall
		Vector3 myForward = Vector3.Cross(myTransform.right, normal);
		Quaternion dstRot = Quaternion.LookRotation(myForward, normal);

		StartCoroutine (jumpTime (orgPos, orgRot, dstPos, dstRot, normal));
		//jumptime
	}

	private IEnumerator jumpTime(Vector3 orgPos, Quaternion orgRot, Vector3 dstPos, Quaternion dstRot, Vector3 normal) {
		for ( float t = 0.0f; t < 2.0f; ){
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

