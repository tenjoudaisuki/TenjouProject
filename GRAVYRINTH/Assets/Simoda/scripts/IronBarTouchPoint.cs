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
    public Transform player;
    public float offsetY;

    void Start()
    {
        tr = gameObject.transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = Vector3.zero;
        Debug.DrawRay(tr.position, direction, Color.green);
        //if (isHit == true)
        //    player.position = tr.position + direction * offsetY;

    }

    public void OnTriggerStay(Collider other)
    {
        print("Hit");
        //print(other.gameObject.tag + " " + other.gameObject.transform.position);

        if (other.gameObject.tag != "IronBar") return;

        if (isHit == true)
            tr.position += playerDirection * 0.01f;
    }

    public void OnTriggerExit(Collider other)
    {
        //player.position = tr.position + direction * offsetY;
        //headPoint.GetComponent<PlayerHeadPoint>().SetHeadPoint(tr.position);
        TriggerExit(other);
    }

    public void TriggerExit(Collider other)
    {
        tr.right = other.GetComponent<IronBar>().GetBarVector();
        player.parent = tr;
        if (player.GetComponent<PlayerIronBar>().barType == PlayerIronBar.BarType.IRON_BAR)
        {
            player.localPosition = new Vector3(0, -6.2f, 0);
            player.up = -direction;
            player.localRotation = Quaternion.Euler(player.localRotation.eulerAngles.x, 0.0f, player.localRotation.eulerAngles.z);
        }
        isHit = false;
    }


    public void SetPlayerDirection(Vector3 down, Vector3 direction)
    {
        playerDirection = down;
        this.direction = direction;
        print(this.direction);
    }

    public void SetIsHit(bool hit)
    {
        isHit = hit;
    }
}
