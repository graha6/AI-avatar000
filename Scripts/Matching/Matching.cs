using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Matching : MonoBehaviour
{
    public GameObject comObj;           //ComServerオブジェクト
    ComServer comScript;
    public int MasterFlg;               //1:Master 0:Client

    public TextMeshProUGUI MatchText;
    public TextMeshProUGUI PlayerText;

    public static readonly string CMD_MATCH = "11";
    public static readonly string CMD_BIRTH = "12";

    public GameObject StartButton;

    private void Awake()
    {
        DontDestroyOnLoad(comObj);                      //[Stage1]まで引き継ぐ

    }
    // Start is called before the first frame update
    void Start()
    {

        comScript = comObj.GetComponent<ComServer>();
        comScript.ComStart(MasterFlg);

       
        StartButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!comScript.isComEnable())
        {
            return;
        }


        if (MasterFlg == 1)         //Masterに処理する
        {
            prcMasterClient();
        }
        else                        //Clientに処理する①
        {
            prcClient();
        }

        //メッセージ送信、同期
        if (comScript.chkRcvData())
        {
            string rcvData = comScript.getRcvData();

            string cmd = rcvData.Substring(2, 2);

            if (cmd == CMD_MATCH || cmd == CMD_BIRTH)
            {
                SceneManager.LoadScene("Story");
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.MoveGameObjectToScene(comObj, SceneManager.GetActiveScene());
            comScript.DisConnect();
            SceneManager.LoadScene("Title");
        }
    }
    private void prcMasterClient()
    {
        PlayerText.text = "Player1";

        if (comScript.getPlayerNum() == 1)      //1人になったら以下処理
        {
            StartButton.SetActive(false);
            MatchText.text = "Matching 1/2";
            
        }
        else if (comScript.getPlayerNum() == 2)
        {
            StartButton.SetActive(true);
            MatchText.text = "Matching true!";
        }
        else
        {       
            StartButton.SetActive(false);
            MatchText.text = "Over!!";
        }
    }
    private void prcClient()
    {
        PlayerText.text = "Player2";

        if (comScript.getPlayerNum() == 2)
        {                                       
            MatchText.text = "Matching true!";
        }
        else
        {
            MatchText.text = "now matching...";
        }
    }

    public void OnClick()
    {
        comScript.SendData(Matching.CMD_MATCH, "");     //イベント処理
    }
}
