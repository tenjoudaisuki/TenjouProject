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
                Destroy(this);
            }
            else
            {
                Clear();
                SoundManager.Instance.PlaySe("goal1");
                SoundManager.Instance.PlaySe("goal2");
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
