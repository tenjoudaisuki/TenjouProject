using UnityEngine;
using System.Collections;

public class CameraWorkFrag : MonoBehaviour
{

    public EndingCamera.Phase mPhase;
    public Vector3 offset;

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            var state = other.gameObject.GetComponent<PlayerMoveManager>().GetState();
            if(state == PlayerState.STAGE_FINAL_CLEAR)
            {
                GameObject.Find("Camera").GetComponent<EndingCamera>().PhaseChange(mPhase,offset);
            }
        }
    }
}
