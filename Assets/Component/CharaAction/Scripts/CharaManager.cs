using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaManager : MonoBehaviour {

	private GameObject[] enemy;

	void Start(){
		
	}

	public void SetEnemyActive ( int index, bool active ) {
		enemy [index].SetActive (active);
	}

	public void SetAllEnemyActive( bool active ) {
		foreach( GameObject enemyUnit in enemy ){
			enemyUnit.SetActive (active);
		}
	}
}
