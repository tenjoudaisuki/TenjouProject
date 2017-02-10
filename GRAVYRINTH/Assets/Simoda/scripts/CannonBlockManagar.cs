using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonBlockManagar : MonoBehaviour
{
    private bool isSetAll = false;
    private int setCount = 0;
    private List<CannonBlock> cannonBlocks = new List<CannonBlock>();
    private int isSetTrueCount = 0;

    public bool isDebug = false;
    public GameObject DrawUI;

    void Start()
    {
        cannonBlocks.AddRange(
            GameObject.FindGameObjectWithTag("Taihou").GetComponentsInChildren<CannonBlock>());

        if (isDebug == true)
        {
            foreach (CannonBlock block in cannonBlocks)
            {
                block.gameObject.transform.position = block.cannonSetPoint.position;
                block.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            }
        }
    }

    void Update()
    {
        if (cannonBlocks.Count == setCount)
        {
            isSetAll = true;
        }
        else
        {
            isSetAll = false;
        }
    }

    public void IsSetTrue()
    {
        setCount++;
        isSetTrueCount++;
        if (isSetTrueCount == 1) return;
        Instantiate(DrawUI);
    }

    public void IsSetFalse()
    {
        setCount--;
    }

    public bool GetIsSetAll()
    {
        return isSetAll;
    }
}
