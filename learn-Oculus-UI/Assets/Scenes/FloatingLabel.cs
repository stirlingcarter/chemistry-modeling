using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingLabel : MonoBehaviour {
    public GameObject protein;
    public float x_val;
    public float y_val;
    public float z_val;
    Vector3 labelOffset;

	void Start () {
        labelOffset = new Vector3(x_val, y_val, z_val);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = protein.transform.position + labelOffset;
	}
}