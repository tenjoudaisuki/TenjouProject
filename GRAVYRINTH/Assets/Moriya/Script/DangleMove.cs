using UnityEngine;
using System.Collections;

public class DangleMove : MonoBehaviour
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

    private Vector3 forward;

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
        if (touchIronBar == true)
        {
            rb.velocity = Vector3.zero;

            //プレイヤーの位置から回転軸までの距離　0.009はIronBarの半径
            float distance = hitInto.distance + 0.009f;

            tr.RotateAround(tr.position + tr.up * distance, tr.right, Input.GetAxis("Vertical") * angleSpeed * Time.deltaTime);


            float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
            Vector3 barPos = ironBar.transform.position;

            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetIronBarVector());
            Vector3 movement = barVectorNor * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            tr.localPosition += movement;
            tr.localPosition =
                new Vector3(
                    Mathf.Clamp(tr.localPosition.x, barPos.x - moveArea, barPos.x + moveArea),
                    Mathf.Clamp(tr.localPosition.y, barPos.y - moveArea + 0.2f, barPos.y + moveArea - 0.2f),
                    Mathf.Clamp(tr.localPosition.z, barPos.z - moveArea, barPos.z + moveArea));

            Debug.DrawRay(tr.position, forward * 5.0f);
        }

        if (touchIronBar == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            //tr.parent = null;
            //tr.parent = GameObject.Find("Pausable").transform;
            //touchIronBar = false;

            m_GravityDir.SetDirection(-tr.up);
            m_MoveManager.SetState(PlayerState.NORMAL);
            //プレイヤーの向きを更新する
            m_MoveManager.SetPlayerUpFront(tr.up, Vector3.Cross(tr.up, Camera.main.transform.right));
            //rb.AddForce(-tr.up * 200.0f);

            //カメラの視点をプレイヤーにする
            GameObject.Find("Camera").GetComponent<CameraControl>().SetTarget(gameObject);

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

        jumpCursor.IsHit(true);

        //tr.parent = ironBar.transform;
        StartCoroutine(DelayMethod(1.0f, () =>
        {
            forward = Vector3.Cross(tr.up, ironBar.GetComponent<IronBar>().GetIronBarVector());
            Quaternion rotate = Quaternion.LookRotation(-forward, tr.up);
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