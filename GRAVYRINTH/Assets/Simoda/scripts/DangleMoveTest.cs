using UnityEngine;
using System.Collections;

public class DangleMoveTest : MonoBehaviour
{
    public bool touchIronBar = false;
    public float moveSpeed = 1.0f;
    public float angleSpeed = 90.0f;
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
    private Vector3 movement;

    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;

    //アニメーション
    private Animator anm;

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

        if (touchIronBar == true)
        {
            rb.velocity = Vector3.zero;

            //プレイヤーの位置から回転軸までの距離　0.009はIronBarの半径
            float distance = 0.57f + 0.009f;

            tr.RotateAround(tr.position + tr.up * distance, tr.right, -Input.GetAxis("Vertical") * angleSpeed * Time.deltaTime);


            //float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
            //Vector3 barPos = ironBar.transform.position;

            Vector3 head = tr.localPosition + tr.up * 0.62f;
            Ray right = new Ray(head, tr.right);
            Ray left = new Ray(head, -tr.right);

            Debug.DrawRay(head, tr.up * 5.0f);

            RaycastHit rightOrLeftHitInto;
            int layerMask = ~(1 << LayerMask.NameToLayer("IgnoredObj") | 1 << LayerMask.NameToLayer("IronBar"));

            if (Physics.Raycast(right.origin, right.direction, out rightOrLeftHitInto, 0.3f, layerMask, QueryTriggerInteraction.Ignore)
                && Input.GetAxis("Horizontal") > 0.1f)
            {
            }
            else if (Physics.Raycast(left.origin, left.direction, out rightOrLeftHitInto, 0.3f, layerMask, QueryTriggerInteraction.Ignore)
                     && Input.GetAxis("Horizontal") < -0.1f)
            {
            }
            else
            {
                barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetIronBarVector());
                movement = barVectorNor * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
                tr.localPosition += movement;
            }

            //tr.localPosition =
            //    new Vector3(
            //        Mathf.Clamp(tr.localPosition.x, barPos.x - moveArea, barPos.x + moveArea),
            //        Mathf.Clamp(tr.localPosition.y, barPos.y - moveArea + 0.2f, barPos.y + moveArea - 0.2f),
            //        Mathf.Clamp(tr.localPosition.z, barPos.z - moveArea, barPos.z + moveArea));

            //Debug.DrawRay(tr.position, forward * 5.0f);

            //アニメーション
            if (Vector3.Dot(tr.right, movement) > 0)
            {
                anm.SetFloat("Pole", 1);
            }
            else if (Vector3.Dot(tr.right, movement) < 0)
            {
                anm.SetFloat("Pole", -1);
            }
        }

        if (touchIronBar == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            //アニメーション
            anm.SetBool("Pole_Jump", true);

            m_GravityDir.SetDirection(-tr.up);
            //カメラの視点をプレイヤーにする
            GameObject.Find("Camera").GetComponent<CameraControl>().SetTarget(gameObject);
            m_MoveManager.SetPlayerUpFront(tr.up, tr.forward);

            GetComponent<NormalMove>().SetIronBarHitDelay(ironBarHitDelay);
            touchIronBar = false;
            jumpCursor.IsHit(false);

            //プレイヤーの向きを更新する
            //m_MoveManager.SetPlayerUpFront(tr.up, Vector3.Cross(tr.up, Camera.main.transform.right));
            //m_MoveManager.SetPlayerUpFront(tr.up, tr.forward);

            StartCoroutine(DelayMethod(1, () =>
            {
                m_MoveManager.SetState(PlayerState.NORMAL);
            }));
            StartCoroutine(DelayMethod(3, () =>
            {
                m_MoveManager.SetPlayerUpFront(tr.up, tr.forward);
            }));
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            anm.SetBool("Pole_Move", true);
            se.volume = 1.0f;
        }
        else
        {
            anm.SetBool("Pole_Move", false);
            se.volume = 0.0f;
        }
    }

    public void SetTouchIronBar(bool ishit, RaycastHit hitInto, string upOrDown)
    {
        rb.velocity = Vector3.zero;

        if (upOrDown == "Up")
        {
            //StartCoroutine(DelayMethod(1, () =>
            //{
            //    tr.localPosition = hitInto.point + -tr.up * 0.57f;
            //}));

            StartCoroutine(DelayMethod(2, () =>
            {
                tr.localPosition = hitInto.point + -tr.up * 0.57f;
            }));

        }
        else if (upOrDown == "Down")
        {
            //StartCoroutine(DelayMethod(1, () =>
            //{
            //    tr.localPosition = hitInto.point + -tr.up * (0.009f + 0.57f);
            //}));

            StartCoroutine(DelayMethod(2, () =>
            {
                tr.localPosition = hitInto.point + -tr.up * (0.009f + 0.57f + 0.05f);
            }));
        }

        this.hitInto = hitInto;
        touchIronBar = true;

        ironBar = hitInto.collider.gameObject;

        jumpCursor.ChangeType(JumpCursorDraw.CursorType.IronBar);
        jumpCursor.IsHit(true);

        //tr.parent = ironBar.transform;
        //StartCoroutine(DelayMethod(1, () =>
        //{
        //    forward = Vector3.Cross(tr.up, ironBar.GetComponent<IronBar>().GetIronBarVector());
        //    Quaternion rotate = Quaternion.LookRotation(-forward, tr.up);
        //    tr.localRotation = rotate;
        //}));

        StartCoroutine(DelayMethod(3, () =>
        {
            rb.velocity = Vector3.zero;
            Vector3 playerForward = Vector3.Cross(tr.up, ironBar.GetComponent<IronBar>().GetIronBarVector());
            Quaternion rotate = Quaternion.LookRotation(-playerForward, tr.up);
            tr.localRotation = rotate;
            float angle = 90.0f - Vector3.Angle(tr.up, ironBar.GetComponent<IronBar>().GetIronBarVector());
            //print(angle);
            tr.localRotation *= Quaternion.Euler(0, 0, angle);
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