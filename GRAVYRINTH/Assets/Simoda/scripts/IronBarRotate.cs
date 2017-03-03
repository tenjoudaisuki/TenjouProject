using UnityEngine;
using System.Collections;

public class IronBarRotate : MonoBehaviour
{
    public enum RotateType
    {
        ALWAYS,
        TOUCH,
    }

    [SerializeField, Tooltip("ALWAYS:常に回っている、TOUCH:振れている間常に回っている")]
    public RotateType rotateType;

    [SerializeField, Tooltip("回転軸")]
    public Vector3 axis = Vector3.zero;
    [SerializeField, Tooltip("回転速度")]
    public float angle = 45.0f;

    private Transform player;
    private Transform tr;

    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        switch (rotateType)
        {
            case RotateType.ALWAYS:
                tr.RotateAround(tr.position, axis, angle * Time.deltaTime);
                if (player.GetComponent<PlayerMoveManager>().GetState() == PlayerState.IRON_BAR_DANGLE
                    || player.GetComponent<PlayerMoveManager>().GetState() == PlayerState.IRON_BAR_CLIMB)
                {
                    player.RotateAround(tr.position, axis, angle * Time.deltaTime);

                }

                break;

            case RotateType.TOUCH:
                if (player.GetComponent<PlayerMoveManager>().GetState() == PlayerState.IRON_BAR_DANGLE
                    || player.GetComponent<PlayerMoveManager>().GetState() == PlayerState.IRON_BAR_CLIMB)
                {
                    player.RotateAround(tr.position, axis, angle * Time.deltaTime);
                    tr.RotateAround(tr.position, axis, angle * Time.deltaTime);
                }
                break;
        }
    }
}
