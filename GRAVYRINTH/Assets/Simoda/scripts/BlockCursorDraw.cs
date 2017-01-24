using UnityEngine;
using System.Collections;

public class BlockCursorDraw : MonoBehaviour
{
    public enum BlockCursorType
    {
        None,
        Block,
        CannonBlock,
    }

    public BlockCursorType blockCursorType;

    //private GameObject blockCursor;
    private Transform player;
    private Transform tr;
    private MeshRenderer cursorRenderer;
    private Vector3 offset;
    public GameObject targetBlock;

    public void Awake()
    {
        ChangeBlockCursorType(BlockCursorType.None);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        tr = gameObject.transform;

        cursorRenderer = GetComponent<MeshRenderer>();
        cursorRenderer.enabled = false;
    }

    void Update()
    {
        if (blockCursorType == BlockCursorType.None) return;

        if (targetBlock == null)
            Destroy(gameObject);

        switch (blockCursorType)
        {
            case BlockCursorType.Block:
                BlockCursorControl();
                break;

            case BlockCursorType.CannonBlock:
                CannonBlockCursorControl();
                break;
        }
    }

    public void BlockCursorControl()
    {
        if (targetBlock == null) return;

        float currentDistance = Vector3.Distance(targetBlock.transform.position, player.position + offset);

        if (currentDistance <= targetBlock.GetComponent<Block>().GetPushDistance()
            && player.GetComponent<NormalMove>().GetIsGroundHit()
            && Vector3.Angle(player.up, targetBlock.GetComponent<Block>().GetPlayerDirection().normal) >= 89.0f
            && Vector3.Angle(player.up, targetBlock.GetComponent<Block>().GetPlayerDirection().normal) <= 91.0f)
        {
            //表示をする
            cursorRenderer.enabled = true;
            //位置をプレイヤーの頭の上へ
            transform.position = player.position + player.up * 0.8f;

            //常にカメラの方向を見るように回転
            //transform.forward = Camera.main.transform.forward;
            //transform.Rotate(-90.0f, 0.0f, 0.0f);
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        }
        else
        {
            //表示しない
            cursorRenderer.enabled = false;
        }
    }

    public void CannonBlockCursorControl()
    {
        float currentDistance = Vector3.Distance(targetBlock.transform.position, player.position + offset);

        if (currentDistance <= targetBlock.GetComponent<CannonBlock>().GetPushDistance()
            && Vector3.Angle(player.up, targetBlock.GetComponent<CannonBlock>().GetPlayerDirection().normal) != 0.0f)
        {
            //表示をする
            cursorRenderer.enabled = true;
            //位置をプレイヤーの頭の上へ
            transform.position = player.position + player.up * 0.8f;

            //常にカメラの方向を見るように回転
            transform.forward = Camera.main.transform.forward;
            transform.Rotate(-90.0f, 0.0f, 0.0f);
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

    public void SetOffset(Vector3 offset)
    {
        this.offset = offset;
    }

    public void ChangeBlockCursorType(BlockCursorType type)
    {
        blockCursorType = type;
    }

    public void SetBlock(GameObject block)
    {
        targetBlock = block;
    }
}
