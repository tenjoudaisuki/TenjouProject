using UnityEngine;
using System.Collections;

public class BlockCursorDraw : MonoBehaviour
{
    private GameObject blockCursor;
    private Transform player;
    private Transform tr;
    private Vector3 offset;
    private MeshRenderer cursorRenderer;
    //private bool drawDistance;

    public GameObject blockCursorPrefab;

    void Start()
    {
        player = GameObject.Find("Player").transform;

        tr = gameObject.transform;

        offset = new Vector3(
            0.0f,
            gameObject.GetComponent<Block>().offsetY,
            0.0f);

        // drawDistance = true;

        blockCursor = Instantiate(blockCursorPrefab);
        cursorRenderer = blockCursor.GetComponent<MeshRenderer>();
        cursorRenderer.enabled = false;
    }

    void Update()
    {
        BlockCursorControl();
    }

    private void BlockCursorControl()
    {
        float distance = Vector3.Distance(tr.position, player.position + offset);

        if (distance <= GetComponent<Block>().pushDistance)
        {
            cursorRenderer.enabled = true;
            blockCursor.transform.position = player.position + player.up * 0.8f;

            //if (drawDistance == true)
            blockCursor.transform.forward = Camera.main.transform.forward;
            blockCursor.transform.Rotate(-90.0f, 0.0f, 0.0f);
            //blockCursor.transform.Rotate(new Vector3(0.0f, 0.0f, -45.0f * Time.deltaTime));

            //drawDistance = false;

            //blockCursor.transform.LookAt(Camera.main.transform);
            //Quaternion a = Quaternion.LookRotation(Camera.main.transform.forward);

            //Vector3 a = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 1, 1)).normalized;
            //blockCursor.transform.localRotation = Quaternion.Euler(a);
            //player.rotation = Quaternion.LookRotation(forward, -direction);
        }
        else
        {
            cursorRenderer.enabled = false;
        }
    }
}
