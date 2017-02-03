using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageEvent : MonoBehaviour
{
    public enum EventTextType
    {
        SingleText,
        MultiText,
    }
    public EventTextType eventTextType;

    //UIの入力管理
    private UIInputManager input;

    //メニューの項目を変更中かどうか
    private bool changingSelection = true;

    private RectTransform background;
    private RectTransform next;
    private List<RectTransform> rectTransforms = new List<RectTransform>();

    public List<RectTransform> texts = new List<RectTransform>();
    public int textNumbar = 0;

    void Start()
    {
        //UIInputManagerを取得
        input = GetComponent<UIInputManager>();

        //ListにFrameの中にあるUIのRectTransform全てを追加
        rectTransforms.AddRange(GameObject.Find("Frame").GetComponentsInChildren<RectTransform>());
        //ListにFrameも入ってしまっているので削除（中身を子のみにする）
        rectTransforms.Remove(GameObject.Find("Frame").GetComponent<RectTransform>());

        //Backgroundの検索　代入
        background = GameObject.Find("Background").GetComponent<RectTransform>();
        next = GameObject.Find("Next").GetComponent<RectTransform>();

        //1.0秒後に変更しているかどうかをfalseに
        StartCoroutine(DelayMethod(1.0f, () =>
        {
            changingSelection = false;
        }));

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

        switch (eventTextType)
        {
            case EventTextType.SingleText:
                //Listの中身のAlphaを1.0秒かけて1.0にTween
                foreach (RectTransform rectTr in rectTransforms)
                {
                    LeanTween.alpha(rectTr, 1.0f, 1.0f);
                }

                input.SetSubmitAction(() =>
                {
                    SoundManager.Instance.PlaySe("enter");

                    changingSelection = true;

                    foreach (RectTransform rectTr in rectTransforms)
                    {
                        LeanTween.alpha(rectTr, 0.0f, 1.0f);
                    }

                    StartCoroutine(DelayMethod(1.1f, () =>
                    {
                        Destroy(gameObject);
                    }));
                });
                break;

            case EventTextType.MultiText:
                for (int i = 1; i < texts.Count; i++)
                {
                    rectTransforms.Remove(texts[i]);
                }

                //Listの中身のAlphaを1.0秒かけて1.0にTween
                foreach (RectTransform rectTr in rectTransforms)
                {
                    LeanTween.alpha(rectTr, 1.0f, 1.0f);
                }

                input.SetSubmitAction(() =>
                {
                    SoundManager.Instance.PlaySe("enter");

                    changingSelection = true;

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
                            changingSelection = false;
                        }));
                    }));
                });
                break;
        }
    }

    void Update()
    {
        switch (eventTextType)
        {
            case EventTextType.SingleText:
                //メニューの項目を変更中ならば処理しない
                if (changingSelection == true) return;

                if (Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return))
                {
                    input.Submit();
                }
                break;

            case EventTextType.MultiText:
                //メニューの項目を変更中ならば処理しない
                if (changingSelection == true) return;

                if (Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return))
                {
                    input.Submit();
                    if (textNumbar < texts.Count - 1)
                    {
                        input.SetSubmitAction(() =>
                        {
                            SoundManager.Instance.PlaySe("enter");

                            changingSelection = true;

                            LeanTween.alpha(texts[textNumbar], 0.0f, 1.0f);

                            rectTransforms.Remove(texts[textNumbar]);
                            textNumbar++;
                            rectTransforms.Add(texts[textNumbar]);

                            StartCoroutine(DelayMethod(1.1f, () =>
                            {
                                LeanTween.alpha(texts[textNumbar], 1.0f, 1.0f);

                                StartCoroutine(DelayMethod(1.1f, () =>
                                {
                                    changingSelection = false;
                                }));
                            }));
                        });
                    }
                    else
                    {
                        input.SetSubmitAction(() =>
                        {
                            SoundManager.Instance.PlaySe("enter");

                            changingSelection = true;

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
                break;
        }


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
