using UnityEngine;
using System.Collections;

public class ClearPoint : MonoBehaviour
{
    public string mNextStageName;
    public Color mFadeColor;

    public bool isEnd = false;

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            if (isEnd)
            {
                GameObject camera = GameObject.Find("Camera");
                camera.GetComponent<CameraManager>().StateChange(State.Clear);
                GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeFactory>().FadeColorChange(mFadeColor);
                GameManager.Instance.GameModeChangTitleEx();
            }
            else
            {
                Clear();
                SoundManager.Instance.PlaySe("tinnitus1");
                SoundManager.Instance.PlaySe("something_call_to_you");
            }
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
