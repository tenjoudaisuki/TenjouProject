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

public class NormalMove : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private Rigidbody rb;
    private Animator anm;
    private CapsuleCollider cc;
    private AudioSource se;
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
    [SerializeField, TooltipAttribute("壁キックの強さ")]
    private float m_WallKickPower = 200.0f;
    [SerializeField, TooltipAttribute("壁キックの高さ（上方向へ向かう量、0だと真横、1だと斜め45度）")]
    private float m_WallKickHeight = 1.5f;
    [SerializeField, TooltipAttribute("壁衝突時　壁キック可能な壁とみなす角度")]
    private float m_WallKickAbleAngle = 80.0f;
    [SerializeField, TooltipAttribute("壁キック後の操作不能時間")]
    private float m_DisableInputTime = 0.2f;
    [SerializeField, TooltipAttribute("ジャンプ後、通常移動速度からジャンプ中の移動速度に変更するまでにかかる時間")]
    private float m_ToJumpMoveSpeedTime = 0.5f;
    [SerializeField, TooltipAttribute("ぶら下がりをスペースキーで解除時、当たり判定を消滅させる時間")]
    private float m_DangleColliderOffTime = 0.6f;
    [SerializeField, TooltipAttribute("壁に着地したとき、押し返す量")]
    private float m_OnWallPushBack = 0.1f;
    [SerializeField, TooltipAttribute("プレイヤーを入力方向へ向ける速さ　補完値")]
    private float m_RotateLerpValue = 0.3f;
    [SerializeField, TooltipAttribute("ブロックを掴んでいる状態のときの後ろ方向のレイの長さ")]
    private float m_BlockMoveBackwardRayLength = 0.5f;
    [SerializeField, TooltipAttribute("鉄棒をよじ登りで判定するときの当たり判定の大きさ")]
    private float m_CrimbHitSize = 0.1f;
    [SerializeField, TooltipAttribute("上方向の鉄棒をぶら下がりで判定するときの当たり判定の大きさ")]
    private float m_DangleUpHitSize = 0.1f;
    [SerializeField, TooltipAttribute("下方向の鉄棒をぶら下がりで判定するときの当たり判定の大きさ")]
    private float m_DangleDownHitSize = 0.2f;
    [SerializeField, TooltipAttribute("鉄棒をよじ登りで判定するときのレイの長さ 変えない方がいいかも")]
    private float m_CrimRayLength = 0.1f;
    [SerializeField, TooltipAttribute("上方向の鉄棒をぶら下がりで判定するときのレイの長さ 変えない方がいいかも")]
    private float m_DangleUpRayLength = 0.7f;
    [SerializeField, TooltipAttribute("下方向の鉄棒をぶら下がりで判定するときのレイの長さ 変えない方がいいかも")]
    private float m_DangleDownRayLength = 0.2f;
    [SerializeField, TooltipAttribute("歩きのSE")]
    private AudioClip m_WalkMoveSEClip;
    [SerializeField, TooltipAttribute("壁ずりのSE")]
    private AudioClip m_WallSEClip;
    [SerializeField, TooltipAttribute("崖登りを行うか（デバッグ用）")]
    private bool m_IsWallHold = false;

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
    //ヒットしているブロック（動かすブロック）
    private Block m_CollisionBlock;
    //操作不能か？
    private bool m_DisableInput = false;


    //壁との判定で使用 レイのヒット情報
    RayHitInfo m_WallHitInfoFront, m_WallHitInfoLeft, m_WallHitInfoRight;
    //壁の法線方向
    Vector3 m_WallNormal;

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

    //実際の移動速度
    private float m_LastSpeed;

    //連続で鉄棒に当たらないようにするための待ち時間
    private float m_IronBarHitDelay = 0.0f;

    //ジャンプ中に速度を遅くするコルーチン
    private Coroutine m_LastSpeedCoroutine;

    //アニメーション用
    //*-壁キック
    private bool m_IsWall;
    //*-ブロック
    private float m_Block;

    //イベント時の操作不能用
    private bool m_IsEventDisableInput = false;

    /*==外部参照変数==*/

    void Awake()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        anm = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider>();
        se = GetComponent<AudioSource>();
        m_MoveManager = GetComponent<PlayerMoveManager>();
    }

    void Start()
    {
        //rigidbodyによるrotateの変更のみを封じる
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        //オブジェクト取得
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        m_Camera = Camera.main.transform;

        //地面との判定
        CheckGroundHit();

        anm.speed = m_AnimSpeed;

        //親を取得
        m_InitParentTr = tr.parent;

        m_LastSpeed = m_MoveSpeed;

        //null参照回避のため一度動かす
        m_LastSpeedCoroutine = StartCoroutine(LastSpeedCalc());

        m_IsEventDisableInput = false;
    }

    void Update()
    {
        se.volume = 0.0f;

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

        //アニメーション
        anm.SetBool("IsWall", m_IsWall);
        //歩きのSEの音量
        if (m_GroundHitInfo.isHit && m_MoveVelocity.magnitude > 0.0f)
        {
            if (se.clip == m_WallSEClip)
            {
                se.clip = m_WalkMoveSEClip;
                se.Play();
            }
            se.volume = 1.0f;
        }
    }

    void FixedUpdate()
    {
        //重力で下方向に移動する
        Gravity();

        //アニメーション
        anm.SetFloat("Jump_Velo", Vector3.Dot(tr.up, rb.velocity));
    }

    void LateUpdate()
    {

    }


    /**==============================================================================================*/
    /** Update内で実行
    /**==============================================================================================*/

    /// <summary>
    /// 地面との判定などを行う
    /// </summary>
    private void Ground()
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
    }

    /// <summary>
    /// 鉄棒との判定処理
    /// </summary>
    public void IronBar()
    {
        m_IronBarHitDelay -= Time.deltaTime;

        Ray forward = new Ray(tr.position, tr.forward);
        RaycastHit forwardHitInto;

        //Debug.DrawRay(forward.origin, forward.direction * 0.2f, Color.yellow);

        int layerMask = 1 << 8;

        //鉄棒をポールとして判定
        if (Physics.BoxCast(forward.origin, Vector3.one * m_CrimbHitSize, forward.direction, out forwardHitInto, tr.localRotation, m_CrimRayLength, layerMask, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(tr.up, forwardHitInto.collider.GetComponent<IronBar>().GetBarVector());

            if (forwardHitInto.collider.tag == ("IronBar") && angle < 45.0f && m_IronBarHitDelay < 0.0f)
            {
                CapsuleCollider col = this.gameObject.GetComponent<CapsuleCollider>();
                col.enabled = false;
                m_MoveManager.SetState(PlayerState.IRON_BAR_CLIMB);
                GetComponent<CrimbMove>().SetTouchIronBar(true, forwardHitInto);

                //アニメーション
                anm.SetTrigger("PoleV");
                StopCoroutine(m_LastSpeedCoroutine);
            }
        }

        Ray down = new Ray(tr.position, -tr.up);
        RaycastHit downHitInto;

        //Debug.DrawRay(down.origin, down.direction * 0.7f, Color.black);

        //鉄棒を鉄棒として判定
        if (Physics.BoxCast(down.origin, Vector3.one * m_DangleDownHitSize, down.direction, out downHitInto, tr.localRotation, m_DangleDownRayLength, layerMask, QueryTriggerInteraction.Ignore)
            && !GetIsGroundHit())
        {
            float angle = Vector3.Angle(tr.up, downHitInto.collider.GetComponent<IronBar>().GetBarVector());

            if (downHitInto.collider.tag == ("IronBar") && angle >= 45.0f && m_IronBarHitDelay < 0.0f)
            {
                CapsuleCollider col = this.gameObject.GetComponent<CapsuleCollider>();
                col.enabled = false;
                m_MoveManager.SetState(PlayerState.IRON_BAR_DANGLE);
                GetComponent<DangleMove>().SetTouchIronBar(true, downHitInto, "Down");

                //アニメーション
                anm.SetTrigger("PoleH");
                StopCoroutine(m_LastSpeedCoroutine);

                return;
            }
        }


        Ray up = new Ray(tr.position, tr.up);
        RaycastHit upHitInto;

        //Debug.DrawRay(up.origin, up.direction * 0.7f, Color.black);

        //鉄棒を鉄棒として判定
        if (Physics.BoxCast(up.origin, Vector3.one * m_DangleUpHitSize, up.direction, out upHitInto, tr.localRotation, m_DangleUpRayLength, layerMask, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(tr.up, upHitInto.collider.GetComponent<IronBar>().GetBarVector());

            if (upHitInto.collider.tag == ("IronBar") && angle >= 45.0f && m_IronBarHitDelay < 0.0f)
            {
                CapsuleCollider col = this.gameObject.GetComponent<CapsuleCollider>();
                col.enabled = false;
                m_MoveManager.SetState(PlayerState.IRON_BAR_DANGLE);
                GetComponent<DangleMove>().SetTouchIronBar(true, upHitInto, "Up");

                //アニメーション
                anm.SetTrigger("PoleH");
                StopCoroutine(m_LastSpeedCoroutine);

            }
        }
    }

    /// <summary>
    /// 壁キック処理
    /// </summary>
    private void WallKick()
    {
        float wallAngle = Vector3.Angle(m_Front, m_WallNormal);

        ////不要になった
        //Vector3 inputAxis = new Vector3(MoveFunctions.GetMoveInputAxis().x, 0, MoveFunctions.GetMoveInputAxis().y);
        //float frontAngle = Vector3.Angle(m_Front, m_Front * inputAxis.magnitude);

        //壁に触れているか？
        if ((180 - m_WallKickAbleAngle / 2 < wallAngle && wallAngle < 180 + m_WallKickAbleAngle / 2))
        {

            //落下中なら
            if (Vector3.Dot(tr.up, rb.velocity) < 0)
            {
                rb.velocity = Vector3.zero;
                rb.AddForce(GetDown() * 10);

                if (!m_IsWall)
                {
                    //アニメーション
                    anm.SetTrigger("Wall");

                    m_IsWall = true;
                    se.clip = m_WallSEClip;
                    se.Play();
                }
                se.volume = 1.0f;
            }

            //壁キックボタンを押したとき
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                SoundManager.Instance.PlaySe("jump");

                //壁キックする方向
                Vector3 dir = tr.up * m_WallKickHeight + m_WallNormal;
                dir.Normalize();

                //壁キック
                rb.velocity = Vector3.zero;//いったんリセット
                rb.AddForce(dir * m_WallKickPower);

                //前方向を壁キックした壁の方向にする
                m_Front = m_WallNormal;
                //入力方向も反対にする
                m_InputAngleY += 180;

                //アニメーション
                anm.SetTrigger("Wall_Jump");

                //最終的な移動量の計算コルーチン実行
                m_LastSpeedCoroutine = StartCoroutine(LastSpeedCalc());
                //操作不能時間計測開始
                StartCoroutine(WallKickInputDisable());
            }

            //if (frontAngle >= 0)
            //{
            //    rb.velocity = Vector3.zero;
            //    rb.AddForce(GetDown() * 10);
            //    m_IsWallTouch = true;
            //}
        }
        else
        {
            m_IsWall = false;
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        //地面の上方向とカメラの右方向で外積を取得
        Vector3 camerafoward = -Vector3.Cross(m_Up, m_Camera.right);

        //地面と当たっていたら
        if (m_GroundHitInfo.isHit)
        {
            OnGround();
        }

        //着地した瞬間の処理
        if (m_IsGroundHitTrigger)
        {
            OnGroundTrigger(camerafoward);

            //アニメーション
            anm.SetTrigger("Landing");
            m_IsWall = false;
        }

        //移動方向入力
        Vector2 inputVec = Vector2.zero;
        //入力不可状態なら入力を取得しない
        if (!m_DisableInput && !m_IsEventDisableInput)
            inputVec = MoveFunctions.GetMoveInputAxis();
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
            if (!m_IsEventDisableInput)
                m_Front = Quaternion.AngleAxis(m_InputAngleY, m_Up) * camerafoward;

            //SpinParentに乗っているときの前方向計算処理
            if (m_IsOnSpinParent)
            {

            }
        }

        //ブロックが近くになかったらnullを設定
        if (m_CollisionBlock != null
            && m_CollisionBlock.GetPushDistance() < Vector3.Distance(tr.position, m_CollisionBlock.gameObject.transform.position))
        {
            m_CollisionBlock = null;
        }

        //ブロック移動ボタンを押していて、かつブロックが近くにある時
        if (Input.GetButton("Action") && m_CollisionBlock != null && m_GroundHitInfo.isHit == true
            && Vector3.Angle(tr.up, m_CollisionBlock.GetPlayerDirection().normal) >= 87.0f
            && Vector3.Angle(tr.up, m_CollisionBlock.GetPlayerDirection().normal) <= 93.0f)
        {
            //ブロック移動ボタンを押した瞬間
            if (Input.GetButtonDown("Action"))
            {
                m_MoveVelocity = Vector3.zero;
                SoundManager.Instance.PlaySe("block");
            }
            Vector3 prev = m_MoveVelocity;
            //前ベクトル×スティックの傾き×移動速度
            m_MoveVelocity = (tr.forward * inputVec.magnitude) * m_LastSpeed;

            //アニメーション
            anm.SetBool("Block", true);

            m_CollisionBlock.IsPushDistance();
            //ブロックが押せない状態なら実行しない
            if (m_CollisionBlock.isPush == false
                || Vector3.Angle(tr.up, m_CollisionBlock.GetPlayerDirection().normal) <= 87.0f
                || Vector3.Angle(tr.up, m_CollisionBlock.GetPlayerDirection().normal) >= 93.0f)
            {
            }
            else
            {

                //print(Vector3.Angle(tr.right, m_Camera.right));
                float angle = Vector3.Angle(tr.right, m_Camera.right);
                float input = -inputVec.y;

                BlockArrow blockArrow = GameObject.FindGameObjectWithTag("BlockArrow").GetComponent<BlockArrow>();

                if (angle <= 45.0f)
                {
                    input = -inputVec.y;
                    blockArrow.SetInfo(true, "Vertical");
                    m_CollisionBlock.GetComponent<Block>().SetPushDecision(-Input.GetAxis("Vertical") < -0.1f);
                }
                else if (angle >= 135.0f)
                {
                    input = inputVec.y;
                    blockArrow.SetInfo(true, "Vertical");
                    m_CollisionBlock.GetComponent<Block>().SetPushDecision(-Input.GetAxis("Vertical") > 0.1f);
                }
                else
                {
                    angle = Vector3.Angle(tr.forward, m_Camera.right);
                    if (angle <= 45.0f)
                    {
                        input = -inputVec.x;
                        blockArrow.SetInfo(true, "Horizontal");
                        m_CollisionBlock.GetComponent<Block>().SetPushDecision(-Input.GetAxis("Horizontal") < -0.1f);
                    }
                    else if (angle >= 135.0f)
                    {
                        input = inputVec.x;
                        blockArrow.SetInfo(true, "Horizontal");
                        m_CollisionBlock.GetComponent<Block>().SetPushDecision(-Input.GetAxis("Horizontal") > 0.1f);
                    }
                }

                //後ろに壁があるなら移動させない
                float stopspeed = 1.0f;
                //後ろ方向への移動入力があるなら実行
                if (input >= 0)
                {
                    //自身の後ろレイを飛ばして壁との判定
                    Ray ray_back = new Ray(tr.position + tr.forward * 0.2f, -tr.forward);
                    RaycastHit hit;
                    bool ishit;
                    //指定レイヤー以外と判定させる
                    int layermask = ~(1 << 10 | 1 << LayerMask.NameToLayer("IronBar"));
                    ishit = Physics.Raycast(ray_back, out hit, m_BlockMoveBackwardRayLength, layermask, QueryTriggerInteraction.Ignore);

                    if (ishit) stopspeed = 0.0f;
                }


                //ブロックの向きから移動方向を計算
                Vector3 moveDirection = m_CollisionBlock.GetBlockMoveDirection();
                m_MoveVelocity = (moveDirection * input) * m_LastSpeed * stopspeed;
                //ブロックを移動させる
                m_CollisionBlock.SetMoveVector(m_MoveVelocity);

                //ブロックを引きずる音開始
                if (prev == Vector3.zero && Mathf.Abs(input) > 0)
                    SoundManager.Instance.PlayLoopSe("rumble");
                //ブロックを引きずる音終了
                else if (prev != Vector3.zero && m_MoveVelocity.magnitude <= 0.1f)
                    SoundManager.Instance.StopLoopSe();


                //自身の前方向をブロックに向ける
                m_Front = -m_CollisionBlock.GetPlayerDirection().normal;

                //向きを変更
                m_Front.Normalize();
                m_Up.Normalize();
                Quaternion rotateBlock = Quaternion.LookRotation(m_Front, m_Up);
                tr.localRotation = Quaternion.Slerp(transform.localRotation, rotateBlock, m_RotateLerpValue);

                //tr.localRotation = rotateBlock;
            }
        }
        //通常時
        else
        {
            //前ベクトル×スティックの傾き×移動速度
            m_MoveVelocity = (tr.forward * inputVec.magnitude) * m_LastSpeed;

            if (Input.GetButtonUp("Action") && m_CollisionBlock != null)
                SoundManager.Instance.StopLoopSe();

            BlockArrow blockArrow = GameObject.FindGameObjectWithTag("BlockArrow").GetComponent<BlockArrow>();
            blockArrow.SetInfo(false, "Horizontal");
            
            //向きを変更
            m_Front.Normalize();
            m_Up.Normalize();
            Quaternion rotate = Quaternion.LookRotation(m_Front, m_Up);
            tr.localRotation = Quaternion.Slerp(transform.localRotation, rotate, m_RotateLerpValue);
            //tr.localRotation = rotate;
            

            //アニメーション
            anm.SetBool("Block", false);
        }
        //アニメーション
        if (m_MoveVelocity != Vector3.zero)
        {
            anm.SetFloat("Block_Velo", Vector3.Dot(m_Front, m_MoveVelocity));
        }
        else
            anm.SetFloat("Block_Velo", 0);

        //ジャンプ処理
        Jump();

        //進行方向に壁がある場合は移動しない
        if (!CollisionWall())
            //移動
            tr.position += m_MoveVelocity * Time.deltaTime;

        //アニメーション
        anm.SetBool("Move", inputVec.magnitude > 0.0f);
        //anm.SetFloat("Jump_Velo", Vector3.Dot(tr.up, rb.velocity));
    }

    /// <summary>
    /// 崖のぼり(実装難しいかも)
    /// </summary>
    private void WallHold()
    {
        //Vector3 rayPos1 = tr.position + tr.up * m_Height;
        //Vector3 rayPos2 = tr.position + tr.up * m_Height * 3 / 4;
        //Ray ray1 = new Ray(rayPos1, tr.forward);
        //Ray ray2 = new Ray(rayPos2, tr.forward);

        //RaycastHit hit_hed, hit_neck;
        //RayHitInfo m_WallHitHead, m_WallHitNeck;

        ////レイヤー決定
        //int layermask = ~((1 << LayerMask.NameToLayer("IgnoredObj")) + (1 << LayerMask.NameToLayer("WallHold")));
        //m_WallHitHead.isHit = Physics.Raycast(ray1, out hit_hed, 0.3f, layermask);
        //m_WallHitNeck.isHit = Physics.Raycast(ray2, out hit_neck, 0.3f, layermask);

        //m_WallHitHead.hit = hit_hed;
        //m_WallHitNeck.hit = hit_neck;

        ////レイをデバッグ表示
        //Debug.DrawRay(rayPos1, tr.forward, Color.red, 1, false);
        //Debug.DrawRay(rayPos2, tr.forward, Color.red, 1, false);

        ////壁のぼり開始フラグ
        //if (m_WallHoldFlag)
        //{
        //    m_WallHoldTimer += Time.deltaTime;
        //}
        ////壁のぼりが始まったら
        //if (m_WallHoldTimer > 0.1f)
        //{
        //    tr.position += (m_Height * 1.2f * tr.up * Time.deltaTime) + (0.5f * tr.forward * Time.deltaTime);

        //    //アニメーション
        //    if (anm.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Idle"))
        //    {
        //        anm.SetBool("ClambLarge", false);
        //        m_WallHoldFlag = false;
        //        m_WallHoldTimer = 0;
        //    }
        //    else
        //        anm.SetBool("ClambLarge", true);
        //}
        //if (!m_WallHitHead.isHit && m_WallHitNeck.isHit)
        //{
        //    m_WallHoldFlag = true;
        //}
        //else
        //{
        //    //m_WallHoldFlag = false;
        //}
    }








    /**==============================================================================================*/
    /** 上記関数内で使用
    /**==============================================================================================*/

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
        //[IgnoredObj][IronBar][Player]レイヤー以外と判定させる
        int layermask = ~(1 << 10 | 1 << LayerMask.NameToLayer("IronBar") | 1 << LayerMask.NameToLayer("Player"));
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
        //操作不能状態ならジャンプしない
        if (m_IsEventDisableInput) return;

        //地面にいるときのジャンプ始動処理
        if (m_GroundHitInfo.isHit && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            SoundManager.Instance.PlaySe("jump");
            //アニメーション
            anm.SetTrigger("Jump");

            //力を加えてジャンプ
            rb.velocity = Vector3.zero;//いったんリセット
            rb.AddForce(m_Up * m_JumpPower);
            //一定時間経過まで地面との判定を行わない
            m_IsCheckGround = false;
            //判定の結果も切っておく
            m_GroundHitInfo.isHit = false;

            //最終的な移動量の計算コルーチン実行
            m_LastSpeedCoroutine = StartCoroutine(LastSpeedCalc());
            //ジャンプ後の地面との判定を行わない時間計測コルーチン実行
            StartCoroutine(CheckGroundOffTime());
        }
    }

    /// <summary>
    /// 重力で下に移動する
    /// </summary>
    private void Gravity()
    {
        //地面にいない、かつ壁のぼりをしていないときは重力をかける
        if (!m_GroundHitInfo.isHit && m_WallHoldTimer == 0)
        {
            rb.AddForce(GetDown() * m_GravityPower);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 着地した瞬間の処理
    /// </summary>
    private void OnGroundTrigger(Vector3 cameraFoward)
    {
        SoundManager.Instance.PlaySe("land");
        //外積をスティックの角度で回転させて前ベクトルを計算
        if (!m_IsEventDisableInput)
            m_Front = Quaternion.AngleAxis(m_InputAngleY, m_Up) * cameraFoward;
        else
            m_Front = Quaternion.AngleAxis(m_InputAngleY, m_Up) * tr.forward;
        //操作可能にする
        m_DisableInput = false;
    }

    /// <summary>
    /// 着地しているときの処理
    /// </summary>
    private void OnGround()
    {
        //上方向と平面の法線方向のなす角
        float angle = Vector3.Angle(m_Up, m_GroundHitInfo.hit.normal);
        //斜面として認識する角度以下なら地面に当たったとする
        if (angle <= m_SlopeDeg)
        {
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
                Vector3 movement = hitTr.gameObject.GetComponent<SpinChild>().GetMovement();
                tr.position += movement;
            }

            ////回転オブジェクトに当たっているなら
            //if (hitTr.tag == "SpinParent")
            //{
            //    //相手を自身の親に設定して追従
            //    tr.parent = hitTr;
            //    m_ParentRotation = hitTr.rotation;
            //    m_IsOnSpinParent = true;
            //}
            //else
            //{
            //    //親子関係を解除
            //    tr.parent = m_InitParentTr;
            //    m_IsOnSpinParent = false;
            //}
        }
        //斜面として認識する角度より大きいなら壁に当たったとする（その後はずり落ちる）
        else
        {
            //壁に対して水平方向がupになるようにする

            //壁の向き
            Vector3 n = m_GroundHitInfo.hit.normal.normalized;
            //壁に向かう方向
            Vector3 wallFront = -n;
            //現在の前方向と壁に向かう方向を比較して右を確定
            Vector3 right = tr.right;
            if (Vector3.Dot(m_Front, wallFront) <= 0)
            {
                right = -right;
            }

            //上方向を計算
            Quaternion toUp = Quaternion.AngleAxis(-90.0f, right);
            m_Up = toUp * wallFront;
            m_Up.Normalize();
            m_Front = wallFront;

            //ちょっと押し返して壁にめり込まないようにする
            tr.position += n * m_OnWallPushBack;
        }
    }

    /// <summary>
    /// 壁との当たり判定用Ray(スマートじゃない)
    /// </summary>
    private bool CollisionWall()
    {
        m_WallNormal = Vector3.zero;
        Vector3 rayPos = tr.position + tr.forward * 0.1f + tr.up / 3;
        Ray ray_front = new Ray(rayPos, tr.forward);
        Ray ray_left = new Ray(rayPos, tr.forward - tr.right);
        Ray ray_right = new Ray(rayPos, tr.forward + tr.right);

        RaycastHit hit_front, hit_left, hit_right;

        //指定レイヤー以外と判定させる
        int layermask = ~(1 << 10 | 1 << LayerMask.NameToLayer("IronBar"));
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
                m_WallNormal = m_WallHitInfoFront.hit.normal.normalized;
            else if (m_WallHitInfoLeft.isHit)
                m_WallNormal = m_WallHitInfoLeft.hit.normal.normalized;
            else
                m_WallNormal = m_WallHitInfoRight.hit.normal.normalized;

            return true;
        }

        else
            return false;
    }

    /// <summary>
    /// ジャンプした直後の、地面と判定させない時間計測コルーチン
    /// </summary>
    IEnumerator CheckGroundOffTime()
    {
        float timer = 0.0f;
        m_IsCheckGround = false;
        while (true)
        {
            //時間経過で操作可能にする
            timer += Time.deltaTime;
            if (timer > m_JumpedTime)
            {
                m_IsCheckGround = true;
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 操作不能時間計測コルーチン
    /// </summary>
    IEnumerator WallKickInputDisable()
    {
        //操作不能にする
        m_DisableInput = true;
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
    /// ジャンプ後の速度計算コルーチン
    /// </summary>
    IEnumerator LastSpeedCalc()
    {
        float timer = 0.0f;
        while (true)
        {
            timer += Time.deltaTime;
            m_LastSpeed = Mathf.Lerp(m_MoveSpeed, m_JumpMoveSpeed, timer / m_ToJumpMoveSpeedTime);
            if (m_GroundHitInfo.isHit)
            {
                m_LastSpeed = m_MoveSpeed;
                if (m_LastSpeedCoroutine != null)
                    StopCoroutine(m_LastSpeedCoroutine);
            }


            yield return null;
        }
    }

    /// <summary>
    /// ぶら下がり解除時の当たり判定消滅時間計測コルーチン
    /// </summary>
    IEnumerator DangleToNormalColliderOff()
    {
        float timer = 0.0f;
        CapsuleCollider col = this.gameObject.GetComponent<CapsuleCollider>();
        col.enabled = false;
        while (timer < m_DangleColliderOffTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        col.enabled = true;
        yield break;
    }

    /**==============================================================================================*/
    /** 外部から使用する
    /**==============================================================================================*/

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
    /// 鉄棒状態から通常状態へ遷移時したときの処理
    /// </summary>
    public void IronbarToNormal()
    {
        //地面との判定を再開
        m_IsCheckGround = true;
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
        m_InputAngleY += 180;
        StartCoroutine(DangleToNormalColliderOff());
        //最終的な移動量の計算コルーチン実行
        m_LastSpeedCoroutine = StartCoroutine(LastSpeedCalc());
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
        //向きを変更
        Quaternion rotate = Quaternion.LookRotation(m_Front, m_Up);
        tr.localRotation = rotate;
    }

    /// <summary>
    /// ぶら下がりから通常時へ移行したときの処理
    /// </summary>
    public void DangleToNormal()
    {
        StartCoroutine(DangleToNormalColliderOff());
        //最終的な移動量の計算コルーチン実行
        print("called statrcoroutine");
        StopCoroutine(m_LastSpeedCoroutine);
        m_LastSpeedCoroutine = StartCoroutine(LastSpeedCalc());
    }

    //連続で鉄棒に当たらないための時間を設定
    public void SetIronBarHitDelay(float delay)
    {
        m_IronBarHitDelay = delay;
    }

    /// <summary>
    /// SEを開始
    /// </summary>
    public void RestartSE()
    {
        se.volume = 0.0f;
        se.Play();
    }

    /// <summary>
    /// SEを停止
    /// </summary>
    public void StopSE()
    {
        se.Stop();
    }

    /// <summary>
    /// イベント時の操作不能の開始・終了を行う trueで操作不能 falseで操作可能
    /// </summary>
    public void SetEventInputDisable(bool isInputDisable)
    {
        m_IsEventDisableInput = isInputDisable;
        m_InputAngleY = 0.0f;

        if (m_IsEventDisableInput)
            StopSE();
        else
            RestartSE();
    }

}
