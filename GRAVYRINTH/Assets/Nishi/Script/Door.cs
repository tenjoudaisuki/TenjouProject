using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public float mSpeed;
    float Rotatez = 0.0f;
	// Update is called once per frame
	void Update () {
        //Vector3 axis = transform.parent.forward;
        //Debug.Log(axis);
        //transform.Rotate(axis,mSpeed * Time.deltaTime);
        Rotatez += mSpeed * Time.deltaTime;
        Vector3 z = new Vector3(0, 0, Rotatez);
        transform.localEulerAngles = z;
	
	}
}
