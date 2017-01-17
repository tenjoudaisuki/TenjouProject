using UnityEngine;
using System.Collections;

public class IronBar : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;

    private Vector3 barVector;
    private float moveArea;
    private Transform player;

    void Start()
    {
        barVector = point2.transform.position - point1.transform.position;
        barVector.Normalize();
        moveArea = Vector3.Distance(transform.position, point1.transform.position);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        barVector = point2.transform.position - point1.transform.position;
        barVector.Normalize();

        Debug.DrawRay(transform.position, barVector, Color.cyan);

        moveArea = Vector3.Distance(transform.position, point1.transform.position);
    }

    public Vector3 GetBarVector()
    {
        //print(Vector3.Angle(player.up, barVector));
        if (Vector3.Angle(player.up, barVector) > 120.0f)
            return -barVector;
        else
            return barVector;
    }

    public Vector3 GetIronBarVector()
    {
        float angle = Vector3.Angle(player.right, barVector);
        //print(angle);
        if (angle > 90.0f)
            return -barVector;
        else
            return barVector;
    }

    public Vector3 GetPoleVector()
    {
        float angle = Vector3.Angle(player.up, barVector);
        //print(angle);
        if (angle > 120.0f)
            return -barVector;
        else
            return barVector;
    }

    public float GetMoveArea()
    {
        return moveArea;
    }
}
