using UnityEngine;
using System.Collections;

public class scr_rooms : MonoBehaviour {

	private int GridSize = 1;
	public GameObject chest;
	public GameObject chest2;
	public GameObject chest3;





	void Start()	{
		Vector3 currentPos = transform.position;
		transform.position = new Vector3 (Mathf.Floor (currentPos.x / GridSize) * GridSize, Mathf.Floor (currentPos.y / GridSize) * GridSize, Mathf.Floor (currentPos.z / GridSize) * GridSize);

		int randomChestNum = Random.Range (0, 10);

		if (randomChestNum <= 2) {
			Instantiate (chest, new Vector3 (transform.position.x, transform.position.y+1, transform.position.z), Quaternion.identity);
			Instantiate (chest2, new Vector3 (transform.position.x, transform.position.y+1, transform.position.z-2), Quaternion.identity);
			Instantiate (chest3, new Vector3 (transform.position.x, transform.position.y+1, transform.position.z-4), Quaternion.identity);
		}



	}








}
