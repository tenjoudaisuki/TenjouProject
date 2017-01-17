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

            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
            Vector3 movement = barVectorNor * -Input.GetAxis("Vertical") * -0.1f * moveSpeed;
            tr.localPosition += movement;
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
        }
    }

    public void SetTouchIronBar(bool ishit, RaycastHit hitInto)
    {
        this.hitInto = hitInto;
        touchIronBar = true;

        ironBar = hitInto.collider.gameObject;

        //tr.parent = ironBar.transform;

        Quaternion rotate = Quaternion.LookRotation(tr.forward, ironBar.GetComponent<IronBar>().GetBarVector());
        tr.localRotation = rotate;
    }
}