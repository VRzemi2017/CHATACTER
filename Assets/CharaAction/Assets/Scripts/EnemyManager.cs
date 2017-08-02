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
	}
}
