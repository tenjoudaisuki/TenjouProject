using UnityEngine;
using System.Collections;

public class RedWall : MonoBehaviour
{

    GameObject mTarget;
    public GameObject mRedWall;
    public GameObject mEventUi;
    private GameObject mCurrentUI;

    public void Start()
    {
        mTarget = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        if (!mTarget) return;

        if (mTarget.GetComponent<PlayerMoveManager>().GetState() == PlayerState.STAGE_FINAL_CLEAR)
        {
            Destroy(mRedWall.GetComponent<BoxCollider>());
            Destroy(this);
        }

    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerMoveManager>().SetEventInputDisable(true);
            Vector3 dir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>().GetDirection();
            //Debug.Log(Vector3.Angle(transform.forward, dir));
            if (Vector3.Angle(transform.forward, dir) < 30)
            {
                if(!mCurrentUI) mCurrentUI = (GameObject)Instantiate(mEventUi);
                Vector3 position = other.gameObject.transform.position + (-Vector3.forward * 10);
                GameObject.Find("GravityDirection").GetComponent<GravityDirection>().SetDirection(-Vector3.forward);
                other.gameObject.GetComponent<NormalMove>().Respawn(position, Vector3.forward, Vector3.up);
                GameObject.Find("Camera").GetComponent<CameraManager>().CameraReset(); ;
            }
            else
            {
                if (!mCurrentUI) mCurrentUI = (GameObject)Instantiate(mEventUi);
                Vector3 position = other.gameObject.transform.position + (-Vector3.forward * 10) + (other.transform.up * 10);
                other.gameObject.GetComponent<NormalMove>().Respawn(other.transform.position + (-Vector3.forward), other.transform.up, -Vector3.forward);
                GameObject.Find("Camera").GetComponent<CameraManager>().CameraReset(); ;
            }
        }
    }
}
