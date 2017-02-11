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
        }

    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 dir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>().GetDirection();
            Debug.Log(Vector3.Angle(transform.forward, dir));
            if (Vector3.Angle(transform.forward, dir) < 30)
            {
                if(!mCurrentUI) mCurrentUI = (GameObject)Instantiate(mEventUi);
                Vector3 position = other.gameObject.transform.position + (-Vector3.forward * 10);
                GameObject.Find("GravityDirection").GetComponent<GravityDirection>().SetDirection(-Vector3.up);
                other.gameObject.GetComponent<NormalMove>().Respawn(position, transform.up, -transform.forward);
                GameObject.Find("Camera").GetComponent<CameraManager>().CameraReset(); ;
            }
        }
    }
}
