using UnityEngine;
using System.Collections;

public class StageFinalClearCollider : MonoBehaviour 
{
    private GameObject m_Taihou;

    void Start()
    {
        m_Taihou = GameObject.FindGameObjectWithTag("Taihou");
        GetComponent<SphereCollider>().enabled = false;
    }

    void Update()
    {
        //クリアしたら
        if(Input.GetKeyDown(KeyCode.N))
        {
            //当たり判定を有効にする
            GetComponent<SphereCollider>().enabled = true;
            //大砲を傾ける
            m_Taihou.transform.rotation = Quaternion.Euler( new Vector3(-12, 0, 0));
        }
    }


    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Player")
        {
            coll.gameObject.GetComponent<PlayerMoveManager>().SetState(PlayerState.STAGE_FINAL_CLEAR);
            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Ending);
        }
    }	
}
