using UnityEngine;
using System.Collections;

public class IronBar : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;
    public Vector3 barVector;

    private Transform player;

    void Start()
    {
        barVector = point2.transform.position - point1.transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {

    }

    public Vector3 GetBarVector()
    {
        float angle = Vector3.Angle(player.right, player.position + barVector);
        print(angle);
        if (angle > 90.0f)
            barVector = -barVector;

        return barVector;
    }
}
