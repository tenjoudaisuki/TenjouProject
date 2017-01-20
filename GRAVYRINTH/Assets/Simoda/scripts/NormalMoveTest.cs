﻿/**==========================================================================*/
/**
 * プレイヤーの移動（通常時）
 * 歩く、斜面を歩く、段差を登る。
 * アニメーションも行う。
 * 重力の方向のセットもここで行う。
 * 作成者：守屋   作成日：16/10/14
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class NormalMoveTest : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private Rigidbody rb;
    private Animator anm;
    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("移動速度")]
    private float m_MoveSpeed = 3.0f;
    [SerializeField, TooltipAttribute("ジャンプ中の移動速度")]
    private float m_JumpMoveSpeed = 2.0f;
    [SerializeField, TooltipAttribute("身長")]
    private float m_Height = 0.65f;
    [SerializeField, TooltipAttribute("斜面と認識する角度（壁と斜面の境界値）")]
    private float m_SlopeDeg = 45.0f;
    [SerializeField, TooltipAttribute("ジャンプ力")]
    private float m_JumpPower = 200.0f;
    [SerializeField, TooltipAttribute("重力の強さ")]
    private float m_GravityPower = 8.0f;
    [SerializeField, TooltipAttribute("地面との判定のレイの長さ")]
    private float m_RayLength = 0.7f;
    [SerializeField, TooltipAttribute("プレイヤー正面と壁との判定のレイの長さ")]
    private float m_WallRayLength = 0.2f;
    [SerializeField, TooltipAttribute("ジャンプ後の地面と判定を行わない時間の長さ")]
    private float m_JumpedTime = 1.0f;
    [SerializeField, TooltipAttribute("アニメーション再生速度")]
    private float m_AnimSpeed = 1.5f;
    [SerializeField, TooltipAttribute("ポールからジャンプするときの強さ")]
    private float m_PoleJumpPower = 140.0f;
    //[SerializeField, TooltipAttribute("壁キック時の操作不能時間")]　実装するかも？
    //private float m_WallJumpDisableInputTime = 1.0f;
    [SerializeField, TooltipAttribute("壁キックの強さ")]
    public float m_WallKickPower = 200.0f;
    [SerializeField, TooltipAttribute("壁キックの高さ（上方向へ向かう量、0だと真横、1だと斜め45度）")]
    public float m_WallKickHeight = 1.5f;
    [SerializeField, TooltipAttribute("壁衝突時　壁キック可能な壁とみなす角度")]
    public float m_WallKickAbleAngle = 80.0f;
    [SerializeField, TooltipAttribute("壁キック後の操作不能時間")]
    public float m_DisableInputTime = 0.2f;
    [SerializeField, TooltipAttribute("崖登りを行うか（デバッグ用）")]
    public bool m_IsWallHold = false;

    /*==内部設定変数==*/
    //重力の方向を所持するクラス。プレイヤー以外で重力を扱う場合こちらのクラスを使用してください。
    private GravityDirection m_GravityDir;
    //カメラ
    private Transform m_Camera;
    //移動方向と移動量
    private Vector3 m_MoveVelocity;
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
    //現在の上方向
    private Vector3 m_Up = Vector3.up;
    //現在の前方向
    Vector3 m_Front = Vector3.forward;
    //入力方向に応じてＹ軸を回転させる
    private float m_InputAngleY = 0.0f;
    // 移動速度保存用
    private float m_Save;
    //ヒットしているブロック（動かすブロック）
    private Block m_CollisionBlock;
    //  アニメーション用変数
    private float m_WallJumpTimer;
    //操作不能か？
    private bool m_DisableInput = false;
    //地面ではなく斜面との衝突中か？
    private bool m_IsHitSlope = false;

    //壁との判定で使用 レイのヒット情報
    RayHitInfo m_WallHitInfoFront, m_WallHitInfoLeft, m_WallHitInfoRight;
    //壁の法線方向
    Vector3 wallNormal;
    //壁キックできるか？
    bool isWallKick;
    //壁に触っているか？
    bool isWallTouch;

    //最初の親トランスフォーム
    private Transform m_InitParentTr;

    //壁のぼり用タイマー
    private float m_WallHoldTimer;
    //壁のぼり判定フラグ
    private bool m_WallHoldFlag;

    //親と接触中か？
    private bool m_IsOnSpinParent = false;
    //親の回転
    private Quaternion m_ParentRotation;

    // 01/17アニメーション
    private float m_HoverTimer;

    //連続で鉄棒に当たらないようにするための待ち時間
    private float m_IronBarHitDelay = 0.0f;

    /*==外部参照変数==*/

    void Awake()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        anm = GetComponent<Animator>();
        m_MoveManager = GetComponent<PlayerMoveManager>();
    }

    void Start()
    {
        //オブジェクト取得
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        m_Camera = Camera.main.transform;

        //地面との判定
        CheckGroundHit();

        //移動速度保存
        m_Save = m_MoveSpeed;
        anm.speed = m_AnimSpeed;

        //親を取得
        m_InitParentTr = tr.parent;
    }

    void Update()
    {
        //地面との判定処理
        Ground();

        //鉄棒との判定処理
        IronBar();

        //空中にいるときは壁キック処理
        if (!m_GroundHitInfo.isHit)
            WallKick();

        //移動処理(壁のぼりをしていなければ)
        if (m_WallHoldTimer == 0)
            Move();

        //壁のぼり
        if (m_IsWallHold)
            WallHold();

        //重力をセット
        m_GravityDir.SetDirection(GetDown());
    }

    void LateUpdate()
    {

    }

    //鉄棒との判定処理
    public void IronBar()
    {
        m_IronBarHitDelay -= Time.deltaTime;

        Ray forward = new Ray(tr.position, tr.forward);
        RaycastHit forwardHitInto;

        Debug.DrawRay(forward.origin, forward.direction * 0.2f, Color.yellow);

        int layerMask = 1 << 8;

        //鉄棒をポールとして判定
        if (Physics.SphereCast(forward.origin, 0.1f, forward.direction, out forwardHitInto, 0.2f, layerMask, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(tr.up, forwardHitInto.collider.GetComponent<IronBar>().GetBarVector());

            if (forwardHitInto.collider.tag == ("IronBar") && angle < 45.0f && m_IronBarHitDelay < 0.0f)
            {
                m_MoveManager.SetState(PlayerState.IRON_BAR_CLIMB);
                GetComponent<CrimbMoveTest>().SetTouchIronBar(true, forwardHitInto);
            }
        }


        Ray up = new Ray(tr.position, tr.up);
        RaycastHit upHitInto;

        Debug.DrawRay(up.origin, up.direction * 0.7f, Color.black);

        //鉄棒を鉄棒として判定
        if (Physics.SphereCast(up.origin, 0.1f, up.direction, out upHitInto, 0.7f, layerMask, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(tr.up, upHitInto.collider.GetComponent<IronBar>().GetBarVector());

            if (upHitInto.collider.tag == ("IronBar") && angle >= 45.0f && m_IronBarHitDelay < 0.0f)
            {
                m_MoveManager.SetState(PlayerState.IRON_BAR_DANGLE);
                GetComponent<DangleMoveTest>().SetTouchIronBar(true, upHitInto);
            }
        }

    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    //鉄棒にあたった瞬間
    //    if (collision.gameObject.tag == "IronBar")
    //    {
    //        //鉄棒の方向
    //        Vector3 barV = Vector3.Normalize(collision.gameObject.GetComponent<IronBar>().GetBarVector());
    //        //自身と鉄棒のなす角に応じて状態変更
    //        float angle = Vector3.Angle(tr.up, barV);
    //        //print(tr.up);
    //        //float dot = Vector3.Dot(tr.up, barV);
    //        print("angle" + angle);
    //        if (angle > 45.0f)
    //        {
    //            m_MoveManager.SetState(PlayerState.IRON_BAR_DANGLE);
    //        }
    //        else
    //        {
    //            m_MoveManager.SetState(PlayerState.IRON_BAR_CLIMB);
    //        }
    //    }
    //}

    /// <summary>
    /// 地面との判定などを行う
    /// </summary>
    private void Ground()
    {
        //重力で下方向に移動する
        Gravity();
        //ジャンプした直後の、地面と判定させない時間計測処理
        if (!m_IsCheckGround)
        {
            //指定時間が経過したら地面との判定を再開
            m_JumpedTimer += Time.deltaTime;
            if (m_JumpedTimer > m_JumpedTime)
            {
                m_JumpedTimer = 0.0f;
                m_IsCheckGround = true;
            }
        }

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
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        //地面の上方向とカメラの右方向で外積を取得
        Vector3 camerafoward = -Vector3.Cross(m_Up, m_Camera.right);

        //ジャンプ関連の移動処理
        //地面と当たっていたら
        if (m_GroundHitInfo.isHit)
        {
            //上方向と平面の法線方向のなす角
            float angle = Vector3.Angle(m_Up, m_GroundHitInfo.hit.normal);
            //斜面として認識する角度以下なら地面に当たったとする
            if (angle <= m_SlopeDeg)
            {
                //斜面と衝突していない
                m_IsHitSlope = false;
                //当たった地点に移動
                tr.position = m_GroundHitInfo.hit.point;
                //上方向を当たった平面の法線方向に変更
                m_Up = m_GroundHitInfo.hit.normal.normalized;

                //ヒットした相手のトランスフォーム
                Transform hitTr = m_GroundHitInfo.hit.transform;
                //回転床と当たっているなら
                if (hitTr.tag == "SpinChild")
                {
                    //床の移動方向に移動
                    Vector3 movement = hitTr.parent.gameObject.GetComponent<SpinChild>().GetMovement();
                    tr.position += movement;
                }

                //回転オブジェクトに当たっているなら
                if (hitTr.tag == "SpinParent")
                {
                    //相手を自身の親に設定して追従
                    tr.parent = hitTr;
                    m_ParentRotation = hitTr.rotation;
                    m_IsOnSpinParent = true;
                }
                else
                {
                    //親子関係を解除
                    tr.parent = m_InitParentTr;
                    m_IsOnSpinParent = false;
                }
            }
            //斜面として認識する角度より大きいなら壁に当たったとする（その後はずり落ちる）
            else
            {
                //斜面と衝突している
                m_IsHitSlope = true;
                //壁に対して水平方向がupになるようにする
                //まずは前方向を確定
                Vector3 n = m_GroundHitInfo.hit.normal;
                Vector3 front = -n.normalized;
                //上方向を計算
                Quaternion toUp = Quaternion.AngleAxis(-90.0f, tr.right);
                m_Up = toUp * front;
                m_Up.Normalize();
                m_Front = front;
            }
            m_HoverTimer = 0;
        }
        else
        {
            print(m_HoverTimer);
            // 01/17アニメーション
            m_HoverTimer += Time.deltaTime;
            if (m_HoverTimer > 0.2f)
                anm.SetBool("Hover", true);
        }


        //着地した瞬間の処理
        if (m_IsGroundHitTrigger)
        {
            // 01/17アニメーション
            anm.SetBool("Jump", false);
            anm.SetBool("Wall", false);
            anm.SetBool("WallJump", false);
            anm.SetBool("PoleHJump", false);
            anm.SetBool("PoleVJump", false);
            anm.SetBool("Hover", false);

            // アニメーション用の変数初期化
            isWallTouch = false;
            m_WallJumpTimer = 0;
            //外積をスティックの角度で回転させて前ベクトルを計算
            m_Front = Quaternion.AngleAxis(m_InputAngleY, m_Up) * camerafoward;
            //操作可能にする
            m_DisableInput = false;
        }

        //移動方向入力
        Vector2 inputVec = Vector2.zero;
        //入力不可状態なら入力を取得しない
        if (!m_DisableInput)
            inputVec = MoveFunctions.GetMoveInputAxis();
        //アニメーション変更
        anm.SetBool("Move", inputVec.magnitude > 0.0f);
        //スティックが入力されたら向きを変える
        if (inputVec.magnitude > 0.3f)
        {
            //スティックの傾きを↑を0°として計算
            m_InputAngleY = Vector2.Angle(Vector2.up, inputVec);
            //ステイックが左に傾いていればyをマイナスに
            if (inputVec.x < 0) m_InputAngleY = -m_InputAngleY;
        }

        //壁キック後の操作不能状態でなければ前ベクトルを計算
        if (!m_DisableInput)
        {
            //無理やりだけど、カメラと同じ計算方法でプレイヤーをカメラと同じ量回転させる
            m_InputAngleY -= m_Camera.GetComponent<CameraControl>().GetHorizontalInput();
            //外積をスティックの角度で回転させて前ベクトルを計算
            m_Front = Quaternion.AngleAxis(m_InputAngleY, m_Up) * camerafoward;

            if (m_IsOnSpinParent)
            {
                Debug.DrawLine(tr.position, tr.position + m_ParentRotation * -Vector3.forward);

                Quaternion to = Quaternion.FromToRotation(
                    m_Front,
                    m_ParentRotation * Vector3.forward
                    );
                //m_Front = -to * m_Front;           
            }
        }

        //前ベクトル×スティックの傾き×移動速度
        float speed = m_MoveSpeed;
        if (!m_GroundHitInfo.isHit)
            speed = m_JumpMoveSpeed;
        m_MoveVelocity = (tr.forward * inputVec.magnitude) * speed;

        //ブロック移動ボタンを押していて、かつブロックが近くにある時
        if (Input.GetButton("Action") && m_CollisionBlock != null && m_GroundHitInfo.isHit == true)
        {
            // 01/17アニメーション
            anm.SetBool("Block", true);

            m_CollisionBlock.IsPushDistance();
            if (m_CollisionBlock.isPush == false) return;

            print(m_MoveVelocity);
            Vector3 moveDirection = m_CollisionBlock.GetBlockMoveDirection();
            m_MoveVelocity = (moveDirection * -inputVec.y) * m_Save;
            m_CollisionBlock.SetMoveVector(m_MoveVelocity);

            m_Front = -m_CollisionBlock.GetPlayerDirection().normal;

            //向きを変更
            Quaternion rotateBlock = Quaternion.LookRotation(m_Front, m_Up);
            tr.localRotation = Quaternion.Slerp(transform.localRotation, rotateBlock, 0.3f);
            //tr.localRotation = rotateBlock;

            //移動
            tr.position += m_MoveVelocity * Time.deltaTime;

            // 01/17アニメーション
            if (m_MoveVelocity == Vector3.zero)
            {
                anm.SetBool("BlockMove", false);
            }
            else
            {
                anm.SetBool("BlockMove", true);
            }
            anm.SetFloat("Block_x", m_MoveVelocity.x * m_Front.x);
            anm.SetFloat("Block_y", m_MoveVelocity.y * m_Front.y);
            anm.SetFloat("Block_z", m_MoveVelocity.z * m_Front.z);
        }
        //通常時
        else
        {
            //向きを変更
            Quaternion rotate = Quaternion.LookRotation(m_Front, m_Up);
            tr.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 0.3f);
            //tr.localRotation = rotate;

            //移動
            tr.position += m_MoveVelocity * Time.deltaTime;

            // 01/17アニメーション
            anm.SetBool("Block", false);
        }

        //ジャンプ処理
        Jump();


        //進行方向に壁がある場合は移動量を0にする
        if (CollisionWall())
            m_MoveSpeed = 0;
        else
            m_MoveSpeed = m_Save;

        // 01/17アニメーション
        anm.SetFloat("Jump_x", rb.velocity.x * tr.up.x);
        anm.SetFloat("Jump_y", rb.velocity.y * tr.up.y);
        anm.SetFloat("Jump_z", rb.velocity.z * tr.up.z);
    }

    /// <summary>
    /// 現在の下方向を取得
    /// </summary>
    private Vector3 GetDown()
    {
        return Vector3.Normalize(-m_Up);
    }

    /// <summary>
    /// 現在向いている下方向にレイを飛ばし、ヒットした情報をm_GroundHitInfoに入れる。
    /// </summary>
    private void CheckGroundHit()
    {
        Vector3 rayPos = tr.position + m_Up * m_Height;
        Ray ray = new Ray(rayPos, GetDown());
        RaycastHit hit;
        //[IgnoredObj]レイヤー以外と判定させる
        int layermask = ~(1 << 10);
        m_GroundHitInfo.isHit = Physics.Raycast(ray, out hit, m_RayLength, layermask, QueryTriggerInteraction.Ignore);
        m_GroundHitInfo.hit = hit;
        ////レイをデバッグ表示
        //Debug.DrawRay(rayPos, GetDown() * m_RayLength, Color.grey, 1.0f, false);
        //Debug.DrawRay(m_GroundHitInfo.hit.point, m_GroundHitInfo.hit.normal, Color.red, 1.0f, false);
        //Debug.DrawRay(m_GroundHitInfo.hit.point, tr.up, Color.blue, 1.0f, false);
    }

    /// <summary>
    /// ジャンプする
    /// </summary>
    private void Jump()
    {
        //地面にいるときのジャンプ始動処理
        if (m_GroundHitInfo.isHit && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            // 01/17アニメーション
            anm.SetBool("Jump", true);

            //力を加えてジャンプ
            rb.velocity = Vector3.zero;//いったんリセット
            rb.AddForce(m_Up * m_JumpPower);
            //一定時間経過まで地面との判定を行わない
            m_IsCheckGround = false;
            //判定の結果も切っておく
            m_GroundHitInfo.isHit = false;
            //タイマーも初期化
            m_JumpedTimer = 0.0f;

            m_IsHitSlope = false;
        }
    }

    /// <summary>
    /// 重力で下に移動する
    /// </summary>
    private void Gravity()
    {
        //地面にいない、かつ壁のぼりをしていないときは重力をかける
        if (!m_GroundHitInfo.isHit && m_WallHoldTimer == 0)
            rb.AddForce(GetDown() * m_GravityPower);
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 最終的な前右方向の移動量を取得する
    /// </summary>
    public Vector3 GetMoveVelocity()
    {
        return m_MoveVelocity;
    }

    /// <summary>
    /// 地面に接地しているか？
    /// </summary>
    public bool GetIsGroundHit()
    {
        return m_GroundHitInfo.isHit;
    }

    /// <summary>
    /// 壁との当たり判定用Ray(スマートじゃない)
    /// </summary>
    private bool CollisionWall()
    {
        wallNormal = Vector3.zero;
        Vector3 rayPos = tr.position + tr.forward * 0.1f + tr.up / 3;
        Ray ray_front = new Ray(rayPos, tr.forward);
        Ray ray_left = new Ray(rayPos, tr.forward - tr.right);
        Ray ray_right = new Ray(rayPos, tr.forward + tr.right);

        RaycastHit hit_front, hit_left, hit_right;

        //[IgnoredObj]レイヤー以外と判定させる
        int layermask = ~(1 << 10);
        m_WallHitInfoFront.isHit = Physics.Raycast(ray_front, out hit_front, m_WallRayLength, layermask, QueryTriggerInteraction.Ignore);
        m_WallHitInfoLeft.isHit = Physics.Raycast(ray_left, out hit_left, m_WallRayLength, layermask, QueryTriggerInteraction.Ignore);
        m_WallHitInfoRight.isHit = Physics.Raycast(ray_right, out hit_right, m_WallRayLength, layermask, QueryTriggerInteraction.Ignore);

        m_WallHitInfoFront.hit = hit_front;
        m_WallHitInfoLeft.hit = hit_left;
        m_WallHitInfoRight.hit = hit_right;

        //レイをデバッグ表示
        Debug.DrawRay(rayPos, tr.forward, Color.grey, m_WallRayLength, false);
        Debug.DrawRay(rayPos, tr.forward - tr.right, Color.grey, m_WallRayLength, false);
        Debug.DrawRay(rayPos, tr.forward + tr.right, Color.grey, m_WallRayLength, false);

        if (m_WallHitInfoFront.isHit ||
            m_WallHitInfoLeft.isHit ||
            m_WallHitInfoRight.isHit)
        {
            //壁の法線が確定
            if (m_WallHitInfoFront.isHit)
                wallNormal = m_WallHitInfoFront.hit.normal.normalized;
            else if (m_WallHitInfoLeft.isHit)
                wallNormal = m_WallHitInfoLeft.hit.normal.normalized;
            else
                wallNormal = m_WallHitInfoRight.hit.normal.normalized;

            return true;
        }

        else
            return false;
    }

    /// <summary>
    /// 鉄棒状態から通常状態へ遷移時したときの処理
    /// </summary>
    public void StateIronbarToNormal()
    {
        //地面との判定を再開
        m_IsCheckGround = true;
        m_JumpedTimer = 0.0f;
    }
    /// <summary>
    /// 通常状態からステージクリア状態へ遷移させる処理
    /// </summary>
    public void NormalToStageClear()
    {
        m_MoveManager.SetState(PlayerState.STAGE_CLEAR);
    }
    /// <summary>
    /// 上と前を更新
    /// </summary>
    public void SetUpFront(Vector3 up, Vector3 front)
    {
        m_Up = up;
        m_Front = front;
    }

    /// <summary>
    /// ポールをキックしたときの移動
    /// </summary>
    public void StartPoleKick(Vector3 v)
    {
        rb.velocity = Vector3.zero;//いったんリセット
        rb.AddForce(v * m_PoleJumpPower);
    }

    /// <summary>
    /// 当たったブロックをセットする
    /// </summary>
    /// <param name="obj"></param>
    public void SetCollisionBlock(GameObject obj)
    {
        try
        {
            m_CollisionBlock = obj.GetComponent<Block>();
        }
        catch
        {
            m_CollisionBlock = null;
        }
    }

    /// <summary>
    /// 座標と向きを指定してリスポーンする
    /// </summary>
    public void Respawn(Vector3 position, Vector3 up, Vector3 front)
    {
        tr.position = position;
        m_Up = up;
        m_Front = front;
        //重力などをリセット
        rb.velocity = Vector3.zero;
    }



    public void WallKick()
    {
        Vector3 inputAxis = new Vector3(MoveFunctions.GetMoveInputAxis().x, 0, MoveFunctions.GetMoveInputAxis().y);

        float wallAngle = Vector3.Angle(tr.forward, wallNormal);
        float frontAngle = Vector3.Angle(tr.forward, tr.forward * inputAxis.magnitude);

        if ((180 - m_WallKickAbleAngle / 2 < wallAngle && wallAngle < 180 + m_WallKickAbleAngle / 2))
        {
            //アニメーション変更
            anm.SetBool("Wall", true);

            if (Vector3.Dot(tr.up, rb.velocity) < 0)
                rb.velocity = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                //操作不能にする
                m_DisableInput = true;
                m_IsHitSlope = false;
                Vector3 dir = tr.up * m_WallKickHeight + wallNormal;
                dir.Normalize();

                rb.velocity = Vector3.zero;//いったんリセット
                rb.AddForce(dir * m_WallKickPower);

                m_Front = wallNormal;
                m_InputAngleY *= -1;

                //アニメーション変更
                anm.SetBool("Wall", false);
                anm.SetTrigger("WallJump");

                //操作不能時間計測開始
                StartCoroutine(WallKickInputDisable());
            }

            if (frontAngle > 1)
            {
                rb.velocity = Vector3.zero;
                rb.AddForce(GetDown() * 50);
                isWallTouch = true;
            }
        }
        else
        {
            anm.SetBool("Wall", false);
            if (!anm.GetBool("Wall"))
            {
                m_WallJumpTimer += 1 * Time.deltaTime;
                //アニメーションの設定
                anm.SetFloat("WallJumpTimer", m_WallJumpTimer);
            }
            else
                m_WallJumpTimer = 0;
        }
        //if (m_GroundHitInfo.isHit)
        //    isWallKick = false;
    }

    /// <summary>
    /// 操作不能時間計測コルーチン
    /// </summary>
    IEnumerator WallKickInputDisable()
    {
        float timer = 0.0f;
        while (true)
        {
            //時間経過で操作可能にする
            timer += Time.deltaTime;
            if (timer > m_DisableInputTime)
            {
                m_DisableInput = false;
                yield break;
            }
            yield return null;
        }
    }
    /// <summary>
    /// 崖つかまり
    /// </summary>
    public void WallHold()
    {
        Vector3 rayPos1 = tr.position + tr.up * m_Height;
        Vector3 rayPos2 = tr.position + tr.up * m_Height * 3 / 4;
        Ray ray1 = new Ray(rayPos1, tr.forward);
        Ray ray2 = new Ray(rayPos2, tr.forward);

        RaycastHit hit_hed, hit_neck;
        RayHitInfo m_WallHitHead, m_WallHitNeck;

        //[IgnoredObj]レイヤー以外と判定させる
        int layermask = ~((1 << LayerMask.NameToLayer("IgnoredObj")) + (1 << LayerMask.NameToLayer("WallHold")));
        m_WallHitHead.isHit = Physics.Raycast(ray1, out hit_hed, 0.3f, layermask);
        m_WallHitNeck.isHit = Physics.Raycast(ray2, out hit_neck, 0.3f, layermask);

        m_WallHitHead.hit = hit_hed;
        m_WallHitNeck.hit = hit_neck;

        //レイをデバッグ表示
        Debug.DrawRay(rayPos1, tr.forward, Color.red, 1, false);
        Debug.DrawRay(rayPos2, tr.forward, Color.red, 1, false);

        //壁のぼり開始フラグ
        if (m_WallHoldFlag)
        {
            m_WallHoldTimer += Time.deltaTime;
        }
        //壁のぼりが始まったら
        if (m_WallHoldTimer > 0.1f)
        {
            tr.position += (m_Height * 1.2f * tr.up * Time.deltaTime) + (0.5f * tr.forward * Time.deltaTime);
            if (anm.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Idle"))
            {
                anm.SetBool("ClambLarge", false);
                m_WallHoldFlag = false;
                m_WallHoldTimer = 0;
            }
            else
                anm.SetBool("ClambLarge", true);
        }
        if (!m_WallHitHead.isHit && m_WallHitNeck.isHit)
        {
            m_WallHoldFlag = true;
        }
        else
        {
            //m_WallHoldFlag = false;
        }
    }

    //連続で鉄棒に当たらないための時間を設定
    public void SetIronBarHitDelay(float delay)
    {
        m_IronBarHitDelay = delay;
    }
}
