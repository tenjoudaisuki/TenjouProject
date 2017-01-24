using UnityEngine;
using System.Collections;

public class BlockManager : MonoBehaviour
{
    public float pushDistancePlus = 0.5f;
    public float distanceToWallPlus = 0.0f;
    private float distanceToWallDivide = 14.0f;

    void Start()
    {

    }

    void Update()
    {

    }

    public float GetPushDistancePlus()
    {
        return pushDistancePlus;
    }

    public float GetDistanceToWallPlus()
    {
        return distanceToWallPlus;
    }

    public float GetDistanceToWallDivide()
    {
        return distanceToWallDivide;
    }
}
