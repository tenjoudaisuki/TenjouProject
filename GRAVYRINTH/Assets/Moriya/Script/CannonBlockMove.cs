/**==========================================================================*/
/**
 * 最終ステージの大砲ギミックを完成させるためのブロックを押しているときの移動
 * 作成者：守屋   作成日：16/12/15
/**==========================================================================*/
using UnityEngine;
using System.Collections;

public class CannonBlockMove : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private Rigidbody rb;
    private Animator anm;
    private PlayerMoveManager m_MoveManager;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("重力の強さ")]
    private float m_GravityPower = 8.0f;
    [SerializeField, TooltipAttribute("身長")]
    private float m_Height = 0.5f;
    [SerializeField, TooltipAttribute("地面との判定のレイの長さ")]
    private float m_RayLength = 0.7f;
    [SerializeField, TooltipAttribute("掴むブロックの高さ")]
    private float m_BlockHeight = 0.5f;

    /*==内部設定変数==*/
    //上方向
    private Vector3 m_Up;
    //前方向
    private Vector3 m_Front;
    //地面とのヒット情報
    private RayHitInfo m_GroundHitInfo;
    //掴んでいるブロック
    private GameObject m_CannonBlock;

    void Start()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        anm = GetComponent<Animator>();
        m_MoveManager = GetComponent<PlayerMoveManager>();
    }

    void Update()
    {
        //重力
        if (!m_GroundHitInfo.isHit)
            rb.AddForce(-tr.up * m_GravityPower);
        else
            rb.velocity = Vector3.zero;

        //地面との判定
        Vector3 rayPos = tr.position + tr.up * m_Height;
        Ray ray = new Ray(rayPos, -tr.up);
        RaycastHit hit;
        //[IgnoredObj]レイヤー以外と判定させる
        int layermask = ~(1 << 10);
        m_GroundHitInfo.isHit = Physics.Raycast(ray, out hit, m_RayLength, layermask);
        m_GroundHitInfo.hit = hit;

        //ジャンプ関連の移動処理
        //地面と当たっていたら
        if (m_GroundHitInfo.isHit)
        {
            //当たった地点に移動
            tr.position = m_GroundHitInfo.hit.point;
            //上方向を当たった平面の法線方向に変更
            m_Up = m_GroundHitInfo.hit.normal.normalized;
        }
        else
        {
            tr.GetComponent<PlayerMoveManager>().SetState(PlayerState.NORMAL);
        }



        //前
        //m_Front = m_CannonBlock.transform.position - (tr.position + tr.up * m_BlockHeight);
        //m_Front.Normalize();
        m_Front = -m_CannonBlock.GetComponent<CannonBlock>().GetPlayerDirection().normal;
        //回転
        Quaternion rotate = Quaternion.LookRotation(m_Front, m_Up);
        tr.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 0.3f);
    }


    /// <summary>
    /// 現在掴んでいるキャノンブロックをセットする
    /// </summary>
    public void SetCannonBlockObject(GameObject obj)
    {
        m_CannonBlock = obj;
    }
}
