using UnityEngine;
using System.Collections;

public class JumpCursorDraw : MonoBehaviour
{
    private GameObject jumpCursor;
    private Transform player;
    private Transform tr;
    private Vector3 offset;
    private MeshRenderer cursorRenderer;
    public bool isHit = false;

    //public GameObject jumpCursorPrefab;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        tr = gameObject.transform;

        //offset = new Vector3(
        //    0.0f,
        //    gameObject.GetComponent<Block>().offsetY,
        //    0.0f);

        jumpCursor = GameObject.Find("JumpCursor");
        cursorRenderer = jumpCursor.GetComponent<MeshRenderer>();
        cursorRenderer.enabled = false;
    }

    void Update()
    {
        if (isHit == true)
            JumpCursorControl();
        else
            cursorRenderer.enabled = false;
    }

    private void JumpCursorControl()
    {
        cursorRenderer.enabled = true;
        jumpCursor.transform.position = player.position + player.up * 0.8f;

        jumpCursor.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
    }

    public void IsHit(bool hit)
    {
        isHit = hit;
    }
}
