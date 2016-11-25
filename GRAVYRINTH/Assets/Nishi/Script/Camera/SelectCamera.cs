using UnityEngine;
using System.Collections;

public class SelectCamera : MonoBehaviour {

    GameObject mCameraPosition;
    Vector3 mFromPos;
    Quaternion mFromRotate;

    float mTimer = 0.0f;

	// Use this for initialization
	void Start ()
    {
        mTimer = 0.0f;
        mCameraPosition = GameObject.Find("SelectCameraPosition");
        mFromPos = transform.position;
        mFromRotate = transform.localRotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        mTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(mFromPos, mCameraPosition.transform.position, mTimer);
        transform.localRotation = Quaternion.Slerp(mFromRotate, mCameraPosition.transform.localRotation, mTimer);
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetComponent<CameraManager>().StateChange(State.GamePlay);
        }

    }
}
