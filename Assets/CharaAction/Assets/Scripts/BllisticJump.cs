using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BllisticJump : MonoBehaviour {

	public Transform target;
	GameObject enemy;
	Rigidbody rbody;

	public float angle = 15f;
	public float agroDis = 15f;
	public float speed = 5f;

	void Start(){
		rbody = GetComponent<Rigidbody> ();
		this.enemy = GameObject.Find ("Enemy");
		target = GameObject.FindGameObjectWithTag ("Player").transform;
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

	void FixedUpdate(){
		if (Vector3.Distance (transform.position, target.transform.position) <= agroDis) {
			transform.LookAt (target);
			rbody.velocity = BallisticVelocityVector (target, angle);
		}
		Debug.Log ("Velocity:" + rbody.velocity);
	}
}
