/**==========================================================================*/
/**
 * プレイヤーの移動（丸まり時）
 * 作成者：守屋   作成日：16/11/05
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class RollMove : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private Rigidbody rb;
    private SphereCollider col;
    private Animator anm;
    private NormalMove m_NormalMove;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("重力の強さ")]
    private float m_GravityPower = 8.0f;

    /*==内部設定変数==*/
    //重力の方向
    private GravityDirection m_GravityDir;
    //プレイヤーが最後に移動した前と右の移動量
    private Vector3 m_LastMoveVelocity;

    /*==外部参照変数==*/

    void Awake()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        anm = GetComponent<Animator>();
        m_NormalMove = GetComponent<NormalMove>();
    }

    void Start()
    {
        //オブジェクト取得
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
    }

    void Update()
    {
        //丸まりボタンを押しているときは丸まり移動
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //押した瞬間の前と右の移動量を取得
            if (Input.GetKeyDown(KeyCode.LeftShift))
                m_LastMoveVelocity = m_NormalMove.GetMoveVelocity();

            //アニメーションをセット
            anm.SetBool("InputRoll", true);
            anm.SetBool("InputMove", false);
            anm.SetBool("InputJump", false);

            //当たり判定を有効にする
            col.enabled = true;
            //力を加える方向を計算
            Vector3 velocity = m_GravityDir.GetDirection() * m_GravityPower + m_LastMoveVelocity;
            //転がる
            rb.AddForce(velocity * m_GravityPower);
            //減速
            m_LastMoveVelocity *= 0.99f;
        }
        else
        {
            //アニメーションをセット
            anm.SetBool("InputRoll", false);
            //当たり判定を無効にする
            col.enabled = false;

            //地面にいるなら移動量初期化
            if (m_NormalMove.GetIsGroundHit())
                rb.velocity = Vector3.zero;
        }

    }
}
