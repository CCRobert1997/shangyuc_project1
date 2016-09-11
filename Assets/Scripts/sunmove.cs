using UnityEngine;
using System.Collections;

public class sunmove : MonoBehaviour {

	// Use this for initialization
	
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(new Vector3(0f, 0f, 10f) * Time.deltaTime);
	}
}
