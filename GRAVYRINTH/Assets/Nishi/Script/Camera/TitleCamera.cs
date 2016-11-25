using UnityEngine;
using System.Collections;

public class TitleCamera : MonoBehaviour {

    public GameObject mStageCenter;
    public float mSpeed;
    public float mDistance = 5.0f;
    public Vector3 mOffset;

    bool isStart = false;
    Vector3 mFromPos;
    float mTimer = 0.0f;

	// Use this for initialization
	void Start ()
    {
        isStart = false;
        transform.position = (mStageCenter.transform.forward * -mDistance) + mOffset;
        transform.LookAt(mStageCenter.transform);
	}
	
	// Update is called once per frame
	void Update ()
    {
            transform.RotateAround(mStageCenter.transform.position, Vector3.up, mSpeed);
            if (Input.GetKeyDown(KeyCode.C))
            {
                GetComponent<CameraManager>().StateChange(State.Select);
            }

        //if(isStart)
        //{
        //    if (mTimer >= 1.2f)
        //    {
        //        GetComponent<CameraControl>().enabled = true;
        //        this.enabled = false;
        //    }
        //    mTimer += Time.deltaTime;
        //    GameObject player = GameObject.Find("Player");
        //    transform.LookAt(player.transform);
        //    Vector3 nextPos = player.transform.position + (player.transform.forward * -2) + new Vector3(0,0.4f,0);
        //    Debug.Log(nextPos);
        //    transform.position = Vector3.Lerp(mFromPos,nextPos,mTimer);
        //}
	}
}
