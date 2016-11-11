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
            Vector3 velocity = m_GravityDir.GetDirection() * 5.0f + m_LastMoveVelocity;
            //転がる
            rb.AddForce(velocity);
            //減速
            m_LastMoveVelocity *= 0.99f;
        }
        else
        {
            //当たり判定を無効にする
            col.enabled = false;

            //rb.velocity = Vector3.zero;
        }

    }

    void LateUpdate()
    {
        print(rb.velocity);
    }
}
