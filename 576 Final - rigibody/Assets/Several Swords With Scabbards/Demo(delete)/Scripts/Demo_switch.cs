using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_switch : MonoBehaviour {
	public GameObject[] Sword;
	public bool act;
	public int sword_numd=0;
	public bool act1;
	void Update () {
				if (act == true) {
			
				Sword [sword_numd].SetActive (false);
				sword_numd += 1;
				Sword [sword_numd].SetActive (true);
				act = false;
		}
		if (act1 == true) {
			
				Sword [sword_numd].SetActive (false);
				sword_numd -= 1;
				Sword [sword_numd].SetActive (true);
				act1 = false;
			}
			
		if (Input.GetKeyDown (KeyCode.E)) {
			if (sword_numd < 9) {
				act = true;
			}

		}
		if (Input.GetKeyDown (KeyCode.Q)) {
			if (sword_numd > 0) {
				act1 = true;
			}
		}
		
	}
}
