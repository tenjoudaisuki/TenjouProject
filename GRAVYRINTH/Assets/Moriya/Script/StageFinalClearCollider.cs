using UnityEngine;
using System.Collections;

public class StageFinalClearCollider : MonoBehaviour
{
    private CannonBlockManagar m_CannonManager;
    private GameObject m_Taihou;
    private GameObject m_StairUp;
    private GameObject m_StairRight;
    private GameObject m_StairLeht;
    private GameObject m_TaihouRingArrow1;
    private GameObject m_TaihouRingArrow2;
    private GameObject m_TaihouRingArrow3;

    private bool isClear = false;

    public float taihouAngle = -10.0f;
    public float taihouAngleTime = 3.0f;
    public float stairPullDownTime = 3.0f;

    void Start()
    {
        m_CannonManager = GameObject.Find("CannonBlockManagar").GetComponent<CannonBlockManagar>();

        m_Taihou = GameObject.FindGameObjectWithTag("Taihou");
        GetComponent<SphereCollider>().enabled = false;

        m_StairUp = GameObject.Find("f_stair (2)");
        m_StairRight = GameObject.Find("f_stair (3)");
        m_StairLeht = GameObject.Find("f_stair (1)");

        m_TaihouRingArrow1 = GameObject.FindGameObjectWithTag("Taihou").transform.FindChild("block1").transform.FindChild("f_taihou").transform.FindChild("f_taihouringarrow1").gameObject;
        m_TaihouRingArrow2 = GameObject.FindGameObjectWithTag("Taihou").transform.FindChild("block1").transform.FindChild("f_taihou").transform.FindChild("f_taihouringarrow2").gameObject;
        m_TaihouRingArrow3 = GameObject.FindGameObjectWithTag("Taihou").transform.FindChild("block1").transform.FindChild("f_taihou").transform.FindChild("f_taihouringarrow3").gameObject;

        //m_TaihouRingArrow1 = GameObject.Find("f_taihouringarrow1").gameObject;
        //m_TaihouRingArrow2 = GameObject.Find("f_taihouringarrow2").gameObject;
        //m_TaihouRingArrow3 = GameObject.Find("f_taihouringarrow3").gameObject;
    }

    void Update()
    {
        //クリアしたら
        if (isClear == false && m_CannonManager.GetIsSetAll() == true)
        {
            //当たり判定を有効にする
            GetComponent<SphereCollider>().enabled = true;
            //大砲を傾ける
            //m_Taihou.transform.rotation = Quaternion.Euler(new Vector3(-12, 0, 0));
            LeanTween.rotate(m_Taihou, new Vector3(taihouAngle, 0, 0), taihouAngleTime)
                .setEase(LeanTweenType.easeInOutBack);

            //上・右・左の階段をひっこめる
            LeanTween.move(m_StairUp, m_StairUp.transform.position + new Vector3(0, 2, 0), stairPullDownTime);
            LeanTween.move(m_StairRight, m_StairRight.transform.position + new Vector3(2, 0, 0), stairPullDownTime);
            LeanTween.move(m_StairLeht, m_StairLeht.transform.position + new Vector3(-2, 0, 0), stairPullDownTime);

            m_TaihouRingArrow1.active = true;
            m_TaihouRingArrow2.active = true;
            m_TaihouRingArrow3.active = true;

            isClear = true;
        }
    }


    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            coll.gameObject.GetComponent<PlayerMoveManager>().SetState(PlayerState.STAGE_FINAL_CLEAR);
            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Ending);
        }
    }
}
