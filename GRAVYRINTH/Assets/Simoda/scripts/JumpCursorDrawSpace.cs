using UnityEngine;
using System.Collections;

public class JumpCursorDrawSpace : MonoBehaviour
{
    private JumpCursorDraw jumpCursor;

    void Start()
    {
        jumpCursor = GameObject.Find("JumpCursor").GetComponent<JumpCursorDraw>();
    }

    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            jumpCursor.ChangeType(JumpCursorDraw.CursorType.Space);
            jumpCursor.IsHit(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            jumpCursor.IsHit(false);
        }
    }
}
