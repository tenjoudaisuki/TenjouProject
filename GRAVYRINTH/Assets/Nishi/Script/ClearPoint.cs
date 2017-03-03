using UnityEngine;
using System.Collections;

public class ClearPoint : MonoBehaviour
{
    public string mNextStageName;
    public Color mFadeColor;

    public bool isEnd = false;
    public float mFadeTime = 1.0f;
    public float mFadeBackTime = 0.5f;

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            PlayerFade(other.gameObject);
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

    private void PlayerFade(GameObject player)
    {
        LeanTween.value(1, 0, mFadeTime).setOnUpdate((float val) =>
        {
            player.GetComponentInChildren<Transparency>().enabled = false;
            var skr = player.GetComponentInChildren<SkinnedMeshRenderer>();
            var materials = skr.materials;
            for (int i = 0; i < 3; i++)
            {
                Color color = materials[i].color;
                color.a = val;
                materials[i].color = color;
            }
        }).setOnComplete(()=> {
            LeanTween.value(1, 0, mFadeTime).setOnComplete(() => { player.GetComponentInChildren<Transparency>().enabled = true; }).setDelay(mFadeBackTime);
        });
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="delayFrameCount"></param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float delaysecond, System.Action action)
    {
        yield return new WaitForSeconds(delaysecond);
        action();
    }
}
