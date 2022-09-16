using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stage2Ctrl : MonoBehaviourPunCallbacks
{
    private GameObject comObj = null;  //ComServerオブジェクト
    private ComServer comScript = null;

    public int MasterFlg;              //1:Master 0:Client


    enum ScreenState
    {
        Spawn_Client,
        Playing
    }
    ScreenState _nowState;

    public GameObject putin;
    GameObject putinObj;                //putin制御用

    public GameObject windowObj;        //実績ウィンドウオブジェクト(UI)
    public int window;
    public TextMeshProUGUI WindowText;

    float wTime, wTime1, wTime2;        //各実績ウィンドウ専用時間計算
    bool isMove;                        //windowObj左右移動の正誤     

    GameObject AIavatarObj = null;
    GameObject DogavatarObj = null;
    Animator AIanim = null;
    Animator Doganim = null;
    public RuntimeAnimatorController[] changeAnim;      //変更するController

    public GameObject panelObj;                         //stage移動時の演出オブジェクト(UI)
    Image panelimage;
    float amount = 0;
    int panel;                                          //1:+amount
                                                        //2:-amount
    GameObject enemyObj;
    float wTimeX = 0;                                   //shortstory専用時間計算
    int shortstory;

    //public GameObject lightObj;                         //[Directional Light]

    public static readonly string CMD_STAGE = "07";
    public static readonly string CMD_RECEC = "08";

    // Start is called before the first frame update
    void Start()
    {

        comObj = GameObject.Find("ComServer");
        comScript = comObj.GetComponent<ComServer>();
        comScript.setMessageQueueRunning(true);              //[ComServer]からメッセージを受信

        comScript.ComStart();

        _nowState = ScreenState.Spawn_Client;
        comScript.ComStart(MasterFlg);

        SceneManager.MoveGameObjectToScene(comObj, SceneManager.GetActiveScene());      //解除

        putinObj = Instantiate(putin, transform.position, transform.rotation);
        putinObj.SetActive(false);

        wTime = 0;
        wTime1 = 0;
        wTime2 = 0;
        isMove = false;

        GameObject ClayObj = GameObject.Find("Clay");         //ゲットするItemたち
        ClayObj.GetComponent<Animator>().enabled = false;     //各プレイヤーCtrlからONに
        GameObject PaintsObj = GameObject.Find("Paints");
        PaintsObj.GetComponent<Animator>().enabled = false;

        panelimage = panelObj.GetComponent<Image>();
        panelimage.fillAmount = amount;


        enemyObj = GameObject.Find("story Canvas").transform.Find("enemy").gameObject;   //shortstory登場キャラ
        enemyObj.SetActive(false);                                                       //各プレイヤーCtrlからONに

        //lightObj.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        //メッセージ送信、同期
        if (comScript.chkRcvData())
        {
            string rcvData = comScript.getRcvData();

            string cmd = rcvData.Substring(2, 2);
            if (cmd == CMD_STAGE || cmd == CMD_RECEC)
            {
                SceneManager.LoadScene("Credit title");
            }
        }

        /*if (!(comObj.GetComponent<ComServer>().getPlayerNum() == 3))
        {
            SceneManager.LoadScene("Error");
        }*/
        switch (_nowState)
        {

            case ScreenState.Spawn_Client:
                if (MasterFlg == 1)             //(PhotonNetwork.IsMasterClient)の代替え
                {
                    putinObj.SetActive(true);   //オブジェクトのカメラ用意を待つための時間稼ぎ
                    Invoke("AIavatar", 2f);

                }
                if (MasterFlg == 0)             //(!PhotonNetwork.IsMasterClient)の代替え
                {
                    putinObj.SetActive(true);
                    Invoke("Dogavatar", 2f);
                }

                _nowState = ScreenState.Playing;
                break;
        }

        //各種実績ウィンドウ
        switch (window)
        {
            case 1:
                Clay();
                WindowText.text = "ねんど を 獲得！\n   素材名: Clay";
                isMove = false;

                wTime += Time.deltaTime;
                if (wTime > 9f) break;              //条件を満たすと抜ける
                {
                    isMove = true;
                }

                break;
            case 2:
                Paints();
                WindowText.text = "えのぐ を 獲得！\n   素材名: Paints";
                isMove = false;

                wTime1 += Time.deltaTime;
                if (wTime1 > 9f) break;
                {
                    isMove = true;
                }
                break;
            case 3:
                WindowText.text = "むし を 検知！\n　怒ってる？";
                isMove = false;

                wTime2 += Time.deltaTime;
                if (wTime2 > 9f) break;
                {
                    isMove = true;
                }
                break;
        }

        Vector2 pos;
        if (isMove)
        {
            pos = windowObj.transform.position;
            pos.x = pos.x - 4f;
            if (pos.x < 1760)
            {
                pos.x = 1760;
            }
            windowObj.transform.position = pos;
        }
        else
        {
            pos = windowObj.transform.position;
            pos.x = pos.x + 1.5f;

            if (pos.x > 2200)
            {
                pos.x = 2200;
            }
            windowObj.transform.position = pos;
        }

        //stage移動時の演出
        switch (panel)
        {
            case 1:
                amount += Time.deltaTime;
                panelimage.fillAmount = amount;
                break;
            case 2:
                amount -= Time.deltaTime;
                panelimage.fillAmount = amount;

                break;
        }
        //stage3でのショートストーリー
        switch (shortstory)
        {
            case 1:
                wTimeX += Time.deltaTime;
                if (wTimeX > 2f)
                {
                    panel = 1;
                }
                if (wTimeX > 10f)
                {
                    panel = 2;
                }
                if (wTimeX > 12f)
                {
                    enemyObj.SetActive(false);
                    AIavatarObj.GetComponent<PlayerCtrl>().StageGo();
                    DogavatarObj.GetComponent<PlayerCtrl_Dog>().StageGo();
                }
                break;
        }
    }

    void AIavatar()
    {
        AIavatarObj = PhotonNetwork.Instantiate("AI_Stage2", new Vector2(0, -1.8f), transform.rotation);
        AIavatarObj.GetComponent<PlayerCtrl>().SetMyAvatar();
        GameObject BG = AIavatarObj.transform.Find("Canvas").gameObject;       //stage3で使うBG
        BG.SetActive(false);
        putinObj.SetActive(false);

        PlayerCtrl AIScript = AIavatarObj.GetComponent<PlayerCtrl>();
        AIScript.respawnPosx = 0;                                               //プレイヤーのリスポーン位置を変更
        AIScript.respawnPosy = -1.8f;
    }
    void Dogavatar()
    {
        DogavatarObj = PhotonNetwork.Instantiate("Dog_Stage2", new Vector2(3, -1.8f), transform.rotation);
        DogavatarObj.GetComponent<PlayerCtrl_Dog>().SetMyAvatar();

        GameObject BG = DogavatarObj.transform.Find("Canvas").gameObject;                   //stage3で使うBG
        BG.SetActive(false);
        putinObj.SetActive(false);

        PlayerCtrl_Dog AIScript = DogavatarObj.GetComponent<PlayerCtrl_Dog>();
        AIScript.respawnPosx = 3;                                               //プレイヤーのリスポーン位置を変更
        AIScript.respawnPosy = -1.8f;
    }
    void Clay()
    {
        AIanim = GameObject.Find("AI_Stage2(Clone)").GetComponent<Animator>();
        AIanim.runtimeAnimatorController = changeAnim[0];             //AnimatorのControllerを変更[Sculpt]

        Doganim = GameObject.Find("Dog_Stage2(Clone)").GetComponent<Animator>();
        Doganim.runtimeAnimatorController = changeAnim[1];            //AnimatorのControllerを変更[Sculpt]

    }
    void Paints()
    {
        AIanim = GameObject.Find("AI_Stage2(Clone)").GetComponent<Animator>();
        AIanim.runtimeAnimatorController = changeAnim[2];    //AnimatorのControllerを変更[Texture]

        Doganim = GameObject.Find("Dog_Stage2(Clone)").GetComponent<Animator>();
        Doganim.runtimeAnimatorController = changeAnim[3];   //AnimatorのControllerを変更[Texture]
    }

    public void Stage()                                      //次ステージ移動演出//[PlayerCtrl]から呼ばれる
    {
        panel = 1;
        Invoke("Newstage", 4);
    }
    void Newstage()
    {
        panel = 2;
        AIavatarObj = GameObject.Find("AI_Stage2(Clone)");
        AIavatarObj.transform.position = new Vector2(292, 20);                 //stage3へ移動

        DogavatarObj = GameObject.Find("Dog_Stage2(Clone)");
        DogavatarObj.transform.position = new Vector2(290, 22);

        GameObject BG = AIavatarObj.transform.Find("Canvas").gameObject;       //BG表示
        BG.SetActive(true);
        GameObject BG1 = DogavatarObj.transform.Find("Canvas").gameObject;
        BG1.SetActive(true);
    }
    public void ShortStory()                                  //[PlayerCtrl]から呼ばれる
    {
        enemyObj.SetActive(true);
        shortstory = 1;
    }
    public void FinishLoad()                                  //各プレイヤーCtrlから呼ばれる
    {
        comScript.SendData(Stage2Ctrl.CMD_STAGE, "");         //イベント処理
    }
}