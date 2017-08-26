using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public CharacterController controller;

	Vector3 moveDirection = Vector3.zero;

	public float gravity;
	public float speedZ;
	public float speedJump;

	// Use this for initialization
	void Start () {
		controller = GetComponent< CharacterController > ();
	}
	
	// Update is called once per frame
	void Update () {
		CharacterMove ();
	}

	void CharacterMove() {
		if (controller.isGrounded) {
			if (Input.GetAxis ("Vertical") > 0.0f) {
				moveDirection.z = Input.GetAxis ("Vertical") * speedZ;
			} else {
				moveDirection.z = 0;
			}

			transform.Rotate (0, Input.GetAxis ("Horizontal") * 3, 0);

			if (Input.GetButton ("Jump")) {
				moveDirection.y = speedJump;
			}

		}
			moveDirection.y -= gravity * Time.deltaTime;

			Vector3 globalDirection = transform.TransformDirection (moveDirection);
			controller.Move (globalDirection * Time.deltaTime);

			if (controller.isGrounded) {
				moveDirection.y = 0;
			}


	}


}
