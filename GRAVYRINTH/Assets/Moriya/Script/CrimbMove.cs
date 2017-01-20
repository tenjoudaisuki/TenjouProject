using UnityEngine;
using System.Collections;

public class CrimbMove : MonoBehaviour
{
    public bool touchIronBar = false;
    public float moveSpeed = 1.0f;
    public float angleSpeed = 90.0f;

    private Transform tr;
    private Rigidbody rb;
    private GravityDirection m_GravityDir;
    //private GameObject ironBarTouchPoint;
    private RaycastHit hitInto;
    private Vector3 barVectorNor;
    private GameObject ironBar;
    private JumpCursorDraw jumpCursor;

    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;

    void Start()
    {
        tr = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        //ironBarTouchPoint = GameObject.Find("IronBarTouchPoint");
        jumpCursor = GameObject.Find("JumpCursor").GetComponent<JumpCursorDraw>();
        m_MoveManager = GetComponent<PlayerMoveManager>();
    }

    void Update()
    {
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

        if (touchIronBar == true)
        {
            rb.velocity = Vector3.zero;

            //プレイヤーの位置から回転軸までの距離　0.009はIronBarの半径
            float distance = hitInto.distance + 0.009f;

            tr.RotateAround(tr.position + tr.forward * distance, tr.up, -Input.GetAxis("Horizontal") * angleSpeed * Time.deltaTime);


            float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
            Vector3 barPos = ironBar.transform.position;

            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
            Vector3 movement = barVectorNor * -Input.GetAxis("Vertical") * -moveSpeed * Time.deltaTime;
            tr.localPosition += movement;
            tr.localPosition =
                new Vector3(
                    Mathf.Clamp(tr.localPosition.x, barPos.x - moveArea, barPos.x + moveArea),
                    Mathf.Clamp(tr.localPosition.y, barPos.y - moveArea + 0.62f, barPos.y + moveArea),
                    Mathf.Clamp(tr.localPosition.z, barPos.z - moveArea, barPos.z + moveArea));
        }

        if (touchIronBar == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            //tr.parent = null;
            //tr.parent = GameObject.Find("Pausable").transform;
            m_MoveManager.SetState(PlayerState.NORMAL);
            //背面斜め上方向に方向にジャンプする
            m_MoveManager.PlayerPoleKick(Vector3.Normalize(-tr.forward + tr.up));

            GetComponent<NormalMove>().SetIronBarHitDelay(1.0f);

            touchIronBar = false;

            jumpCursor.IsHit(false);
        }
    }

    public void SetTouchIronBar(bool ishit, RaycastHit hitInto)
    {
        this.hitInto = hitInto;
        touchIronBar = true;

        ironBar = hitInto.collider.gameObject;

        //tr.parent = ironBar.transform;

        jumpCursor.IsHit(true);

        StartCoroutine(DelayMethod(1.0f, () =>
        {
            Quaternion rotate = Quaternion.LookRotation(tr.forward, ironBar.GetComponent<IronBar>().GetBarVector());
            tr.localRotation = rotate;
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
}