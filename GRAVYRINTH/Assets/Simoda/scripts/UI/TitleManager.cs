using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    private Scene title;

    private RectTransform titleTextBack;
    private RectTransform titleLogoBack;
    private RectTransform titleLogo;
    private RectTransform pressStartButtonBack;
    private RectTransform pressStartButton;

    private bool isSkip = true;

    void Start()
    {
        //StartCoroutine(DelayMethod(0, () => { SceneManager.LoadScene("TutorialTestSimoda", LoadSceneMode.Additive); }));
        //StartCoroutine(DelayMethod(1, () => { SceneManager.SetActiveScene(TutorialTest); }));
        //StartCoroutine(DelayMethod(0, () => { GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Title); }));
        title = SceneManager.GetSceneByName("Title");

        titleTextBack = GameObject.Find("TitleTextBack").GetComponent<RectTransform>();
        titleLogoBack = GameObject.Find("TitleLogoBack").GetComponent<RectTransform>();
        titleLogo = GameObject.Find("TitleLogo").GetComponent<RectTransform>();
        pressStartButtonBack = GameObject.Find("PressStartButtonBack").GetComponent<RectTransform>();
        pressStartButton = GameObject.Find("PressStartButton").GetComponent<RectTransform>();

        //titleLogoBackの位置は（0,-240）
        //LeanTween.move(titleLogoBack, new Vector2(0.0f, -50.0f), 2.0f)
        //    .setOnComplete(() =>
        //    {
        //        titleLogo.GetComponent<ImageFlashing>().FlashingIn(titleLogo, 2.0f, 0.0f, 1.0f);
        //        LeanTween.alpha(pressStartButton, 1.0f, 2.0f)
        //        .setOnComplete(() =>
        //        {
        //            pressStartButtonBack.GetComponent<ImageFlashing>().FlashingIn(pressStartButtonBack, 1.0f, 0.0f, 0.0f);
        //        });
        //    });
        LeanTween.alpha(titleTextBack, 1.0f, 1.0f)
            .setOnComplete(() =>
            {
                LeanTween.alpha(titleLogoBack, 1.0f, 2.0f)
                    .setOnComplete(() =>
                    {
                        titleLogo.GetComponent<ImageFlashing>().FlashingIn(titleLogo, 2.0f, 0.0f, 1.0f);
                        LeanTween.alpha(pressStartButton, 1.0f, 2.0f)
                        .setOnComplete(() =>
                        {
                            pressStartButtonBack.GetComponent<ImageFlashing>().FlashingIn(pressStartButtonBack, 1.0f, 0.0f, 0.0f);
                            isSkip = false;
                        });
                    });
            });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSkip == true)
            {
                RectTransform[] rectTransforms = GameObject.Find("Frame").GetComponentsInChildren<RectTransform>();
                foreach (RectTransform rectTr in rectTransforms)
                {
                    LeanTween.alpha(rectTr, 1.0f, 0.0f);
                }

                StartCoroutine(DelayMethod(0, () =>
                {
                    isSkip = false;
                }));
            }
            else
            {
                titleLogo.GetComponent<ImageFlashing>().FlashingStop(1.0f);
                pressStartButtonBack.GetComponent<ImageFlashing>().FlashingStop(1.0f);
                LeanTween.scale(pressStartButtonBack, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);
                LeanTween.scale(pressStartButton, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

                RectTransform[] rectTransforms = GameObject.Find("Frame").GetComponentsInChildren<RectTransform>();
                //rectTransforms.GetValue()

                foreach (RectTransform rectTr in rectTransforms)
                {
                    LeanTween.alpha(rectTr, 0.0f, 1.0f)
                        .setOnComplete(() =>
                        {
                            try
                            {
                                GameObject.Find("Frame").active = false;
                            }
                            catch
                            {
                                // 16/12/12 add 西--------------------------------------------------
                                GameManager.Instance.GameModeChange(GameManager.GameMode.Select);
                                //------------------------------------------------------------------
                                SceneManager.UnloadScene(title);
                            }
                        });
                }
            }
        }
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="delayFrameCount"></param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(int delayFrameCount, System.Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }
        action();
    }
}
