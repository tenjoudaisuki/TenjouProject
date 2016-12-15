using UnityEngine;
using System.Collections;

public class BlockCursorDraw : MonoBehaviour
{
    private GameObject blockCursor;
    private Transform player;
    private Transform tr;
    private MeshRenderer cursorRenderer;

    //public GameObject blockCursorPrefab;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        tr = gameObject.transform;

        blockCursor = GameObject.Find("BlockCursor");
        cursorRenderer = blockCursor.GetComponent<MeshRenderer>();
        cursorRenderer.enabled = false;
    }

    void Update()
    {

    }

    public void BlockCursorControl(float currentDistance, float pushDistance)
    {
        if (currentDistance <= pushDistance)
        {
            //表示をする
            cursorRenderer.enabled = true;
            //位置をプレイヤーの頭の上へ
            blockCursor.transform.position = player.position + player.up * 0.8f;

            //常にカメラの方向を見るように回転
            blockCursor.transform.forward = Camera.main.transform.forward;
            blockCursor.transform.Rotate(-90.0f, 0.0f, 0.0f);
        }
        else
        {
            //表示しない
            cursorRenderer.enabled = false;
        }
    }

    public void NotShow()
    {
        //表示しない
        cursorRenderer.enabled = false;
    }
}
