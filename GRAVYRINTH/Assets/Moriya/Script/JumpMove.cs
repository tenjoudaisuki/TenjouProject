///**==========================================================================*/
///**
// * プレイヤーの移動（ジャンプ時）
// * 作成者：守屋   作成日：16/11/15
///**==========================================================================*/

//using UnityEngine;
//using System.Collections;

//public class JumpMove : MonoBehaviour 
//{
//    /*==所持コンポーネント==*/
//    private Transform tr;
//    private Rigidbody rb;
//    private Animator anm;
//    //プレイヤーの状態管理クラス
//    private PlayerMoveManager m_MoveManager;

//    /*==外部設定変数==*/
//    [SerializeField, TooltipAttribute("移動速度")]
//    private float m_MoveSpeed = 3.0f;
//    [SerializeField, TooltipAttribute("身長")]
//    private float m_Height = 0.5f;
//    [SerializeField, TooltipAttribute("斜面と認識する角度（壁と斜面の境界値）")]
//    private float m_SlopeDeg = 45.0f;
//    [SerializeField, TooltipAttribute("ジャンプ力")]
//    private float m_JumpPower = 200.0f;
//    [SerializeField, TooltipAttribute("重力の強さ")]
//    private float m_GravityPower = 8.0f;
//    [SerializeField, TooltipAttribute("地面との判定のレイの長さ")]
//    private float m_RayLength = 0.7f;
//    [SerializeField, TooltipAttribute("プレイヤー正面と壁との判定のレイの長さ")]
//    private float m_WallRayLength = 4.0f;
//    [SerializeField, TooltipAttribute("ジャンプ後の地面と判定を行わない時間の長さ")]
//    private float m_JumpedTime = 1.0f;
//    [SerializeField, TooltipAttribute("アニメーション再生速度")]
//    private float m_AnimSpeed = 1.5f;


//    void Start()
//    {
	    
//    }
	
//    void Update() 
//    {
	    
//    }



//    ///// <summary>
//    ///// ジャンプ処理
//    ///// </summary>
//    //private void Jump()
//    //{
//    //    //地面にいるときのジャンプ始動処理
//    //    if (m_GroundHitInfo.isHit && Input.GetKeyDown(KeyCode.Space))
//    //    {
//    //        //アニメーションの設定
//    //        anm.SetBool("InputJump", true);
//    //        //力を加えてジャンプ
//    //        rb.AddForce(tr.up * m_JumpPower);
//    //        //一定時間経過まで地面との判定を行わない
//    //        m_IsCheckGround = false;
//    //        //判定の結果も切っておく
//    //        m_GroundHitInfo.isHit = false;
//    //        //タイマーも初期化
//    //        m_JumpedTimer = 0.0f;
//    //    }
//    //}

//    ///// <summary>
//    ///// 重力
//    ///// </summary>
//    //private void Gravity()
//    //{
//    //    //地面にいないときは重力をかける
//    //    if (!m_GroundHitInfo.isHit)
//    //        rb.AddForce(GetDown() * m_GravityPower);
//    //    else
//    //        rb.velocity = Vector3.zero;
//    //}
//}
