using UnityEngine;
using System.Collections;

public class SpinParent : MonoBehaviour {
    public Vector3 mAxis = Vector3.up;
    public float mSpinSpeed;
	void Update ()
    {
        gameObject.transform.Rotate(mAxis, mSpinSpeed);
	}
}
