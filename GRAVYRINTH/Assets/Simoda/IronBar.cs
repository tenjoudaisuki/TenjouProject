using UnityEngine;
using System.Collections;

public class IronBar : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;

    public Vector3 barVector;

    void Start()
    {
        barVector = point2.transform.position - point1.transform.position;
    }

    void Update()
    {

    }

    public Vector3 GetBarVector()
    {
        return barVector;
    }
}
