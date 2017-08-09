using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	public float speed = 5.0f;
	public float upF = 10.0f;
	public float downF = 10.0f;

	/*void Start () {
		Cursor.lockState = CursorLockMode.Locked;
	}*/
	

	void Update () {
		float forward = Input.GetAxis ("Vertical") * Time.deltaTime * speed;
		float strafe = Input.GetAxis ("Horizontal") * Time.deltaTime *  speed;
		transform.Translate (strafe, 0, forward);

		var up = Input.GetAxis ("Jump") * Time.deltaTime * upF;
		var down = Input.GetAxis ("Crouch") * Time.deltaTime * downF;

		transform.Translate (0, up, 0);
		transform.Translate (0, -down, 0);

		/*if(Input.GetKeyDown(KeyCode.Escape)){
			Cursor.lockState = CursorLockMode.None;
		}*/
	}
}
