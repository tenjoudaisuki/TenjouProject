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

    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;

    void Start()
    {
        tr = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        //ironBarTouchPoint = GameObject.Find("IronBarTouchPoint");
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

            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetIronBarVector());
            Vector3 movement = barVectorNor * Input.GetAxis("Horizontal") * 0.1f * moveSpeed;
            tr.localPosition += movement;
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
        }
    }

    public void SetTouchIronBar(bool ishit, RaycastHit hitInto)
    {
        this.hitInto = hitInto;
        touchIronBar = true;

        ironBar = hitInto.collider.gameObject;

        //tr.parent = ironBar.transform;

        //Vector3 forward = Vector3.Cross(tr.up, ironBar.GetComponent<IronBar>().GetIronBarVector());
        //Quaternion rotate = Quaternion.LookRotation(forward, tr.up);
        //tr.localRotation = rotate;
    }
}