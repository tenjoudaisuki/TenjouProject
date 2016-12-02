using UnityEngine;
using System.Collections;

public class ClearPoint : MonoBehaviour
{
    public string mNextStageName;
    public Color mFadeColor;

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            Clear();
            other.GetComponent<NormalMove>().NormalToStageClear();
        }
    }

    void Clear()
    {
        GameObject camera = GameObject.Find("Camera");
        GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeFactory>().FadeColorChange(mFadeColor);
        camera.GetComponent<CameraManager>().StateChange(State.Clear);
        GameManager.Instance.SetNextSceneName(mNextStageName);
    }
}
