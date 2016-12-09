using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private Scene menu;

    private RectTransform menuBack;
    private RectTransform textBack;
    private RectTransform startGame;
    private RectTransform selectStage;
    private RectTransform exitGame;
    private RectTransform back;

    private List<RectTransform> rectTransforms = new List<RectTransform>();

    //private RectTransform previousSelectedObject;
    private RectTransform currentSelectedObject;

    //メニューの項目を変更中かどうか
    private bool changingSelection = false;

    void Start()
    {
        //Menuシーン（自身のシーン）を検索
        menu = SceneManager.GetSceneByName("Menu");

        //UIの検索　代入
        menuBack = GameObject.Find("MenuBack").GetComponent<RectTransform>();
        textBack = GameObject.Find("TextBack").GetComponent<RectTransform>();
        startGame = GameObject.Find("StartGame").GetComponent<RectTransform>();
        selectStage = GameObject.Find("SelectStage").GetComponent<RectTransform>();
        exitGame = GameObject.Find("ExitGame").GetComponent<RectTransform>();
        back = GameObject.Find("Back").GetComponent<RectTransform>();

        //ListにFrameの中にあるUIのRectTransform全てを追加
        rectTransforms.AddRange(GameObject.Find("Frame").GetComponentsInChildren<RectTransform>());
        //ListにFrameも入ってしまっているので削除（中身を子のみにする）
        rectTransforms.Remove(GameObject.Find("Frame").GetComponent<RectTransform>());

        //Listの中身のAlphaを2.0秒かけて1.0にTween
        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 1.0f, 1.0f);
        }

        //初期に選択されている項目を設定
        EventSystem.current.SetSelectedGameObject(startGame.gameObject);
    }

    void Update()
    {
    }

    void LateUpdate()
    {
        //メニューの項目を変更中でなければ実行
        if (changingSelection == false)
        {
            //↑入力時、処理
            if (Input.GetAxis("Vertical") > 0.2f)
            {
                //メニューの項目を更新
                EventSystem.current.SetSelectedGameObject(
                    EventSystem.current.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnUp.gameObject);

                //メニューの項目を変更中に変更
                changingSelection = true;

                //現在選択されているメニューを更新
                currentSelectedObject =
                    EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

                //TextBackを現在選択されているメニューの位置へ移動
                LeanTween.move(textBack, currentSelectedObject.anchoredPosition, 0.5f)
                    .setEase(LeanTweenType.easeInOutCubic) //補間曲線の設定
                    .setOnComplete(() => //Tween終了後実行
                    {
                        changingSelection = false;
                    });
            }
            //↓入力時、処理
            else if (Input.GetAxis("Vertical") < -0.2f)
            {
                //メニューの項目を更新
                EventSystem.current.SetSelectedGameObject(
                    EventSystem.current.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnDown.gameObject);

                //メニューの項目を変更中に変更
                changingSelection = true;

                //現在選択されているメニューを更新
                currentSelectedObject =
                    EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

                //TextBackを現在選択されているメニューの位置へ移動
                LeanTween.move(textBack, currentSelectedObject.anchoredPosition, 0.5f)
                    .setEase(LeanTweenType.easeInOutCubic) //補間曲線の設定
                    .setOnComplete(() => //Tween終了後実行
                    {
                        changingSelection = false;
                    });
            }
        }
        ////過去に選択されていたメニューを更新
        //previousSelectedObject = currentSelectedObject;

        ////現在選択されているメニューを更新
        //currentSelectedObject =
        //    EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

        ////何も選択されていなかったら処理しない
        //if (currentSelectedObject == null) return;
        ////過去の選択と現在の選択が同じならば処理しない
        //if (currentSelectedObject == previousSelectedObject) return;

        ////TextBackを現在選択されているメニューの位置へ移動
        //LeanTween.move(textBack, currentSelectedObject.anchoredPosition, 0.5f)
        //    .setEase(LeanTweenType.easeInOutCubic) //補間曲線の設定
        //    .setOnComplete(() => //Tween終了後実行
        //    {
        //        changingSelection = false;
        //    });
    }

    public void StartGameButtonPressed()
    {
        LeanTween.scale(startGame, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 0.0f, 1.0f);
        }

        StartCoroutine(DelayMethod(1, () =>
        {
            SceneManager.UnloadScene(menu);
        }));

    }

    public void SelectStageButtonPressed()
    {
        LeanTween.scale(selectStage, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

        rectTransforms.Remove(menuBack);
        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 0.0f, 1.0f);
        }
    }

    public void ExitGameButtonPressed()
    {
        LeanTween.scale(exitGame, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

        rectTransforms.Remove(menuBack);
        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 0.0f, 1.0f);
        }
    }

    public void BackButtonPressed()
    {

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
