using UnityEngine;
using System.Collections;

public class IronBarTouchPoint : MonoBehaviour
{
    private Vector3 playerDirection;
    private Transform tr;
    private Rigidbody rb;
    private bool isHit = false;
    private Vector3 direction;

    //public GameObject headPoint;
    private Transform player;
    public float offsetY;

    void Start()
    {
        tr = gameObject.transform;
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Debug.DrawRay(player.position, player.up * 5.0f, Color.black);

        rb.velocity = Vector3.zero;
        Debug.DrawRay(tr.position, direction, Color.green);
        //if (isHit == true)
        //    player.position = tr.position + direction * offsetY;

    }

    public void OnTriggerStay(Collider other)
    {
        //print("Hit");
        //print(other.gameObject.tag + " " + other.gameObject.transform.position);

        if (other.gameObject.tag != "IronBar") return;

        //プレイヤー方向に当たらなくなるまで移動
        if (isHit == true)
            tr.position += playerDirection * 0.01f;
    }

    public void OnTriggerExit(Collider other)
    {
        //player.position = tr.position + direction * offsetY;
        //headPoint.GetComponent<PlayerHeadPoint>().SetHeadPoint(tr.position);
        if (other.gameObject.tag != "IronBar") return;

        TriggerExit(other);
    }

    public void TriggerExit(Collider other)
    {
        if (player.GetComponent<PlayerMoveManager>().GetState() == PlayerState.IRON_BAR_DANGLE)
        {
            //鉄棒が持っているベクトルを右に設定
            tr.right = other.GetComponent<IronBar>().GetIronBarVector();

            //プレイヤーの親を自分に
            player.parent = tr;

            //プレイヤーの位置・回転を設定
            //player.localPosition = new Vector3(0, -6.2f, 0);
            player.localPosition = -direction * -7.0f;

            //player.localRotation = Quaternion.Euler(player.localRotation.eulerAngles.x, 0.0f, player.localRotation.eulerAngles.z);
            //player.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            //player.rotation= Quaternion.LookRotation(, m_Up)
            //player.right = other.GetComponent<IronBar>().GetIronBarVector();
            Vector3 forward = Vector3.Cross(other.GetComponent<IronBar>().GetIronBarVector(), -direction);
            player.rotation = Quaternion.LookRotation(forward, -direction);

        }

        if (player.GetComponent<PlayerMoveManager>().GetState() == PlayerState.IRON_BAR_CLIMB)
        {
            //鉄棒が持っているベクトルを上に設定
            tr.up = other.GetComponent<IronBar>().GetPoleVector();

            //POLE方を向かせる
            tr.position = new Vector3(tr.position.x, player.position.y, tr.position.z);
            Vector3 poleDirection = other.transform.position;
            poleDirection.y = player.position.y;

           // player.up = Vector3.Normalize(other.GetComponent<IronBar>().GetPoleVector());

            player.rotation = Quaternion.LookRotation(player.forward, Vector3.Normalize(other.GetComponent<IronBar>().GetPoleVector()));

            //player.LookAt(poleDirection);
            //player.rotation = Quaternion.LookRotation(tr.position, tr.up);

            //プレイヤーの親を自分に
            player.parent = tr;
        }

        isHit = false;
    }


    public void SetPlayerDirection(Vector3 down, Vector3 direction)
    {
        playerDirection = down;
        this.direction = direction;
        //print(this.direction);
    }

    public void SetIsHit(bool hit)
    {
        isHit = hit;
    }
}
