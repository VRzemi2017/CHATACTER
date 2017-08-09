using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibro : MonoBehaviour {

	SteamVR_TrackedObject trackedObj;
	SteamVR_Controller.Device device;

	void Awake(){
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	void FixedUpdate(){
		device = SteamVR_Controller.Input ((int)trackedObj.index);
	}

	void Update(){
		rumbleController ();
	}

	public void rumbleController(){

		//if ( device.GetPressDown (SteamVR_Controller.ButtonMask.Grip)){
			StartCoroutine(LongVibration(1, 3999 ));
		//}
	}

	IEnumerator LongVibration( float length, float strenght ){
		for (float i = 0; i < length; i += Time.deltaTime) {
			device.TriggerHapticPulse ((ushort)Mathf.Lerp (0, 3999, strenght));
			yield return null;
		}
	}

}
