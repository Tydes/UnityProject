using UnityEngine;
using System.Collections;

public class scr_generator : MonoBehaviour {

	/* 											Created by @_Tydes - Give me a follow														*/
	/* 	You can do whatever you want with this! Just don't resell it or call it your own. You may use it in a game, just credit me please! 	*/
	/* 			This is very similar to how spelunky generates it's world. If you're going to thank anyone, Thank Derek.					*/



	/* INSTRUCTIONS */



	/* MAIN */


	public GameObject[] Rooms;			// the game objects
	public GameObject StarterBlock;		// the block you want to start with
	public GameObject player; 			// spawns the player
	public GameObject ToFollowObject; 	// the object that follows the player

	// variables 
	private int NumberOfRooms = 25;		// number of rooms
	private GameObject Room;			// What room is taken from the list
	private GameObject bRoomTrans; 		// transform stuff
	private GameObject beforeBlock;		// the block before
	private GameObject afterBlock;		// the block after

	// Puts the connectors in a list 
	private GameObject[] connectorList;
	private GameObject randomConnector;
	private GameObject selectedConnector;
	private GameObject spawnedRoom; // The room that is spawned;

	void Start () {

		beforeBlock = Instantiate (StarterBlock, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		// The room that is spawned;

		Instantiate(ToFollowObject,new Vector3 (transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
		Instantiate(player,new Vector3 (transform.position.x, transform.position.y+0.5f, transform.position.z), Quaternion.identity);


		for (int i = NumberOfRooms; i >= 0; i--) {
			// grabs a random block
			Room = Rooms [Random.Range (0, Rooms.Length)]; 

			// gets the before block connector
			bRoomTrans = beforeBlock.transform.FindChild ("connector").gameObject;

			// grabs the block that is spawning
			selectedConnector = Room.transform.FindChild ("connector").gameObject;

			//checks if it's grabbing the right object
			//Debug.Log (bRoomTrans.gameObject.tag);  

			// gets the width and height of the block
			float halfWidth = Room.GetComponent<Renderer> ().bounds.extents.x;
			float halfDepth = Room.GetComponent<Renderer> ().bounds.extents.z;

			afterBlock = Instantiate (Room, new Vector3 (bRoomTrans.transform.position.x, 0, bRoomTrans.transform.position.z + halfDepth), Quaternion.identity) as GameObject;

			beforeBlock = afterBlock;
		}

	}

}




