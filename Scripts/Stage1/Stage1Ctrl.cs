using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stage1Ctrl : MonoBehaviourPunCallbacks
{
    ComServer comScript;               //ComServerスクリプト 
    GameObject comObj;                 //ヒエラルキー上のComServerオブジェクト

    public int MasterFlg;              //1:Master 0:Client

    enum ScreenState
    {
        Spawn_Client,
        Playing
    }
    ScreenState _nowState;

    public GameObject putin;
    GameObject putinObj;                //putin制御用

    // Start is called before the first frame update
    void Start()
    {
        putinObj = Instantiate(putin, transform.position, transform.rotation);
        putinObj.SetActive(false);
        comObj = GameObject.Find("ComServer");
        comScript = comObj.GetComponent<ComServer>();
        comScript.setMessageQueueRunning(true);          //[ComServer]からメッセージを受信

        comScript.ComStart();

        _nowState = ScreenState.Spawn_Client;
        comScript.ComStart(MasterFlg);

        DontDestroyOnLoad(comObj);                      //[Stage2]まで引き継ぐ
    }

    // Update is called once per frame
    void Update()
    {
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
                if (MasterFlg == 0)        //(!PhotonNetwork.IsMasterClient)の代替え
                {
                    putinObj.SetActive(true);
                    Invoke("Dogavatar", 2f);
                }

                _nowState = ScreenState.Playing;
                break;
        }
 
    }

    void AIavatar()
    {
        GameObject AIavatar = PhotonNetwork.Instantiate("AI", new Vector2(-20, -10), transform.rotation);
        AIavatar.GetComponent<PlayerCtrl>().SetMyAvatar();
        putinObj.SetActive(false);
    }
    void Dogavatar()
    {
        GameObject Dogavatar = PhotonNetwork.Instantiate("Dog", new Vector2(-21, -11), transform.rotation);
        Dogavatar.GetComponent<PlayerCtrl_Dog>().SetMyAvatar();
        putinObj.SetActive(false);
    }

}