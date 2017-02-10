using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageEvent : MonoBehaviour
{
    public enum EventTextType
    {
        SingleText,
        MultiText,
        SingleSerifText,
    }
    public EventTextType eventTextType;
    public bool isSerifText = false;
    //public float serifDrawTime = 1.0f;

    //UIの入力管理
    private UIInputManager input;

    //メニューの項目を変更中かどうか
    private bool isChanging = true;
    private EventCamera eventCamera;

    private RectTransform background;
    private RectTransform next;
    private List<RectTransform> rectTransforms = new List<RectTransform>();

    public List<RectTransform> texts = new List<RectTransform>();
    public int textNumbar = 0;

    private UIandCameraSync syncsystem;

    void Start()
    {
        syncsystem = GetComponent<UIandCameraSync>();

        //UIInputManagerを取得
        input = GetComponent<UIInputManager>();

        //ListにFrameの中にあるUIのRectTransform全てを追加
        rectTransforms.AddRange(GameObject.Find("Frame").GetComponentsInChildren<RectTransform>());
        //ListにFrameも入ってしまっているので削除（中身を子のみにする）
        rectTransforms.Remove(GameObject.Find("Frame").GetComponent<RectTransform>());

        eventCamera = Camera.main.GetComponent<EventCamera>();

        //1.0秒後に変更しているかどうかをfalseに
        //StartCoroutine(DelayMethod(1.0f, () =>
        //{
        //    changingSelection = false;
        //}));

        if (isSerifText == true)
        {
            //if (texts.Count >= 2)
            //{
            //    //EventTextTypeを設定
            //    eventTextType = EventTextType.MultiText;
            //}
            //else
            //{
                //EventTextTypeを設定
                eventTextType = EventTextType.SingleSerifText;
            //}
        }
        else
        {
            if (texts.Count >= 2)
            {
                //EventTextTypeを設定
                eventTextType = EventTextType.MultiText;
            }
            else
            {
                //EventTextTypeを設定
                eventTextType = EventTextType.SingleText;
            }
        }

        switch (eventTextType)
        {
            case EventTextType.SingleText:
                SingleTextInitialize();
                break;

            case EventTextType.MultiText:
                MultiTextInitialize();
                break;

            case EventTextType.SingleSerifText:
                SingleSerifTextInitialize();
                break;
        }
    }

    void Update()
    {
        //switch (eventTextType)
        //{
        //    case EventTextType.SingleText:
        //        //SingleTextInput();
        //        break;

        //    case EventTextType.MultiText:
        //        //MultiTextInput();
        //        break;
        //}


    }

    private void SingleTextInitialize()
    {
        //Backgroundの検索　代入
        background = GameObject.Find("EventBackground").GetComponent<RectTransform>();
        next = GameObject.Find("Next").GetComponent<RectTransform>();

        //Listの中身のAlphaを1.0秒かけて1.0にTween
        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 1.0f, 1.0f);
        }

        StartCoroutine(DelayMethod(1.1f, () =>
        {
            syncsystem.SetUIAction(() => { input.Submit(); });
            isChanging = false;
        }));

        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("enter");

            isChanging = true;

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                Destroy(gameObject);
            }));
        });
    }

    private void SingleTextInput()
    {
        //メニューの項目を変更中ならば処理しない
        if (isChanging == true || !eventCamera.IsCameraMoveEnd()) return;

        if (Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return))
        {
            //input.Submit();
        }
    }

    private void MultiTextInitialize()
    {
        //Backgroundの検索　代入
        background = GameObject.Find("EventBackground").GetComponent<RectTransform>();
        next = GameObject.Find("Next").GetComponent<RectTransform>();

        for (int i = 1; i < texts.Count; i++)
        {
            rectTransforms.Remove(texts[i]);
        }

        //Listの中身のAlphaを1.0秒かけて1.0にTween
        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 1.0f, 1.0f);
        }

        StartCoroutine(DelayMethod(1.1f, () =>
        {
            syncsystem.SetUIAction(() => { input.Submit(); });
            isChanging = false;
        }));

        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("enter");

            isChanging = true;

            //rectTransformsからText以外を削除
            rectTransforms.Remove(background);
            rectTransforms.Remove(next);

            LeanTween.alpha(texts[textNumbar], 0.0f, 1.0f);

            rectTransforms.Remove(texts[textNumbar]);
            textNumbar++;
            rectTransforms.Add(texts[textNumbar]);

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                LeanTween.alpha(texts[textNumbar], 1.0f, 1.0f);

                StartCoroutine(DelayMethod(1.1f, () =>
                {
                    isChanging = false;
                    if (textNumbar < texts.Count - 1)
                    {
                        input.SetSubmitAction(() =>
                        {
                            SoundManager.Instance.PlaySe("enter");

                            isChanging = true;

                            LeanTween.alpha(texts[textNumbar], 0.0f, 1.0f);

                            rectTransforms.Remove(texts[textNumbar]);
                            textNumbar++;
                            rectTransforms.Add(texts[textNumbar]);

                            StartCoroutine(DelayMethod(1.1f, () =>
                            {
                                LeanTween.alpha(texts[textNumbar], 1.0f, 1.0f);

                                StartCoroutine(DelayMethod(1.1f, () =>
                                {
                                    isChanging = false;
                                }));
                            }));
                        });
                    }
                    else
                    {
                        input.SetSubmitAction(() =>
                        {
                            SoundManager.Instance.PlaySe("enter");

                            isChanging = true;

                            rectTransforms.Add(background);
                            rectTransforms.Add(next);

                            foreach (RectTransform rectTr in rectTransforms)
                            {
                                LeanTween.alpha(rectTr, 0.0f, 1.0f);
                            }

                            StartCoroutine(DelayMethod(1.1f, () =>
                            {
                                Destroy(gameObject);
                            }));
                        });
                    }
                }));
            }));
        });
    }

    private void MultiTextInput()
    {
        //メニューの項目を変更中ならば処理しない
        if (isChanging == true || !eventCamera.IsCameraMoveEnd()) return;

        //if (Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return))
        {
            input.Submit();
            if (textNumbar < texts.Count - 1)
            {
                input.SetSubmitAction(() =>
                {
                    SoundManager.Instance.PlaySe("enter");

                    isChanging = true;

                    LeanTween.alpha(texts[textNumbar], 0.0f, 1.0f);

                    rectTransforms.Remove(texts[textNumbar]);
                    textNumbar++;
                    rectTransforms.Add(texts[textNumbar]);

                    StartCoroutine(DelayMethod(1.1f, () =>
                    {
                        LeanTween.alpha(texts[textNumbar], 1.0f, 1.0f);

                        StartCoroutine(DelayMethod(1.1f, () =>
                        {
                            isChanging = false;
                        }));
                    }));
                });
            }
            else
            {
                input.SetSubmitAction(() =>
                {
                    SoundManager.Instance.PlaySe("enter");

                    isChanging = true;

                    rectTransforms.Add(background);
                    rectTransforms.Add(next);

                    foreach (RectTransform rectTr in rectTransforms)
                    {
                        LeanTween.alpha(rectTr, 0.0f, 1.0f);
                    }

                    StartCoroutine(DelayMethod(1.1f, () =>
                    {
                        Destroy(gameObject);
                    }));
                });
            }
        }
    }

    private void SingleSerifTextInitialize()
    {
        //Listの中身のAlphaを1.0秒かけて1.0にTween
        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 1.0f, 1.0f);
        }

        StartCoroutine(DelayMethod(2.1f, () =>
        {
            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }
        }));

        StartCoroutine(DelayMethod(3.2f, () =>
        {
            foreach (RectTransform rectTr in rectTransforms)
            {
                Destroy(gameObject);
            }
        }));
    }

    //public void ChangeEnd()
    //{
    //    changingSelection = false;
    //}

    /// <summary>
    /// イベントが終了したかどうか？
    /// </summary>
    /// <returns>false = イベント終了</returns>
    public bool IsChanging()
    {
        return isChanging;
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間[ミリ秒]</param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float waitTime, System.Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
