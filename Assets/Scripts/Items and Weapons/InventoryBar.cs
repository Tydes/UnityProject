using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBar : MonoBehaviour {

    public Text itemName;
    public Button UseButton;
    public Button DropButton;


    /* Item Stuff */
    public string itemType;

    public Item item;         // the gameobject that's attached
    public string nameOfItem;       // Name of the object
    public string description;      // The description
    public GameObject gObject;      // The 3D object to rotate in the preview window





}
