using UnityEngine;
using System.Collections;

public class BlockHitAreaDraw : MonoBehaviour
{
    [SerializeField, TooltipAttribute("起動してからチェックを入れてください")]
    public bool isEnable = false;

    Transform tr;
    Block block;

    //ブロックの中心から端までの距離
    float distanceToWall = 0.0f;

    float xDistance;
    float yDistance;
    float zDistance;

    float xDistanceBox;
    float yDistanceBox;
    float zDistanceBox;

    public void Awake()
    {
        tr = GetComponent<Transform>();
        block = GetComponent<Block>();
    }

    void Start()
    {
        xDistance = Vector3.Distance(tr.position, tr.FindChild("x").position);
        yDistance = Vector3.Distance(tr.position, tr.FindChild("y").position);
        zDistance = Vector3.Distance(tr.position, tr.FindChild("z").position);

        xDistanceBox = xDistance;
        yDistanceBox = yDistance;
        zDistanceBox = zDistance;
    }

    void Update()
    {
        if (-block.GetPlayerDirection().normal == tr.right || -block.GetPlayerDirection().normal == -tr.right)
        {
            distanceToWall = xDistance;

            xDistanceBox = xDistance / 15;
            yDistanceBox = yDistance;
            zDistanceBox = zDistance;
        }

        if (-block.GetPlayerDirection().normal == tr.up || -block.GetPlayerDirection().normal == -tr.up)
        {
            distanceToWall = yDistance;

            xDistanceBox = xDistance;
            yDistanceBox = yDistance / 15;
            zDistanceBox = zDistance;
        }

        if (-block.GetPlayerDirection().normal == tr.forward || -block.GetPlayerDirection().normal == -tr.forward)
        {
            distanceToWall = zDistance;

            xDistanceBox = xDistance;
            yDistanceBox = yDistance;
            zDistanceBox = zDistance / 15;
        }

        distanceToWall += 0.025f;
    }



    public void OnDrawGizmos()
    {
        if (isEnable == false) return;

        Vector3 boxPos = tr.position + -block.GetPlayerDirection().normal * distanceToWall;
        //if (Physics.CheckBox(tr.position, new Vector3(xDistance, yDistance, zDistance), tr.rotation, layermask, QueryTriggerInteraction.Ignore))
        //{
        //    tr.position += GetPlayerDirection().normal * Time.deltaTime;
        //}

        Gizmos.DrawWireCube(boxPos, new Vector3(xDistanceBox, yDistanceBox, zDistanceBox) * 2);
    }
}
