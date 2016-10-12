using UnityEngine;
using System.Collections;

public class scr_OtherGen : MonoBehaviour {

	public int blocksperlevel = 0;
	private int blockcounterX = 4; // 100 blocks
	private int blockcounterY = 4;
	private int randomnumber;
	private int blockCount = 100; // this needs to be the other number to blockcounter.


	public GameObject floor;
	public GameObject miner;

	// Use this for initialization
	void Start () {
		for (float xx = -4; xx < blockcounterX; xx+=0.16f) {
			for (float yy = -4; yy < blockcounterY; yy+=0.16f) {
				//randomnumber = Random.Range (0, 5);
				//if (randomnumber != 0) {
					//Instantiate (floor, new Vector3 (xx - (0.16f * Random.Range (-5,5)), yy  /*+  (0.16f * (Random.Range(1,4)))*/, 0), Quaternion.identity);
					Instantiate(floor,new Vector3(xx,yy,00), Quaternion.identity);
					blockCount--;
					
				//}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (blockCount <= 0) {
		//	Instantiate (miner, new Vector3 (0, -3.84f, 0), Quaternion.identity);
			Destroy (gameObject);
		}
	
	}
}
