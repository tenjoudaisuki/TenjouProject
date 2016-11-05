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
    [SerializeField, TooltipAttribute("身長")]
    private float m_Height = 2.0f;
    [SerializeField, TooltipAttribute("斜面と認識する角度（壁と斜面の境界値）")]
    private float m_SlopeDeg = 45.0f;
    [SerializeField, TooltipAttribute("ジャンプ力")]
    private float m_JumpPower = 200.0f;
    [SerializeField, TooltipAttribute("重力の強さ")]
    private float m_GravityPower = 4.0f;
    [SerializeField, TooltipAttribute("地面との判定のレイの長さ")]
    private float m_RayLength = 4.0f;
    [SerializeField, TooltipAttribute("ジャンプ後の地面と判定を行わない時間の長さ")]
    private float m_JumpedTime = 1.0f;

    /*==内部設定変数==*/
    //重力の方向を所持するクラス。プレイヤー以外で重力を扱う場合こちらのクラスを使用してください。
    private GravityDirection m_GravityDir;
    //カメラ
    private Transform m_Camera;
    //移動方向
    private Vector3 m_MoveVec;
    //地面とのヒット情報
    private RayHitInfo m_GroundHitInfo;
    //地面との判定を行うか？
    private bool m_IsCheckGround = true;
    //ジャンプしてからの経過時間を計るタイマー
    private float m_JumpedTimer = 0.0f;
    //1フレーム前の地面と当たっているかの結果(現在のフレームはm_GroundHitInfo.isHit)
    private bool m_IsPrevGroundHit = false;
    //地面と当たった瞬間かどうか？
    private bool m_IsGroundHitTrigger = false;


    Vector3 up = Vector3.up;
    float y;
    private Vector3 front = Vector3.forward;

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
        m_Camera = Camera.main.transform;

        //地面との判定
        CheckGroundHit();
	}
	
	void Update() 
    {
        //地面との判定
        if (m_IsCheckGround)//ジャンプ直後は判定しない
            CheckGroundHit();

        //地面にヒットした瞬間かどうかを判定
        if (!m_IsPrevGroundHit && m_GroundHitInfo.isHit)
            m_IsGroundHitTrigger = true;
        else
            m_IsGroundHitTrigger = false;
        //1フレーム前の情報として使うために渡す
        m_IsPrevGroundHit = m_GroundHitInfo.isHit;

        //移動処理
        Move();            
    }

    void LateUpdate()
    {
        print(m_IsGroundHitTrigger);
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
            m_Animator.SetBool("InputJump", false);
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
    /// 現在向いている下方向にレイを飛ばし、ヒットした情報をm_GroundHitInfoに入れる。
    /// </summary>
    private void CheckGroundHit()
    {
        Vector3 rayPos = tr.position + tr.up * m_Height;
        Ray ray = new Ray(rayPos, GetDown());
        RaycastHit hit;
        m_GroundHitInfo.isHit = Physics.Raycast(ray, out hit, m_RayLength);
        m_GroundHitInfo.hit = hit;
        
        //レイをデバッグ表示
        Debug.DrawRay(rayPos, GetDown() * m_RayLength, Color.grey, 1.0f, false);       
    }



    /// <summary>
    /// 地面の移動処理
    /// </summary>
    private void NormalMove()
    {
        //地面と当たっていたら
        if (m_GroundHitInfo.isHit)
        {
            //上方向と平面の法線方向のなす角
            float angle = Vector3.Angle(tr.up, m_GroundHitInfo.hit.normal);
            //斜面として認識する角度以上なら何もしない
            if (angle > m_SlopeDeg) return;
            //当たった地点に移動
            tr.position = m_GroundHitInfo.hit.point;
            //上方向を当たった平面の法線方向に変更
            up = m_GroundHitInfo.hit.normal;
        }

        //移動方向入力
        Vector2 inputVec = GetMoveInputAxis();
        //アニメーション変更
        m_Animator.SetBool("InputMove", inputVec.magnitude > 0.0f);
        //スティックが入力されたら向きを変える
        if (inputVec.magnitude > 0.1f)
        {
            //スティックの傾きを↑を0°として計算
            y = Vector2.Angle(Vector2.up, inputVec);
            //ステイックが左に傾いていればyをマイナスに
            if (inputVec.x < 0) y = -y;
        }
        //地面の上方向とカメラの右方向で外積を取得
        Vector3 camerafoward = -Vector3.Cross(up, m_Camera.right);
        //外積をスティックの角度で回転させて前ベクトルを計算
        front = Quaternion.AngleAxis(y, up) * camerafoward;

        //プレイヤーの前ベクトルと上ベクトルを決定
        Quaternion rotate = Quaternion.LookRotation(front, up);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 0.3f);
        //前ベクトル×スティックの傾き
        m_MoveVec = (tr.forward * inputVec.magnitude) * m_MoveSpeed;
        //移動
        tr.position += m_MoveVec * Time.deltaTime;

        //重力で下方向に移動する
        Gravity();
        //ジャンプ処理
        Jump();
    }

    ///// <summary>
    ///// 通常移動
    ///// 作成　西
    ///// </summary>
    //private void NormalMove2()
    //{
    //    //移動方向入力
    //    Vector2 inputVec = GetMoveInputAxis();
    //    //Debug.Log(inputVec);

    //    //アニメーション
    //    m_Animator.SetBool("InputMove", inputVec.magnitude > 0.0f);

    //    //地面との判定
    //    RayHitInfo hitInfo = CheckGroundHit(tr.position + tr.up * m_Height);
    //    //重力で下方向に移動する
    //    Gravity();
    //    if (hitInfo.isHit)
    //    {
    //        //上方向と平面の法線方向のなす角
    //        float angle = Vector3.Angle(tr.up, hitInfo.hit.normal);
    //        //斜面として認識する角度以上なら何もしない
    //        if (angle > m_SlopeDeg) return;

    //        //当たった地点に移動
    //        tr.position = hitInfo.hit.point;

    //        //上方向を当たった平面の法線方向に変更
    //        up = hitInfo.hit.normal;
    //    }

    //    //スティックが入力されたら向きを変える
    //    if (inputVec.magnitude > 0.1f)
    //    {
    //        //スティックの傾きを↑を0°として計算
    //        y = Vector2.Angle(Vector2.up, inputVec);
    //        //ステイックが左に傾いていればyをマイナスに
    //        if (inputVec.x < 0) y = -y;
    //    }

    //    //地面の上方向とカメラの右方向で外積を取得
    //    Vector3 camerafoward = -Vector3.Cross(up, m_Camera.right);
    //    //外積をスティックの角度で回転させて前ベクトルを計算
    //    front = Quaternion.AngleAxis(y, up) * camerafoward;

    //    //プレイヤーの前ベクトルと上ベクトルを決定
    //    //tr.localRotation = Quaternion.LookRotation(front, up);
    //    Quaternion rotate = Quaternion.LookRotation(front, up);
    //    transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 0.3f);

    //    //前ベクトル×スティックの傾き
    //    m_MoveVec = (tr.forward * inputVec.magnitude) * m_MoveSpeed;
    //    //移動
    //    tr.position += m_MoveVec * Time.deltaTime;
    //    //ジャンプ処理
    //    Jump();
    //}

    /// <summary>
    /// 転がり移動処理
    /// </summary>
    private void RollMove()
    {
        //移動
        tr.position += m_MoveVec * Time.deltaTime;
        //減速
        m_MoveVec *= 0.99f;
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    private void Jump()
    {
        //地面にいるときのジャンプ始動処理
        if (m_GroundHitInfo.isHit && Input.GetKeyDown(KeyCode.Space))
        {
            //アニメーションの設定
            m_Animator.SetBool("InputJump", true);
            //力を加えてジャンプ
            rb.AddForce(tr.up * m_JumpPower);
            //しばらく地面との判定を行わない
            m_IsCheckGround = false;
            //判定の結果も切っておく
            m_GroundHitInfo.isHit = false;
            //タイマーも初期化
            m_JumpedTimer = 0.0f;
        }
        //ジャンプした直後の、地面と判定させないための処理
        else if (!m_IsCheckGround)
        {
            //指定時間が経過したら地面との判定を再開
            m_JumpedTimer += Time.deltaTime;
            if (m_JumpedTimer > m_JumpedTime)
            {
                m_JumpedTimer = 0.0f;
                m_IsCheckGround = true;
            }
        }
    }

    /// <summary>
    /// 重力（仮）
    /// </summary>
    private void Gravity()
    {
        //地面にいないときは重力をかける
        if (!m_GroundHitInfo.isHit)
            rb.AddForce(GetDown() * m_GravityPower);
        else
            rb.velocity = Vector3.zero;
    }
}
