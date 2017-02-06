using UnityEngine;
using System.Collections;

public class JumpCursorDraw : MonoBehaviour
{
    public enum CursorType
    {
        Space,
        IronBar,
        Pole,
    }
    private CursorType cursorType;

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

        cursorType = CursorType.Space;

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
        if (GameObject.FindGameObjectWithTag("Fade").transform.childCount != 0)
        {
            isHit = false;
        }

        if (isHit == true)
            JumpCursorControl();
        else
            cursorRenderer.enabled = false;
    }

    private void JumpCursorControl()
    {
        cursorRenderer.enabled = true;
        switch (cursorType)
        {
            case CursorType.Space:
                jumpCursor.transform.position = player.position + player.up * 0.8f;
                break;

            case CursorType.IronBar:
                jumpCursor.transform.position = player.position + player.up * 0.8f + player.forward * 0.3f;
                break;

            case CursorType.Pole:
                jumpCursor.transform.position = player.position + player.up * 0.8f + -Camera.main.transform.forward * 0.4f;
                break;
        }


        jumpCursor.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
    }

    public void ChangeType(CursorType type)
    {
        cursorType = type;
    }

    public void IsHit(bool hit)
    {
        isHit = hit;
    }
}
