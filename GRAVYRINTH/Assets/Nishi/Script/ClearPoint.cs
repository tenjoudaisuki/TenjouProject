using UnityEngine;
using System.Collections;

public class ClearPoint : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            Clear();
        }
    }

    void Clear()
    {
        GameObject camera = GameObject.Find("Camera");
        camera.GetComponent<CameraManager>().StateChange(State.Clear);
    }
}
