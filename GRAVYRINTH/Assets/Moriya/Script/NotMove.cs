/**==========================================================================*/
/**
 * プレイヤーの移動（地面にくっついて移動するのみ、実質何もしない）
 * 作成者：守屋   作成日：16/01/22
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class NotMove : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private Rigidbody rb;
    private Animator anm;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("身長")]
    private float m_Height = 0.65f;
    [SerializeField, TooltipAttribute("重力の強さ")]
    private float m_GravityPower = 8.0f;
    [SerializeField, TooltipAttribute("地面との判定のレイの長さ")]
    private float m_RayLength = 0.7f;

    /*==内部設定変数==*/
    //地面とのヒット情報
    private RayHitInfo m_GroundHitInfo;


    /*==外部参照変数==*/

    void Awake()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        anm = GetComponent<Animator>();
    }

    void Start()
    {
        //rigidbodyによるrotateの変更のみを封じる
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        //地面との判定
        CheckGroundHit();
    }

    void Update()
    {
        //地面との判定
        CheckGroundHit();

        if (!m_GroundHitInfo.isHit) return;

        //当たった地点に移動
        tr.position = m_GroundHitInfo.hit.point;
        //上方向を当たった平面の法線方向に変更
        SetUpFront(m_GroundHitInfo.hit.normal.normalized, tr.forward);
        //tr.up = m_GroundHitInfo.hit.normal.normalized;

        //ヒットした相手のトランスフォーム
        Transform hitTr = m_GroundHitInfo.hit.transform;
        //回転床と当たっているなら
        if (hitTr.tag == "SpinChild")
        {
            //床の移動方向に移動
            Vector3 movement = hitTr.parent.gameObject.GetComponent<SpinChild>().GetMovement();
            tr.position += movement;
        }
    }

    void FixedUpdate()
    {
        //重力で下方向に移動する
        Gravity();
    }

    void LateUpdate()
    {

    }


    /// <summary>
    /// 現在の下方向を取得
    /// </summary>
    private Vector3 GetDown()
    {
        return Vector3.Normalize(-tr.up);
    }

    /// <summary>
    /// 重力で下に移動する
    /// </summary>
    private void Gravity()
    {
        //地面にいないときは重力をかける
        if (!m_GroundHitInfo.isHit)
        {
            rb.AddForce(GetDown() * m_GravityPower);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 現在向いている下方向にレイを飛ばし、ヒットした情報をm_GroundHitInfoに入れる。
    /// </summary>
    private void CheckGroundHit()
    {
        Vector3 rayPos = tr.position + tr.up * m_Height;
        Ray ray = new Ray(rayPos, -tr.up);
        RaycastHit hit;
        //[IgnoredObj][IronBar][Player]レイヤー以外と判定させる
        int layermask = ~(1 << 10 | 1 << LayerMask.NameToLayer("IronBar") | 1 << LayerMask.NameToLayer("Player"));
        m_GroundHitInfo.isHit = Physics.Raycast(ray, out hit, m_RayLength, layermask, QueryTriggerInteraction.Ignore);
        m_GroundHitInfo.hit = hit;
    }

    /// <summary>
    /// 向きを更新
    /// </summary>
    public void SetUpFront(Vector3 up, Vector3 front)
    {
        //向きを変更
        Quaternion rotate = Quaternion.LookRotation(front, up);
        tr.localRotation = rotate;
    }

    /// <summary>
    /// 座標と向きを指定してリスポーンする
    /// </summary>
    public void Respawn(Vector3 position, Vector3 up, Vector3 front)
    {
        tr.position = position;
        SetUpFront(up, front);
    }

}
