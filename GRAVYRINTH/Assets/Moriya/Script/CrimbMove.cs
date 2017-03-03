using UnityEngine;
using System.Collections;

public class CrimbMove : MonoBehaviour
{
    public bool touchIronBar = false;
    public float moveSpeed = 1.0f;
    public float angleSpeed = 90.0f;
    public float poleDownTime = 0.5f;
    public float ironBarHitDelay = 0.5f;

    private Transform tr;
    private Rigidbody rb;
    private AudioSource se;
    private GravityDirection m_GravityDir;
    //private GameObject ironBarTouchPoint;
    private RaycastHit hitInto;
    private Vector3 barVectorNor;
    private GameObject ironBar;
    private JumpCursorDraw jumpCursor;
    private float poleDownTimeCount;

    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;

    //アニメーション
    private Animator anm;
    private bool pole;

    //イベント時の操作不能用
    private bool m_IsEventDisableInput = false;

    void Start()
    {
        tr = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
        se = GetComponent<AudioSource>();
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        //ironBarTouchPoint = GameObject.Find("IronBarTouchPoint");
        jumpCursor = GameObject.Find("JumpCursor").GetComponent<JumpCursorDraw>();
        m_MoveManager = GetComponent<PlayerMoveManager>();

        //アニメーション
        anm = GetComponent<Animator>();

        m_IsEventDisableInput = false;
    }

    void Update()
    {
        if (m_IsEventDisableInput) return;

        //Ray forward = new Ray(tr.position, tr.forward);
        //RaycastHit hitInto;

        //int layerMask = 1 << 8;

        //if (Physics.Raycast(forward.origin, forward.direction, out hitInto, 1.0f, layerMask, QueryTriggerInteraction.Ignore))
        //{
        //    if (hitInto.collider.tag == ("IronBar"))
        //    {
        //        touchIronBar = true;
        //    }
        //}
        //print(pole);
        if (touchIronBar == true)
        {
            rb.velocity = Vector3.zero;

            //プレイヤーの位置から回転軸までの距離　0.009はIronBarの半径
            float distance = 0.009f;

            tr.RotateAround(tr.position + tr.forward * distance, tr.up, Input.GetAxis("Horizontal") * angleSpeed * Time.deltaTime);


            //float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
            //Vector3 barPos = ironBar.transform.position;

            Vector3 center = tr.localPosition + tr.up * 0.31f;
            Ray down = new Ray(center, -tr.up);
            Ray up = new Ray(center, tr.up);

            RaycastHit upOrDownHitInto;
            int layerMask = ~(1 << LayerMask.NameToLayer("IgnoredObj") | 1 << LayerMask.NameToLayer("IronBar"));


            if (Physics.Raycast(up.origin, up.direction, out upOrDownHitInto, 0.45f, layerMask, QueryTriggerInteraction.Ignore))
            {
                //上に障害物があった場合自動で上る処理を止める
                poleDownTimeCount += poleDownTime + 1.0f;
            }

            if (Physics.Raycast(down.origin, down.direction, out upOrDownHitInto, 0.25f, layerMask, QueryTriggerInteraction.Ignore)
                && Input.GetAxis("Vertical") < -0.1f)
            {
                //地面まで降りたら鉄棒を離す
                if (poleDownTimeCount > poleDownTime)
                {
                    // アニメーション
                    anm.SetTrigger("Landing");

                    poleDownTimeCount = 0.0f;
                    GetComponent<NormalMove>().SetIronBarHitDelay(ironBarHitDelay);
                    touchIronBar = false;
                    jumpCursor.IsHit(false);
                    CapsuleCollider col = this.gameObject.GetComponent<CapsuleCollider>();
                    col.enabled = true;
                    m_MoveManager.SetState(PlayerState.NORMAL);
                }
            }
            else if (Physics.Raycast(up.origin, up.direction, out upOrDownHitInto, 0.45f, layerMask, QueryTriggerInteraction.Ignore)
                && Input.GetAxis("Vertical") > 0.1f)
            {
            }
            else
            {
                if (poleDownTimeCount > poleDownTime)
                {
                    //操作で鉄棒を登る処理
                    barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
                    Vector3 movement = barVectorNor * -Input.GetAxis("Vertical") * -moveSpeed * Time.deltaTime;
                    tr.localPosition += movement;

                    // アニメーション
                    pole = false;
                }
                else
                {
                    //自動で鉄棒を登る処理

                    // アニメーション
                    anm.SetFloat("Pole", 1);
                    anm.SetBool("Pole_Move", true);
                    pole = true;

                    poleDownTimeCount += Time.deltaTime;
                    barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
                    Vector3 movement = barVectorNor * -1.0f * -moveSpeed * Time.deltaTime;
                    tr.position += movement;
                }
            }

            //tr.localPosition =
            //    new Vector3(
            //        Mathf.Clamp(tr.localPosition.x, barPos.x - moveArea, barPos.x + moveArea),
            //        Mathf.Clamp(tr.localPosition.y, barPos.y - moveArea + 0.31f, barPos.y + moveArea - 0.31f),
            //        Mathf.Clamp(tr.localPosition.z, barPos.z - moveArea, barPos.z + moveArea));

            if (Vector3.Dot(tr.up, barVectorNor * -Input.GetAxis("Vertical") * -moveSpeed * Time.deltaTime) > 0)
            {
                se.volume = 1.0f;
                anm.SetFloat("Pole", 1);
            }
            else if (Vector3.Dot(tr.up, barVectorNor * -Input.GetAxis("Vertical") * -moveSpeed * Time.deltaTime) < 0)
            {
                se.volume = 1.0f;
                anm.SetFloat("Pole", -1);
            }
            else
                se.volume = 0.0f;
        }

        if (touchIronBar == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            //アニメーション
            anm.SetBool("Pole_Jump", true);

            SoundManager.Instance.PlaySe("jump");
            poleDownTimeCount = 0.0f;

            //背面斜め上方向に方向にジャンプする
            Vector3 poleKickVec = Vector3.Normalize(-tr.forward + tr.up);
            StartCoroutine(DelayMethod(2, () =>
            {
                m_MoveManager.PlayerPoleKick(poleKickVec);
            }));

            GetComponent<NormalMove>().SetIronBarHitDelay(ironBarHitDelay);

            touchIronBar = false;

            jumpCursor.IsHit(false);

            //CapsuleCollider col = this.gameObject.GetComponent<CapsuleCollider>();
            //col.enabled = true;

            m_MoveManager.SetState(PlayerState.NORMAL);
        }

        //アニメーション
        if (!pole)
        {
            if (Input.GetAxis("Vertical") != 0)
                anm.SetBool("Pole_Move", true);
            else
                anm.SetBool("Pole_Move", false);
        }
    }

    public void SetTouchIronBar(bool ishit, RaycastHit hitInto)
    {
        rb.velocity = Vector3.zero;
        poleDownTimeCount = 0.0f;

        StartCoroutine(DelayMethod(6, () =>
        {
            tr.localPosition += tr.forward * (0.17f + hitInto.distance / 2.0f);
            //print(hitInto.distance);
        }));

        this.hitInto = hitInto;
        //touchIronBar = true;

        ironBar = hitInto.collider.gameObject;

        //tr.parent = ironBar.transform;

        jumpCursor.ChangeType(JumpCursorDraw.CursorType.Pole);
        jumpCursor.IsHit(true);

        StartCoroutine(DelayMethod(2, () =>
        {
            Vector3 a = tr.position - ironBar.transform.position;
            Vector3 b = hitInto.point - tr.position;
            float c = Vector3.Dot(a, b.normalized);

            Quaternion rotate = Quaternion.LookRotation(-b * c, ironBar.GetComponent<IronBar>().GetBarVector());
            tr.localRotation = rotate;
        }));

        StartCoroutine(DelayMethod(3, () =>
        {
            Quaternion rotate = Quaternion.LookRotation(tr.forward, ironBar.GetComponent<IronBar>().GetBarVector());
            tr.localRotation = rotate;
            float ang = Vector3.Angle(tr.up, ironBar.GetComponent<IronBar>().GetBarVector());
            //print(ang);

            tr.localRotation *= Quaternion.Euler(ang, 0, 0);
        }));

        Vector3 head = tr.localPosition + tr.up * 0.52f + -tr.forward * 0.2f;
        Ray headRay = new Ray(head, tr.forward);
        Debug.DrawRay(headRay.origin, headRay.direction * 5.0f);

        StartCoroutine(DelayMethod(5, () =>
        {
            if (!Physics.SphereCast(headRay, 0.1f, 0.3f, 1 << 8, QueryTriggerInteraction.Ignore))
            {
                Quaternion rotate = Quaternion.LookRotation(tr.forward, ironBar.GetComponent<IronBar>().GetBarVector());
                tr.localRotation = rotate;
                float ang = Vector3.Angle(tr.up, ironBar.GetComponent<IronBar>().GetBarVector());
                //print(ang);

                tr.localRotation *= Quaternion.Euler(-ang, 0, 0);

                StartCoroutine(DelayMethod(2, () =>
                {
                    if (!Physics.SphereCast(headRay, 0.1f, 0.3f, 1 << 8, QueryTriggerInteraction.Ignore))
                    {
                        rotate = Quaternion.LookRotation(tr.forward, ironBar.GetComponent<IronBar>().GetBarVector());
                        tr.localRotation = rotate;
                        ang = Vector3.Angle(tr.up, ironBar.GetComponent<IronBar>().GetBarVector());
                        //print(ang);

                        tr.localRotation *= Quaternion.Euler(-ang, 0, 0);
                    }
                }));
            }
        }));

        StartCoroutine(DelayMethod(7, () =>
        {
            touchIronBar = true;
        }));
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間[ミリ秒]</param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float waitTime, System.Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="delayFrameCount"></param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(int delayFrameCount, System.Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }
        action();
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.black;
    //    if (hitInto.collider != null)
    //        Gizmos.DrawWireSphere(hitInto.point, 0.2f);
    //}


    /// <summary>
    /// イベント時の操作不能の開始・終了を行う
    /// </summary>
    public void SetEventInputDisable(bool isInputDisable)
    {
        m_IsEventDisableInput = isInputDisable;
    }

}