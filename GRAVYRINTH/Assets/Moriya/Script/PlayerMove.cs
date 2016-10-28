/**==========================================================================*/
/**
 * プレイヤーの移動（通常時）
 * 歩く、斜面を歩く、段差を登る。
 * アニメーションも行う。
 * 重力の方向のセットもここで行う。
 * 作成者：守屋   作成日：16/10/14
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour 
{
    //当たったかどうか付のRaycastHit
    struct RayHitInfo
    {
        public RaycastHit hit;
        //当たったか？
        public bool isHit;
    };

    /*==所持コンポーネント==*/
    private Transform tr;
    private Rigidbody rb;
    private Animator m_Animator;


    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("移動速度")]
    private float m_MoveSpeed = 3.0f;
    [SerializeField, TooltipAttribute("身長(地面との判定を行うRayで使用)")]
    private float m_Height = 2.0f;
    [SerializeField, TooltipAttribute("斜面と認識する角度（壁と斜面の境界値）")]
    private float m_SlopeDeg = 45.0f;
    [SerializeField, TooltipAttribute("ジャンプ力")]
    private float m_JumpPower = 200.0f;
    [SerializeField, TooltipAttribute("重力の強さ")]
    private float m_GravityPower = 4.0f;



    /*==内部設定変数==*/
    //重力の方向を所持するクラス。プレイヤー以外で重力を扱う場合こちらのクラスを使用してください。
    private GravityDirection m_GravityDir;
    //プレイヤーモデルのトランスフォーム（子オブジェクト）
    private Transform m_ModelTr;
    //プレイヤーモデルのＹ軸回転量
    private float m_ModelRotateY;

    //カメラ
    private Transform m_Camera;

    //移動方向
    private Vector3 m_MoveVec;

    //プレイヤーの回転行列
    private Matrix4x4 m_RotateMatrix;


    Vector3 up;

    /*==外部参照変数==*/

    void Awake()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
    }

	void Start()
    {
        //オブジェクト取得
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        m_ModelTr = tr.FindChild("Model");
        m_Camera = Camera.main.transform;

        //値初期化
        m_ModelRotateY = 0.0f;
	}
	
	void Update() 
    {
        //移動処理
        Move();

        //重力の方向を設定
        m_GravityDir.SetDirection(GetDown());

        //ジャンプ処理
        //Jump();
    }

    /// <summary>
    /// 移動方向入力の取得
    /// </summary>
    private Vector2 GetMoveInputAxis()
    {
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        return direction;
    }

    /// <summary>
    /// 移動方向入力の取得
    /// </summary>
    private Vector2 GetMoveInputWASD()
    {
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) direction.y += 1.0f;
        if (Input.GetKey(KeyCode.S)) direction.y += -1.0f;
        if (Input.GetKey(KeyCode.A)) direction.x += -1.0f;
        if (Input.GetKey(KeyCode.D)) direction.x += 1.0f;
        return direction;
    }

    /// <summary>
    /// 移動方向入力を補正して返す
    /// </summary>
    private Vector2 MoveInputCorrection(Vector2 input)
    {
        Vector2 direction = input;
        //加速と減速をいい感じに補正
        if (direction != Vector2.zero)
        {
            float length = direction.magnitude;
            length = Mathf.Min(1, length);
            length = length * length;
            direction = direction.normalized * length;
        }
        return direction;
    }

    /// <summary>
    /// 現在の下方向を取得
    /// </summary>
    private Vector3 GetDown()
    {
        return Vector3.Normalize(-tr.up);
    }

    /// <summary>
    /// 現在向いている下方向にレイを飛ばし、ヒットした情報を返す。
    /// </summary>
    /// <param name="reyPos">レイを飛ばす位置</param>
    /// <returns></returns>
    private RayHitInfo CheckGroundHit(Vector3 reyPos)
    {
        RayHitInfo result;
        Ray ray = new Ray(reyPos, GetDown());
        RaycastHit hit;
        result.isHit = Physics.Raycast(ray, out hit, m_Height);
        result.hit = hit;
        
        //レイをデバッグ表示
        Debug.DrawRay(reyPos, GetDown() * m_Height, Color.grey, 1.0f, false);       

        return result;
    }

    /// <summary>
    /// 通常移動処理
    /// </summary>
    private void NormalMove()
    {
        //移動方向入力
        Vector2 inputVec = GetMoveInputAxis();

        //アニメーション
        m_Animator.SetBool("InputMove", inputVec.magnitude > 0.0f);

        ////入力方向からモデルの回転量を求める（上手くいかないっぽい。ほかの方法を考える。）
        //if (inputVec.magnitude > 0.1f)
        //{
        //    //入力方向を角度に変換
        //    m_ModelRotateY = Mathf.Atan2(-inputVec.y, inputVec.x) * Mathf.Rad2Deg;
        //    //カメラの向きとプレイヤーのデフォルト向きを加味して補正
        //    m_ModelRotateY += m_Camera.localEulerAngles.y + 90.0f;
        //    //回転
        //    Vector3 angles = m_ModelTr.localEulerAngles;
        //    angles.y = m_ModelRotateY;
        //    m_ModelTr.localEulerAngles = angles;
        //}

        //入力された値を移動用に補正
        Vector2 moveVec = MoveInputCorrection(inputVec);
        //進行方向である右方向と前方向を決定
        Vector3 right = m_Camera.transform.right;
        Vector3 front = -Vector3.Cross(tr.up, right);
        //移動方向を計算
        m_MoveVec = (front * moveVec.y + right * moveVec.x) * m_MoveSpeed;
        //移動
        tr.position += m_MoveVec * Time.deltaTime;


        //地面との判定
        RayHitInfo hitInfo = CheckGroundHit(tr.position + tr.up * m_Height);
        //重力で下方向に移動する
        Gravity(hitInfo.isHit);
        if (hitInfo.isHit)
        {
            //上方向と平面の法線方向のなす角
            float angle = Vector3.Angle(tr.up, hitInfo.hit.normal);
            //斜面として認識する角度以上なら何もしない
            if (angle > m_SlopeDeg) return;

            //当たった地点に移動
            tr.position = hitInfo.hit.point;

            //上方向を当たった平面の法線方向に変更
            up = hitInfo.hit.normal;

            //前
            Vector3 f = Vector3.Cross( up, m_Camera.right);
            tr.rotation = Quaternion.LookRotation(f, up);
        }


        //ジャンプ処理
        Jump(hitInfo.isHit);
    }

    /// <summary>
    /// 転がり移動処理
    /// </summary>
    private void RollMove()
    {
        //移動
        tr.position += m_MoveVec * Time.deltaTime;
        //減速
        m_MoveVec *= 0.99f;

        //仮重力
        //tr.position += GetDown() * 1.98f * Time.deltaTime;
        //Gravity();
        //地面との判定
        RayHitInfo hitInfo = CheckGroundHit(tr.position + tr.up * m_Height);
        if (hitInfo.isHit)
        {
            //当たった地点に移動
            tr.position = hitInfo.hit.point;
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //転がり移動
            m_Animator.SetBool("InputRoll", true);
            m_Animator.SetBool("InputMove", false);
            RollMove();
        }
        else
        {
            //通常移動
            m_Animator.SetBool("InputRoll", false);
            NormalMove();
        }
    }


    /// <summary>
    /// ジャンプ処理（小杉さんのタスク）
    /// </summary>
    private void Jump(bool isGround)
    {
        //RayHitInfo hitInfo = CheckGroundHit(tr.position + tr.up * m_Height);
        //if (hitInfo.isHit)
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        tr.GetComponent<Rigidbody>().AddForce(tr.up * m_JumpPower);
        //    }
        //}
        //Debug.Log(hitInfo.isHit);


        if (isGround && Input.GetKeyDown(KeyCode.Space))
            rb.AddForce(tr.up * m_JumpPower);
    }

    /// <summary>
    /// 重力（仮）
    /// </summary>
    private void Gravity(bool isGround)
    {
        //RayHitInfo hitInfo = CheckGroundHit(tr.position + tr.up * m_Height);
        //if (!hitInfo.isHit)
        //{
        //    tr.GetComponent<Rigidbody>().AddForce(GetDown() * m_GravityPower);
        //}
        //else
        //{
        //    tr.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //}
        if (isGround)
            rb.AddForce(GetDown() * m_GravityPower);
        else
            rb.velocity = Vector3.zero;
    }
}
