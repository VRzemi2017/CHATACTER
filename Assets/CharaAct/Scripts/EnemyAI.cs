using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

	private float jumpSpeed = 10f;
	private float jumpRange = 5f;
	private float turnSpeed = 250f;
	private float lerpSpeed = 5f;

	private float distGround = 2.0f;
	private float deltaGround = 0.2f;
	private float gravityForce = 10f;
	private Vector3 surfaceNormal;
	private Vector3 myNormal;

	private bool isGrounded;
	private bool isJumping;

	private Transform myTransform;
	private Rigidbody rigidbody;
	public BoxCollider boxCollider; 

	public Transform target; 

	void Start(){
		myNormal = transform.up;
		myTransform = transform;
		distGround = boxCollider.size.y - boxCollider.center.y;
		rigidbody = GetComponent<Rigidbody> ();
		isGrounded = true;
	}

	void FixedUpdate(){
		rigidbody.AddForce(-gravityForce*rigidbody.mass*myNormal);
	}

	void Update(){
		if (isJumping) {
			return;
		}

		Ray ray;
		RaycastHit hit;

		float step = jumpSpeed * Time.deltaTime;
		if (isGrounded == true) {
			myTransform.position = Vector3.MoveTowards (transform.position, target.position, step);
			ray = new Ray (myTransform.position, myTransform.forward);
		}
	}
}
