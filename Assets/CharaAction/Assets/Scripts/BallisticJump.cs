using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticJump : MonoBehaviour {

	public Transform target;
	Rigidbody rbody;

	public float angle = 15f;
	public float agroDis = 10f;
	public float speed = 5f;

	void Start(){
		rbody = GetComponent<Rigidbody> ();
		target = GameObject.FindGameObjectWithTag ("Player1").transform;
	}

	Vector3 BallisticVelocityVector(Transform target, float angle){
		Vector3 direction = target.position - transform.position;   // get target direction
		float h = direction.y;    									// get height difference
		direction.y = 0;                                            // remove height
		float distance = direction.magnitude;                       // get horizontal distance
		float a = angle * Mathf.Deg2Rad;                            // Convert angle to radians
		direction.y = distance * Mathf.Tan(a);                      // Set direction to elevation angle
		distance += h/Mathf.Tan(a);                                 // Correction for small height differences

		// calculate velocity
		float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2*a) * speed); // def 2
		return velocity * direction.normalized;
	}

	void OnTriggerEnter( Collider other ){
		if (other.tag == "Player1") {
			transform.LookAt (target);
			rbody.velocity = BallisticVelocityVector (target, angle);
		} else if (other.tag == "Player2") {
			transform.LookAt (target);
			rbody.velocity = BallisticVelocityVector (target, angle);
		}
	}
	/*
	void FixedUpdate(){
		if (Vector3.Distance (transform.position, target.transform.position) <= agroDis) {
			transform.LookAt (target);
			rbody.velocity = BallisticVelocityVector (target, angle);
		}
		Debug.Log ("Velocity:" + rbody.velocity);
	}*/
}
