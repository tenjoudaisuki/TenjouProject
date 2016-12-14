using UnityEngine;
using System.Collections;

public class JumpCursorDraw : MonoBehaviour
{
    private GameObject jumpCursor;
    private Transform player;
    private Transform tr;
    private Vector3 offset;
    private MeshRenderer cursorRenderer;
    private bool isTrigger = false;

    public GameObject jumpCursorPrefab;

    void Start()
    {
        player = GameObject.Find("Player").transform;

        tr = gameObject.transform;

        //offset = new Vector3(
        //    0.0f,
        //    gameObject.GetComponent<Block>().offsetY,
        //    0.0f);

        jumpCursor = Instantiate(jumpCursorPrefab);
        cursorRenderer = jumpCursor.GetComponent<MeshRenderer>();
        cursorRenderer.enabled = false;
    }

    void Update()
    {
        if (isTrigger == true)
            JumpCursorControl();
    }

    private void JumpCursorControl()
    {
        cursorRenderer.enabled = true;
        jumpCursor.transform.position = player.position + player.up * 0.8f;

        jumpCursor.transform.forward = Camera.main.transform.forward;
        jumpCursor.transform.Rotate(-90.0f, 0.0f, 0.0f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            isTrigger = true;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isTrigger = false;
            cursorRenderer.enabled = false;
        }
    }
}
