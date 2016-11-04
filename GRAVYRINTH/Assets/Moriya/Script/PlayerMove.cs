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
    //地面とのヒット情報
    RayHitInfo m_GroundHitInfo;


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
        m_ModelTr = tr.FindChild("Model");
        m_Camera = Camera.main.transform;

        //値初期化
        m_ModelRotateY = 0.0f;

        m_GroundHitInfo = CheckGroundHit(tr.position + tr.up * m_Height);
	}
	
	void Update() 
    {
        //移動処理
        Move();

    }

    void LateUpdate()
    {
        //地面との判定
        m_GroundHitInfo = CheckGroundHit(tr.position + tr.up * m_Height);
        print(m_GroundHitInfo.isHit);
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
        //Debug.DrawRay(reyPos, GetDown() * m_Height, Color.grey, 1.0f, false);       

        return result;
    }

    /// <summary>
    /// 地面の移動処理
    /// </summary>
    private void NormalMove()
    {
        //移動方向入力
        Vector2 inputVec = GetMoveInputAxis();
        //アニメーション変更
        m_Animator.SetBool("InputMove", inputVec.magnitude > 0.0f);

        //重力で下方向に移動する
        Gravity(m_GroundHitInfo.isHit);
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
        //ジャンプ処理
        Jump(m_GroundHitInfo.isHit);
    }

    void Testmove()
    {
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
        }

        //前方向を求める
        Vector3 f = -Vector3.Cross(up, m_Camera.right).normalized;
        //プレイヤーの回転を計算、代入
        tr.rotation = Quaternion.LookRotation(f, up);

        Debug.DrawRay(tr.position, tr.up * 20, Color.green, 0.1f, false);
        Debug.DrawRay(tr.position, tr.forward * 20, Color.blue, 0.1f, false);
        Debug.DrawRay(tr.position, tr.right * 20, Color.red, 0.1f, false);


        //移動方向入力
        Vector2 inputVec = GetMoveInputAxis();
        //アニメーション
        m_Animator.SetBool("InputMove", inputVec.magnitude > 0.5f);

        //入力された値を移動用に補正
        Vector2 moveVec = MoveInputCorrection(inputVec);
        //移動方向を計算
        m_MoveVec = (tr.forward * moveVec.y + tr.right * moveVec.x) * m_MoveSpeed;
        //移動
        tr.position += m_MoveVec * Time.deltaTime;

        //ジャンプ処理
        Jump(hitInfo.isHit);
    }

    /// <summary>
    /// 通常移動
    /// 作成　西
    /// </summary>
    private void NormalMove2()
    {
        //移動方向入力
        Vector2 inputVec = GetMoveInputAxis();
        //Debug.Log(inputVec);

        //アニメーション
        m_Animator.SetBool("InputMove", inputVec.magnitude > 0.0f);

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
        }

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
        //tr.localRotation = Quaternion.LookRotation(front, up);
        Quaternion rotate = Quaternion.LookRotation(front, up);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 0.3f);

        //前ベクトル×スティックの傾き
        m_MoveVec = (tr.forward * inputVec.magnitude) * m_MoveSpeed;
        //移動
        tr.position += m_MoveVec * Time.deltaTime;
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
    /// ジャンプ処理（小杉さんのタスク）
    /// </summary>
    private void Jump(bool isGround)
    {
        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(tr.up * m_JumpPower);
            m_Animator.SetBool("InputJump", true);
        }
    }

    /// <summary>
    /// 重力（仮）
    /// </summary>
    private void Gravity(bool isGround)
    {
        if (!isGround)
            rb.AddForce(GetDown() * m_GravityPower);
        else
            rb.velocity = Vector3.zero;
    }
}
