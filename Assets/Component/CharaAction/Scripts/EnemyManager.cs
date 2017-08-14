using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	public GameObject[] enemy;
	public Transform[] spawnPoint;

	void Start () {
		Spawn ();	
	}

	void Spawn(){
		enemy[0] = Instantiate (enemy[0], spawnPoint[0].transform.position, Quaternion.Euler(0,0,0)) as GameObject;
		enemy[1] = Instantiate (enemy[1], spawnPoint[1].transform.position, Quaternion.Euler(0,0,0)) as GameObject;
		enemy[2] = Instantiate (enemy[2], spawnPoint[2].transform.position, Quaternion.Euler(0,0,0)) as GameObject;
		enemy[3] = Instantiate (enemy[3], spawnPoint[3].transform.position, Quaternion.Euler(0,0,0)) as GameObject;
		enemy[4] = Instantiate (enemy[4], spawnPoint[4].transform.position, Quaternion.Euler(0,0,0)) as GameObject;
	}
}
