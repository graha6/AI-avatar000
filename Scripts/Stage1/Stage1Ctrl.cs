using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stage1Ctrl : MonoBehaviourPunCallbacks
{
    ComServer comScript;               //ComServer�X�N���v�g 
    GameObject comObj;                 //�q�G�����L�[���ComServer�I�u�W�F�N�g

    public int MasterFlg;              //1:Master 0:Client

    enum ScreenState
    {
        Spawn_Client,
        Playing
    }
    ScreenState _nowState;

    public GameObject putin;
    GameObject putinObj;                //putin����p

    // Start is called before the first frame update
    void Start()
    {
        putinObj = Instantiate(putin, transform.position, transform.rotation);
        putinObj.SetActive(false);
        comObj = GameObject.Find("ComServer");
        comScript = comObj.GetComponent<ComServer>();
        comScript.setMessageQueueRunning(true);          //[ComServer]���烁�b�Z�[�W����M

        comScript.ComStart();

        _nowState = ScreenState.Spawn_Client;
        comScript.ComStart(MasterFlg);

        DontDestroyOnLoad(comObj);                      //[Stage2]�܂ň����p��
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
                if (MasterFlg == 1)             //(PhotonNetwork.IsMasterClient)�̑�ւ�
                {
                    putinObj.SetActive(true);   //�I�u�W�F�N�g�̃J�����p�ӂ�҂��߂̎��ԉ҂�
                    Invoke("AIavatar", 2f);

                }
                if (MasterFlg == 0)        //(!PhotonNetwork.IsMasterClient)�̑�ւ�
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