using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

	public float turnSpeed = 4.0f;
	public Transform target;

	private Vector3 offset;

	void Start () {
		offset = new Vector3(target.position.x, target.position.y - 40.0f, target.position.z + 7.0f);
	}

	void LateUpdate()
	{
		offset = Quaternion.AngleAxis (Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
		transform.position = target.position + offset; 
		transform.LookAt(target.position);
	}
}
