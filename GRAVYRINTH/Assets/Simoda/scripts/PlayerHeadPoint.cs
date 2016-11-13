using UnityEngine;
using System.Collections;

public class PlayerHeadPoint : MonoBehaviour
{
    private Transform tr;
    private bool touch = false;

    public float offsetY;
    public Transform player;

    void Start()
    {
        tr = gameObject.transform;
    }

    void Update()
    {
        //if (touch == false)
        //    tr.position = new Vector3(player.position.x, player.position.y + offset, player.position.z);
    }

    public void SetHeadPoint(Vector3 pos)
    {
        tr.position = pos;
    }

    public void PlayerTouch()
    {
        touch = true;
    }
}
