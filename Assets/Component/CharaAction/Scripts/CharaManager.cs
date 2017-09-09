using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaManager : MonoBehaviour {

	private GameObject[] enemy;

	public int damage = 1;


	public void SetEnemyActive ( int index, bool active ) {
		enemy [index].SetActive (active);
	}

	public void SetAllEnemyActive( bool active ) {
		foreach( GameObject enemyUnit in enemy ){
			enemyUnit.SetActive (active);
		}
	}
}
