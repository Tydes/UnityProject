using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class scr_UserInterface : MonoBehaviour {

	public Image wep1slot;
	public Image wep2slot;
	public Image weaponIcons;

	public Image wepslot1ui;
	public Image wepslot2ui;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		
		/* Ingame UI */
		if (GameObject.Find ("Player Object(Clone)") != null) {
			GameObject player = GameObject.Find ("Player Object(Clone)");
			scr_player plyscr = player.GetComponent<scr_player> ();

			if (plyscr.WeaponSlotSelected == 1) {
				wep1slot.transform.localScale = new Vector2 (0.6f, 0.6f);
				wep2slot.transform.localScale = new Vector2 (0.4f, 0.4f);
			} else {
				wep1slot.transform.localScale = new Vector2 (0.4f, 0.4f);
				wep2slot.transform.localScale = new Vector2 (0.6f, 0.6f);;
			}



			if (plyscr.WeaponPlacement.transform.childCount != 0) {
				wepslot1ui.sprite = plyscr.WeaponPlacement.GetComponentInChildren<Weapons> ().MainImage;
			//	wepslot1ui.material.color = new Color (255, 255, 255, 255);
			} else {
		//		wepslot1ui.material.color = new Color (0, 0, 0, 0);

			}


			if (plyscr.WeaponPlacement2.transform.childCount != 0) {
				wepslot2ui.sprite = plyscr.WeaponPlacement2.GetComponentInChildren<Weapons> ().MainImage;

		//		wepslot2ui.material.color = new Color (255, 255, 255, 255);
			} else {
		//		wepslot2ui.material.color = new Color (0, 0, 0, 0);
			}


				
		}










	}













}
