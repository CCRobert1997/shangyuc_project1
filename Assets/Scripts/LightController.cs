using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {


    public Terrain terrainmain;
    private Vector3 offset;
    private float xbound;
    private float zbound;
    private Vector3 terrainposition;
    private Vector3 terraincentre;
    private Vector3 lighttofloor;
    // Use this for initialization
    void Start () {
        xbound = terrainmain.terrainData.size.x;
        zbound = terrainmain.terrainData.size.z;
        terrainposition = terrainmain.transform.position;
        terraincentre = new Vector3(terrainposition.x + xbound/2, terrainposition.y, terrainposition.z + zbound/2);
        transform.position = terraincentre + new Vector3(0f, 2000f, 0f);
        transform.eulerAngles = new Vector3 (90f, 0f, 0f);


    }
	
	// Update is called once per frame
	
}
