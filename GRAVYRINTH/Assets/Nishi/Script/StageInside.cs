using UnityEngine;
using System.Collections;

public class StageInside : MonoBehaviour
{

    bool isOutside = false;

    public bool GetPlayerOutside()
    {
        return isOutside;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.name == "GameObject") isOutside = true;
        Debug.Log(isOutside);
    }
}
