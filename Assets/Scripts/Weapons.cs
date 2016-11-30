using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Weapons : MonoBehaviour {

	private int GridSize = 1;

	public bool Equipped = false;

	/* Weapon Stuff */
	public int ObjDamage = 0; 		// Amount of damage once hit
	public int ObjSpeed = 0; 		// The speed of the weapon swing. Must recharge when brought back
	public int ObjCritChance = 0; 	// Critical Hit chance, out of 100, the Higher the better

	public Sprite MainImage; 		// set in the inspector! for example: dagger object = dagger sprite.

	void Start () {

		/* Weapon Names */
		if (gameObject.name == "Dagger") {
			ObjDamage = 1;
			ObjSpeed = 1;
			ObjCritChance = 50; // 50/50
		}

		if (gameObject.name == "Katana") {
			ObjDamage = 3;
			ObjSpeed = 4;
			ObjCritChance = 15;
		}

		if (gameObject.name == "Sword") {
			ObjDamage = 4;
			ObjSpeed = 3;
			ObjCritChance = 10;
		}


	}
	
	void Update () {


		Vector3 currentPos = transform.position;
		if (Equipped == false) {
			transform.position = new Vector3 (Mathf.Floor (currentPos.x / GridSize) * GridSize, transform.position.y, Mathf.Floor (currentPos.z / GridSize) * GridSize);
		} else {
			transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		}



	}
}
