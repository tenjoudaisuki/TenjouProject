using UnityEngine;
using System.Collections;

public class BlockCursor : MonoBehaviour
{
    private GameObject blockCursor;
    private Transform player;
    private Transform tr;
    private Vector3 offset;
    private MeshRenderer blockRenderer;
    private bool drawDistance;

    public GameObject blockCursorPrefab;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        tr = gameObject.transform;
        offset = new Vector3(
            0.0f,
            gameObject.GetComponent<Block>().offsetY,
            0.0f);
        drawDistance = true;

        blockCursor = Instantiate(blockCursorPrefab);
        blockRenderer = blockCursor.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        BlockCursorControl();
    }

    private void BlockCursorControl()
    {
        float distance = Vector3.Distance(tr.position, player.position + offset);

        if (distance <= 2.0f)
        {
            blockRenderer.enabled = true;
            blockCursor.transform.position = tr.position + player.up * 0.5f;

            if (drawDistance == true)
                blockCursor.transform.forward = -player.up;

            blockCursor.transform.Rotate(new Vector3(0.0f, 0.0f, -45.0f * Time.deltaTime));

            drawDistance = false;

            //blockCursor.transform.LookAt(Camera.main.transform);
            //Quaternion a = Quaternion.LookRotation(Camera.main.transform.forward);

            //Vector3 a = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 1, 1)).normalized;
            //blockCursor.transform.localRotation = Quaternion.Euler(a);
            //player.rotation = Quaternion.LookRotation(forward, -direction);
        }
        else
        {
            blockRenderer.enabled = false;
            drawDistance = true;
        }
    }
}
