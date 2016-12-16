using UnityEngine;
using System.Collections;

public class StageFinalClearCollider : MonoBehaviour 
{
    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Player")
        {
            coll.gameObject.GetComponent<PlayerMoveManager>().SetState(PlayerState.STAGE_FINAL_CLEAR);
            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Ending);
        }
    }	
}
