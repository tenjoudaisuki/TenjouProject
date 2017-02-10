using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public enum PauseMenuType
    {
        Menu,
        StageSelect,
        Controll,
    }

    public PauseMenuType menuType;

    //16/12/12 西 add　ステージの名前リスト
    public string[] mStageNameList;

    //private Scene menu;

    //UIの入力管理
    private UIInputManager input;

    //メニューシーン用
    private RectTransform background;
    private RectTransform menuTextBack;
    private RectTransform returnToGame;
    private RectTransform selectStage;
    private RectTransform controll;
    private RectTransform returnToTitle;
    private RectTransform exitGame;
    private RectTransform menuBack;
    private RectTransform menuHint;

    //ステージセレクトシーン用
    private RectTransform stageImage;
    private RectTransform stageSelectTextBack;
    private RectTransform stage;
    private RectTransform stageSubmitButton;
    private RectTransform numbar;
    //private RectTransform tutorial;
    //private RectTransform hyphen;
    private RectTransform rightArrow;
    private RectTransform leftArrow;
    private RectTransform stageSelectBack;
    private RectTransform stageSelectHint;

    //操作方法シーン用
    private RectTransform controllTextBack;
    private RectTransform sousa;
    private RectTransform controllBack;
    private RectTransform controllHint;

    //ステージセレクトの番号の画像格納用
    private Sprite[] sprites;
    //選択中のステージ番号
    private int stageNumber;
    //実際に描画されるステージ番号の配列
    private int[] drawingStageNumbars;

    private List<RectTransform> rectTransforms = new List<RectTransform>();

    private RectTransform previousSelectedObject;
    private RectTransform currentSelectedObject;

    //メニューの項目を変更中かどうか
    private bool changingSelection = true;

    //BGM変更用
    private BGMControl bgmctrl;

    void Start()
    {
        //Menuシーン（自身のシーン）を検索
        //menu = SceneManager.GetSceneByName("Menu");

        //UIInputManagerを取得
        input = GetComponent<UIInputManager>();

        //メニュータイプを設定
        menuType = PauseMenuType.Menu;

        //Backgroundの検索　代入
        background = GameObject.Find("Background").GetComponent<RectTransform>();

        //rectTransformsにBackgroundを追加
        rectTransforms.Add(background);

        //メニュー画面を初期化
        MenuInitialize();

        //Resoucesからステージセレクトの番号の画像格納
        sprites = Resources.LoadAll<Sprite>("UIImage/number");

        //実際に描画されるステージ番号の配列を登録
        drawingStageNumbars = new int[] { 0, 1, 2, 3, 4, 11 };

        bgmctrl = GameObject.Find("BGMControl").GetComponent<BGMControl>();
    }

    void Update()
    {
    }

    void LateUpdate()
    {
        switch (menuType)
        {
            case PauseMenuType.Menu:
                MenuInput();
                break;

            case PauseMenuType.StageSelect:
                if (GameObject.FindGameObjectWithTag("Fade").transform.childCount == 0) StageSelectInput();
                break;

            case PauseMenuType.Controll:
                ControllInput();
                break;
        }
    }

    private void MenuInitialize()
    {
        //UIの検索　代入
        menuTextBack = GameObject.Find("MenuTextBack").GetComponent<RectTransform>();
        returnToGame = GameObject.Find("ReturnToGame").GetComponent<RectTransform>();
        selectStage = GameObject.Find("SelectStage").GetComponent<RectTransform>();
        controll = GameObject.Find("Controll").GetComponent<RectTransform>();
        returnToTitle = GameObject.Find("ReturnToTitle").GetComponent<RectTransform>();
        exitGame = GameObject.Find("ExitGame").GetComponent<RectTransform>();
        menuBack = GameObject.Find("MenuBack").GetComponent<RectTransform>();
        menuHint = GameObject.Find("MenuHint").GetComponent<RectTransform>();

        //ListにFrameの中にあるUIのRectTransform全てを追加
        rectTransforms.AddRange(GameObject.Find("MenuFrame").GetComponentsInChildren<RectTransform>());
        //ListにFrameも入ってしまっているので削除（中身を子のみにする）
        rectTransforms.Remove(GameObject.Find("MenuFrame").GetComponent<RectTransform>());

        //Listの中身のAlphaを2.0秒かけて1.0にTween
        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 1.0f, 1.0f);
        }

        //0.5秒後に変更しているかどうかをfalseに
        StartCoroutine(DelayMethod(0.5f, () =>
        {
            changingSelection = false;

            //rectTransformsからBackgroundを削除
            rectTransforms.Remove(background);
        }));

        //初期に選択されている項目を設定
        EventSystem.current.SetSelectedGameObject(returnToGame.gameObject);

        //現在選択されている項目を更新
        currentSelectedObject =
            EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

        //TextBackを現在選択されているメニューの位置へ移動
        LeanTween.move(menuTextBack, currentSelectedObject.anchoredPosition, 0.0f);

        input.SetCancelAction(() =>
        {
            SoundManager.Instance.PlaySe("back");

            changingSelection = true;

            LeanTween.scale(menuBack, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            //rectTransformsにBackgroundを追加
            rectTransforms.Add(background);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                GameManager.Instance.Pause(false);
                Destroy(gameObject);
            }));
        });
    }

    private void MenuInput()
    {
        //メニューの項目を変更中ならば処理しない
        if (changingSelection == true) return;

        if (Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return))
        {
            input.Submit();
        }

        if (Input.GetButtonDown("PS4_Cross") || Input.GetKeyDown(KeyCode.Backspace))
        {
            input.Cancel();
        }

        input.UpAction(() =>
        {
            //現在選択されている項目がback以外の時のみ処理
            if (EventSystem.current.currentSelectedGameObject == menuBack.gameObject) return;
            SoundManager.Instance.PlaySe("select");

            //メニューの項目を更新
            EventSystem.current.SetSelectedGameObject(
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnUp.gameObject);

            //メニューの項目を変更中に変更
            changingSelection = true;

            //現在選択されている項目を更新
            currentSelectedObject =
                EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

            //TextBackを現在選択されているメニューの位置へ移動
            LeanTween.move(menuTextBack, currentSelectedObject.anchoredPosition, 0.5f)
                .setEase(LeanTweenType.easeInOutCubic) //補間曲線の設定
                .setOnComplete(() => //Tween終了後実行
                {
                    changingSelection = false;
                });
        });

        input.DownAction(() =>
        {
            //現在選択されている項目がback以外の時のみ処理
            if (EventSystem.current.currentSelectedGameObject == menuBack.gameObject) return;
            SoundManager.Instance.PlaySe("select");

            //メニューの項目を更新
            EventSystem.current.SetSelectedGameObject(
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnDown.gameObject);

            //メニューの項目を変更中に変更
            changingSelection = true;

            //現在選択されている項目を更新
            currentSelectedObject =
                EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

            //TextBackを現在選択されているメニューの位置へ移動
            LeanTween.move(menuTextBack, currentSelectedObject.anchoredPosition, 0.5f)
                .setEase(LeanTweenType.easeInOutCubic) //補間曲線の設定
                .setOnComplete(() => //Tween終了後実行
                {
                    changingSelection = false;
                });
        });

        input.RightAction(() =>
        {
            //現在選択されている項目がbackの時のみ処理
            if (EventSystem.current.currentSelectedGameObject != menuBack.gameObject) return;

            SoundManager.Instance.PlaySe("select");
            //メニューの項目を更新(過去に選択されていた項目に)
            EventSystem.current.SetSelectedGameObject(
                previousSelectedObject.gameObject);

            //メニューの項目を変更中に変更
            changingSelection = true;

            //現在選択されている項目を更新
            currentSelectedObject =
                EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

            //textBackの大きさをback用から戻す
            LeanTween.scale(menuTextBack, new Vector3(3.5f, 3.5f, 1.0f), 0.5f);

            //TextBackを現在選択されているメニューの位置へ移動
            LeanTween.move(menuTextBack, currentSelectedObject.anchoredPosition, 0.5f)
                .setEase(LeanTweenType.easeInOutCubic) //補間曲線の設定
                .setOnComplete(() => //Tween終了後実行
                {
                    changingSelection = false;
                });
        });

        input.LeftAction(() =>
        {
            //現在選択されている項目がback以外の時のみ処理
            if (EventSystem.current.currentSelectedGameObject == menuBack.gameObject) return;
            SoundManager.Instance.PlaySe("select");

            //選択されている項目を過去のオブジェクトに格納
            previousSelectedObject = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

            //メニューの項目を更新
            EventSystem.current.SetSelectedGameObject(
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnLeft.gameObject);

            //メニューの項目を変更中に変更
            changingSelection = true;

            //現在選択されている項目を更新
            currentSelectedObject =
                EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

            //textBackの大きさをback用に
            LeanTween.scale(menuTextBack, new Vector3(2.0f, 2.0f, 1.0f), 0.5f);

            //TextBackを現在選択されているメニューの位置へ移動
            LeanTween.move(menuTextBack, currentSelectedObject.anchoredPosition, 0.5f)
                .setEase(LeanTweenType.easeInOutCubic) //補間曲線の設定
                .setOnComplete(() => //Tween終了後実行
                {
                    changingSelection = false;
                });
        });
    }

    private void StageSelectInitialize()
    {
        //UIの検索　代入
        stageImage = GameObject.Find("StageImage").GetComponent<RectTransform>();
        stageSelectTextBack = GameObject.Find("StageSelectTextBack").GetComponent<RectTransform>();
        stage = GameObject.Find("StageText").GetComponent<RectTransform>();
        stageSubmitButton = GameObject.Find("StageSubmitButton").GetComponent<RectTransform>();
        numbar = GameObject.Find("StageNumbar").GetComponent<RectTransform>();
        //tutorial = GameObject.Find("Tutorial").GetComponent<RectTransform>();
        //hyphen = GameObject.Find("Hyphen").GetComponent<RectTransform>();
        rightArrow = GameObject.Find("RightArrow").GetComponent<RectTransform>();
        leftArrow = GameObject.Find("LeftArrow").GetComponent<RectTransform>();
        stageSelectBack = GameObject.Find("StageSelectBack").GetComponent<RectTransform>();
        stageSelectHint = GameObject.Find("StageSelectHint").GetComponent<RectTransform>();

        //ListにFrameの中にあるUIのRectTransform全てを追加
        rectTransforms.AddRange(GameObject.Find("StageSelectFrame").GetComponentsInChildren<RectTransform>());
        //ListにFrameも入ってしまっているので削除（中身を子のみにする）
        rectTransforms.Remove(GameObject.Find("StageSelectFrame").GetComponent<RectTransform>());

        //いったん削除
        //rectTransforms.Remove(tutorial);
        //rectTransforms.Remove(hyphen);

        //Listの中身のAlphaを1.0秒かけて1.0にTween
        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 1.0f, 1.0f);
        }

        //1.1秒後に変更しているかどうかをfalseに
        StartCoroutine(DelayMethod(1.1f, () =>
        {
            changingSelection = false;

            //矢印の透明度設定
            ArrowAlpha();

            //追加
            //rectTransforms.Add(tutorial);
            //rectTransforms.Add(hyphen);
        }));

        //ステージの番号を0に設定
        stageNumber = 0;

        //ステージ番号を初期設定
        StageNumbarInitialize(stageNumber);

        ////16/12/12 add　西--------------------------------------------
        GameManager.Instance.SetNextSceneName(mStageNameList[stageNumber]);
        ////------------------------------------------------------------

        //初期に選択されている項目を設定
        EventSystem.current.SetSelectedGameObject(stageSubmitButton.gameObject);

        //現在選択されている項目を更新
        currentSelectedObject =
            EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

        //TextBackを現在選択されているメニューの位置へ移動
        LeanTween.move(stageSelectTextBack, currentSelectedObject.anchoredPosition, 0.0f);

        //textBackの大きさと回転を設定
        LeanTween.scale(stageSelectTextBack, new Vector3(4.5f, 4.5f, 1.0f), 0.0f);
        LeanTween.rotateX(stageSelectTextBack.gameObject, 77.0f, 0.0f);

        input.SetCancelAction(() =>
        {
            SoundManager.Instance.PlaySe("back");

            changingSelection = true;

            LeanTween.scale(stageSelectBack, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                LeanTween.scale(stageSelectBack, new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
                rectTransforms.Clear();
                MenuInitialize();
                menuType = PauseMenuType.Menu;
            }));
        });
    }

    private void StageSelectInput()
    {
        if (changingSelection == true) return;

        if (Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return))
        {
            input.Submit();
        }

        if (Input.GetButtonDown("PS4_Cross") || Input.GetKeyDown(KeyCode.Backspace))
        {
            input.Cancel();
        }

        input.UpAction(() =>
        {
            //現在選択されている項目がbackの時のみ処理
            if (EventSystem.current.currentSelectedGameObject != stageSelectBack.gameObject) return;
            SoundManager.Instance.PlaySe("select");

            //メニューの項目を更新
            EventSystem.current.SetSelectedGameObject(
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnUp.gameObject);

            //メニューの項目を変更中に変更
            changingSelection = true;

            //現在選択されている項目を更新
            currentSelectedObject =
                EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

            //textBackの大きさと回転をback用から戻す
            LeanTween.scale(stageSelectTextBack, new Vector3(4.5f, 4.5f, 1.0f), 0.5f);
            LeanTween.rotateX(stageSelectTextBack.gameObject, 77.0f, 0.5f);

            //TextBackを現在選択されているメニューの位置へ移動
            LeanTween.move(stageSelectTextBack, currentSelectedObject.anchoredPosition, 0.5f)
                .setEase(LeanTweenType.easeInOutCubic) //補間曲線の設定
                .setOnComplete(() => //Tween終了後実行
                {
                    changingSelection = false;
                });
        });

        input.DownAction(() =>
        {
            //現在選択されている項目がback以外の時のみ処理
            if (EventSystem.current.currentSelectedGameObject == stageSelectBack.gameObject) return;
            SoundManager.Instance.PlaySe("select");

            //メニューの項目を更新
            EventSystem.current.SetSelectedGameObject(
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnDown.gameObject);

            //メニューの項目を変更中に変更
            changingSelection = true;

            //現在選択されている項目を更新
            currentSelectedObject =
                EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

            //textBackの大きさと回転をback用に
            LeanTween.scale(stageSelectTextBack, new Vector3(2.0f, 2.0f, 1.0f), 0.5f);
            LeanTween.rotateX(stageSelectTextBack.gameObject, 70.0f, 0.5f);

            //TextBackを現在選択されているメニューの位置へ移動
            LeanTween.move(stageSelectTextBack, currentSelectedObject.anchoredPosition, 0.5f)
                .setEase(LeanTweenType.easeInOutCubic) //補間曲線の設定
                .setOnComplete(() => //Tween終了後実行
                {
                    changingSelection = false;
                });
        });

        input.RightAction(() =>
        {
            //現在選択されている項目がback以外の時のみ処理
            if (EventSystem.current.currentSelectedGameObject == stageSelectBack.gameObject) return;
            SoundManager.Instance.PlaySe("select");

            changingSelection = true;

            //前のStageNumbarを保存
            int prevNumbar = stageNumber;

            stageNumber++;
            //ステージ番号を更新
            StageNumbarUpdate(stageNumber);

            //ステージ画像を更新
            StageImageUpdate(prevNumbar, stageNumber);

            ////16/12/12 add　西--------------------------------------------
            GameManager.Instance.SetNextSceneName(mStageNameList[stageNumber]);
            ////------------------------------------------------------------

            LeanTween.scale(rightArrow, new Vector3(1.5f, 1.5f, 0.0f), 0.15f)
                .setOnComplete(() =>
                {
                    LeanTween.scale(rightArrow, new Vector3(1.0f, 1.0f, 0.0f), 0.15f);
                    ArrowAlpha();
                });
        });

        input.LeftAction(() =>
        {
            //現在選択されている項目がback以外の時のみ処理
            if (EventSystem.current.currentSelectedGameObject == stageSelectBack.gameObject) return;
            SoundManager.Instance.PlaySe("select");

            changingSelection = true;

            //前のStageNumbarを保存
            int prevNumbar = stageNumber;

            stageNumber--;
            //ステージ番号を更新
            StageNumbarUpdate(stageNumber);

            //ステージ画像を更新
            StageImageUpdate(prevNumbar, stageNumber);

            ////16/12/12 add　西--------------------------------------------
            GameManager.Instance.SetNextSceneName(mStageNameList[stageNumber]);
            ////------------------------------------------------------------

            LeanTween.scale(leftArrow, new Vector3(1.5f, 1.5f, 0.0f), 0.15f)
                .setOnComplete(() =>
                {
                    LeanTween.scale(leftArrow, new Vector3(1.0f, 1.0f, 0.0f), 0.15f);
                    ArrowAlpha();
                });
        });
    }

    private void StageNumbarInitialize(int numbar)
    {
        //stageNumbarを範囲内に収める
        numbar = Mathf.Clamp(numbar, 0, 5);

        //stageNumbrを範囲内に収める
        stageNumber = numbar;

        //ファイルの名前を作成
        string fileName = string.Format("number_{0}", drawingStageNumbars[numbar]);

        //Imageコンポーネント取得
        Image img = this.numbar.gameObject.GetComponent<Image>();

        //SourceImageをStageNumberのものに変更
        img.sprite = System.Array.Find<Sprite>(sprites, (sprite) => sprite.name.Equals(fileName));

        LeanTween.move(this.numbar, new Vector2(114, 275), 0.0f);

        //ステージ番号がチュートリアルの場合　配置を変更
        //if (numbar == 0 || numbar == 4 || numbar == 8)
        //{
        //    LeanTween.alpha(tutorial, 1.0f, 1.0f);
        //    LeanTween.alpha(hyphen, 1.0f, 1.0f);
        //    LeanTween.move(this.numbar, new Vector2(162, 275), 0.0f);
        //}
        //else
        //{
        //    LeanTween.alpha(tutorial, 0.0f, 0.0f);
        //    LeanTween.alpha(hyphen, 0.0f, 0.0f);
        //    LeanTween.move(this.numbar, new Vector2(114, 275), 0.0f);
        //}
    }

    private void StageNumbarUpdate(int numbar)
    {
        //stageNumbarを範囲内に収める
        numbar = Mathf.Clamp(numbar, 0, 5);

        //stageNumbrを範囲内に収める
        stageNumber = numbar;

        //ファイルの名前を作成
        string fileName = string.Format("number_{0}", drawingStageNumbars[stageNumber]);

        //Imageコンポーネント取得
        Image img = this.numbar.gameObject.GetComponent<Image>();

        //SourceImageをStageNumberのものに変更
        img.sprite = System.Array.Find<Sprite>(sprites, (sprite) => sprite.name.Equals(fileName));

        LeanTween.move(this.numbar, new Vector2(114, 275), 0.0f);

        //ステージ番号がチュートリアルの場合　配置を変更
        //if (numbar == 0 || numbar == 4 || numbar == 8)
        //{
        //    LeanTween.alpha(tutorial, 1.0f, 0.1f);
        //    LeanTween.alpha(hyphen, 1.0f, 0.1f);
        //    LeanTween.move(this.numbar, new Vector2(162, 275), 0.0f);
        //}
        //else
        //{
        //    LeanTween.alpha(tutorial, 0.0f, 0.1f);
        //    LeanTween.alpha(hyphen, 0.0f, 0.1f);
        //    LeanTween.move(this.numbar, new Vector2(114, 275), 0.0f);
        //}
    }

    private void ArrowAlpha()
    {
        if (stageNumber == 0)
            LeanTween.alpha(leftArrow, 0.5f, 0.3f);
        else
            LeanTween.alpha(leftArrow, 1.0f, 0.3f);

        if (stageNumber == 5)
            LeanTween.alpha(rightArrow, 0.5f, 0.3f);
        else
            LeanTween.alpha(rightArrow, 1.0f, 0.3f);
    }

    private void StageImageUpdate(int prevNumbar, int currentNumbar)
    {
        if (prevNumbar != currentNumbar)
        {
            //ファイルの名前を作成
            string fileName = string.Format("UIImage/stage{0}", drawingStageNumbars[currentNumbar]);

            //Imageコンポーネント取得
            Image img = stageImage.gameObject.GetComponent<Image>();

            LeanTween.alpha(stageImage, 0.0f, 0.5f)
                .setOnComplete(() =>
                {
                    //SourceImageをStageNumberのものに変更
                    Texture2D texture = Resources.Load(fileName) as Texture2D;
                    img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                    LeanTween.alpha(stageImage, 1.0f, 0.5f)
                    .setOnComplete(() =>
                    {
                        changingSelection = false;
                    });
                });
        }
        else
        {
            StartCoroutine(DelayMethod(0.3f, () =>
            {
                changingSelection = false;
            }));
        }
    }

    private void ControllInitialize()
    {
        //UIの検索　代入
        controllTextBack = GameObject.Find("ControllTextBack").GetComponent<RectTransform>();
        sousa = GameObject.Find("Sousa").GetComponent<RectTransform>();
        controllBack = GameObject.Find("ControllBack").GetComponent<RectTransform>();
        controllHint = GameObject.Find("ControllHint").GetComponent<RectTransform>();

        //ListにFrameの中にあるUIのRectTransform全てを追加
        rectTransforms.AddRange(GameObject.Find("ControllFrame").GetComponentsInChildren<RectTransform>());
        //ListにFrameも入ってしまっているので削除（中身を子のみにする）
        rectTransforms.Remove(GameObject.Find("ControllFrame").GetComponent<RectTransform>());

        //Listの中身のAlphaを2.0秒かけて1.0にTween
        foreach (RectTransform rectTr in rectTransforms)
        {
            LeanTween.alpha(rectTr, 1.0f, 1.0f);
        }

        //0.5秒後に変更しているかどうかをfalseに
        StartCoroutine(DelayMethod(1.1f, () =>
        {
            changingSelection = false;
        }));

        //初期に選択されている項目を設定
        EventSystem.current.SetSelectedGameObject(controllBack.gameObject);

        //現在選択されている項目を更新
        currentSelectedObject =
            EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

        //TextBackを現在選択されているメニューの位置へ移動
        LeanTween.move(controllTextBack, currentSelectedObject.anchoredPosition, 0.0f);

        input.SetCancelAction(() =>
        {
            SoundManager.Instance.PlaySe("back");

            changingSelection = true;

            LeanTween.scale(controllBack, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                LeanTween.scale(controllBack, new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
                rectTransforms.Clear();
                MenuInitialize();
                menuType = PauseMenuType.Menu;
            }));
        });
    }

    private void ControllInput()
    {
        //メニューの項目を変更中ならば処理しない
        if (changingSelection == true) return;

        StartCoroutine(DelayMethod(1, () =>
        {
            if (Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return))
            {
                input.Submit();
            }

            if (Input.GetButtonDown("PS4_Cross") || Input.GetKeyDown(KeyCode.Backspace))
            {
                input.Cancel();
            }
        }));
    }

    /// <summary>
    /// ReturnToGameButtonを押した時の処理
    /// </summary>
    public void ReturnToGameButtonPressed()
    {
        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("back");

            changingSelection = true;

            LeanTween.scale(returnToGame, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            //rectTransformsにBackgroundを追加
            rectTransforms.Add(background);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                GameManager.Instance.Pause(false);
                Destroy(gameObject);
            }));
        });
    }

    /// <summary>
    /// SelectStageButtonを押した時の処理
    /// </summary>
    public void SelectStageButtonPressed()
    {
        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("enter");

            changingSelection = true;

            LeanTween.scale(selectStage, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                LeanTween.scale(selectStage, new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
                rectTransforms.Clear();
                StageSelectInitialize();
                menuType = PauseMenuType.StageSelect;
            }));
        });
    }

    /// <summary>
    /// ControllButtonを押した時の処理
    /// </summary>
    public void ControllButtonPressed()
    {
        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("enter");

            changingSelection = true;

            LeanTween.scale(controll, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                LeanTween.scale(controll, new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
                rectTransforms.Clear();
                ControllInitialize();
                menuType = PauseMenuType.Controll;
            }));
        });
    }

    /// <summary>
    /// ReturnToTitleButtonを押した時の処理
    /// </summary>
    public void ReturnToTitleButtonPressed()
    {
        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("back");

            changingSelection = true;

            LeanTween.scale(returnToTitle, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            //rectTransformsにBackgroundを追加
            rectTransforms.Add(background);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            //1.1秒後にMenuシーンをアンロード
            StartCoroutine(DelayMethod(1.1f, () =>
            {
                GameManager.Instance.Pause(false);
                GameManager.Instance.GameModeChange(GameManager.GameMode.Title);
                SoundManager.Instance.PlayBgm("title");
                Destroy(gameObject);
            }));
        });
    }

    /// <summary>
    /// ExitGameButtonを押した時の処理
    /// </summary>
    public void ExitGameButtonPressed()
    {
        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("back");

            changingSelection = true;

            LeanTween.scale(exitGame, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            //rectTransformsにBackgroundを追加
            rectTransforms.Add(background);

            //1.1秒後にMenuシーンをアンロード
            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                Application.Quit();
            }));
        });
    }

    /// <summary>
    /// Menu画面のBackButtonを押した時の処理
    /// </summary>
    public void MenuBackButtonPressed()
    {
        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("back");

            changingSelection = true;

            LeanTween.scale(menuBack, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            //rectTransformsにBackgroundを追加
            rectTransforms.Add(background);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                GameManager.Instance.Pause(false);
                Destroy(gameObject);
            }));
        });
    }


    /// <summary>
    /// StageSubmitButtonを押した時の処理
    /// </summary>
    public void StageSubmitButtonPressed()
    {
        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("enter");

            changingSelection = true;

            //LeanTween.scale(stage, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);
            //LeanTween.scale(numbar, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            //rectTransformsにBackgroundを追加
            rectTransforms.Add(background);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            //1.1秒後にMenuシーンをアンロード
            StartCoroutine(DelayMethod(1.1f, () =>
            {
                GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeFactory>().FadeInstance(Color.white, 1.0f);
                GameManager.Instance.GameModeChange(GameManager.GameMode.GamePlay);
                GameManager.Instance.Pause(false);
                SoundManager.Instance.StopBgm();
                if (stageNumber == 5)
                {
                    //最終ステージ
                    bgmctrl.StageFinalSelected();
                }
                else if (stageNumber == 0)
                {
                    //チュートリアルステージ
                    bgmctrl.StartGameSelected();
                }
                else
                {
                    //ステージ１～４
                    bgmctrl.Stage1_4Selected();
                }

                Destroy(gameObject);
            }));
        });
    }

    /// <summary>
    /// StageSelect画面のBackButtonを押した時の処理
    /// </summary>
    public void SelectBackButtonPressed()
    {
        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("back");

            changingSelection = true;

            LeanTween.scale(stageSelectBack, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                LeanTween.scale(stageSelectBack, new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
                rectTransforms.Clear();
                MenuInitialize();
                menuType = PauseMenuType.Menu;
            }));
        });
    }


    /// <summary>
    /// 操作説明画面のBackButtonを押した時の処理
    /// </summary>
    public void ControllBackButtonPressed()
    {
        input.SetSubmitAction(() =>
        {
            SoundManager.Instance.PlaySe("back");

            changingSelection = true;

            LeanTween.scale(controllBack, new Vector3(1.5f, 1.5f, 1.0f), 1.0f);

            foreach (RectTransform rectTr in rectTransforms)
            {
                LeanTween.alpha(rectTr, 0.0f, 1.0f);
            }

            StartCoroutine(DelayMethod(1.1f, () =>
            {
                LeanTween.scale(controllBack, new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
                rectTransforms.Clear();
                MenuInitialize();
                menuType = PauseMenuType.Menu;
            }));
        });
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

