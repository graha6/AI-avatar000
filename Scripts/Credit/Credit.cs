using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Credit : MonoBehaviour
{
    Vector2 pos;

    int text;                   //text内容変更
    public float speed;
    public float goal;
    public TextMeshProUGUI CreditText;
    public GameObject ExitButton;

    bool isMove;

    // Start is called before the first frame update
    void Start()
    {
        isMove = true;
        text = 1;
        transform.position = new Vector2(20, -3);
        ExitButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        pos = this.transform.position;
        if (isMove) pos.x = pos.x - speed;
        if (pos.x < goal)
        {
            text++;                     //int変数を加算
            pos.x = 20;
        }
        this.transform.position = pos;

        switch (text)
        {
            case 1:
                CreditText.text = "[AI アバター 000]\n©2022 avatars";
                break;
            case 2:
                CreditText.text = "制作統括 えびさわゆい・かみむらあゆみ";
                break;
            case 3:
                CreditText.text = "企画原案 えびさわゆい";
                break;
            case 4:
                CreditText.text = "プログラム かみむらあゆみ";
                break;
            case 5:
                CreditText.text = "デザイン えびさわゆい";
                break;
            case 6:
                CreditText.text = "本ゲームは　[メタバース]　をモチーフに";
                break;
            case 7:
                CreditText.text = "我々が操る仮想空間上の　アバターの冒険　を描きました";
                break;
            case 8:
                CreditText.text = "最後までプレイしてくれてありがとうございます！";
                break;
            case 9:
                if (pos.x < 4)
                {
                    isMove = false;
                    pos.x = 4;
                }
                CreditText.text = "Thank you for playing!";
                ExitButton.SetActive(true);
                break;

        }
    }
    public void OnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // UnityEditorの実行を停止する処理
#else
        Application.Quit();                                // ゲームを終了する処理
#endif
    }
}