using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stage2Ctrl : MonoBehaviourPunCallbacks
{
    private GameObject comObj = null;  //ComServer�I�u�W�F�N�g
    private ComServer comScript = null;

    public int MasterFlg;              //1:Master 0:Client


    enum ScreenState
    {
        Spawn_Client,
        Playing
    }
    ScreenState _nowState;

    public GameObject putin;
    GameObject putinObj;                //putin����p

    public GameObject windowObj;        //���уE�B���h�E�I�u�W�F�N�g(UI)
    public int window;
    public TextMeshProUGUI WindowText;

    float wTime, wTime1, wTime2;        //�e���уE�B���h�E��p���Ԍv�Z
    bool isMove;                        //windowObj���E�ړ��̐���     

    GameObject AIavatarObj = null;
    GameObject DogavatarObj = null;
    Animator AIanim = null;
    Animator Doganim = null;
    public RuntimeAnimatorController[] changeAnim;      //�ύX����Controller

    public GameObject panelObj;                         //stage�ړ����̉��o�I�u�W�F�N�g(UI)
    Image panelimage;
    float amount = 0;
    int panel;                                          //1:+amount
                                                        //2:-amount
    GameObject enemyObj;
    float wTimeX = 0;                                   //shortstory��p���Ԍv�Z
    int shortstory;

    //public GameObject lightObj;                         //[Directional Light]

    public static readonly string CMD_STAGE = "07";
    public static readonly string CMD_RECEC = "08";

    // Start is called before the first frame update
    void Start()
    {

        comObj = GameObject.Find("ComServer");
        comScript = comObj.GetComponent<ComServer>();
        comScript.setMessageQueueRunning(true);              //[ComServer]���烁�b�Z�[�W����M

        comScript.ComStart();

        _nowState = ScreenState.Spawn_Client;
        comScript.ComStart(MasterFlg);

        SceneManager.MoveGameObjectToScene(comObj, SceneManager.GetActiveScene());      //����

        putinObj = Instantiate(putin, transform.position, transform.rotation);
        putinObj.SetActive(false);

        wTime = 0;
        wTime1 = 0;
        wTime2 = 0;
        isMove = false;

        GameObject ClayObj = GameObject.Find("Clay");         //�Q�b�g����Item����
        ClayObj.GetComponent<Animator>().enabled = false;     //�e�v���C���[Ctrl����ON��
        GameObject PaintsObj = GameObject.Find("Paints");
        PaintsObj.GetComponent<Animator>().enabled = false;

        panelimage = panelObj.GetComponent<Image>();
        panelimage.fillAmount = amount;


        enemyObj = GameObject.Find("story Canvas").transform.Find("enemy").gameObject;   //shortstory�o��L����
        enemyObj.SetActive(false);                                                       //�e�v���C���[Ctrl����ON��

        //lightObj.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        //���b�Z�[�W���M�A����
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
                if (MasterFlg == 1)             //(PhotonNetwork.IsMasterClient)�̑�ւ�
                {
                    putinObj.SetActive(true);   //�I�u�W�F�N�g�̃J�����p�ӂ�҂��߂̎��ԉ҂�
                    Invoke("AIavatar", 2f);

                }
                if (MasterFlg == 0)             //(!PhotonNetwork.IsMasterClient)�̑�ւ�
                {
                    putinObj.SetActive(true);
                    Invoke("Dogavatar", 2f);
                }

                _nowState = ScreenState.Playing;
                break;
        }

        //�e����уE�B���h�E
        switch (window)
        {
            case 1:
                Clay();
                WindowText.text = "�˂�� �� �l���I\n   �f�ޖ�: Clay";
                isMove = false;

                wTime += Time.deltaTime;
                if (wTime > 9f) break;              //�����𖞂����Ɣ�����
                {
                    isMove = true;
                }

                break;
            case 2:
                Paints();
                WindowText.text = "���̂� �� �l���I\n   �f�ޖ�: Paints";
                isMove = false;

                wTime1 += Time.deltaTime;
                if (wTime1 > 9f) break;
                {
                    isMove = true;
                }
                break;
            case 3:
                WindowText.text = "�ނ� �� ���m�I\n�@�{���Ă�H";
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

        //stage�ړ����̉��o
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
        //stage3�ł̃V���[�g�X�g�[���[
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
        GameObject BG = AIavatarObj.transform.Find("Canvas").gameObject;       //stage3�Ŏg��BG
        BG.SetActive(false);
        putinObj.SetActive(false);

        PlayerCtrl AIScript = AIavatarObj.GetComponent<PlayerCtrl>();
        AIScript.respawnPosx = 0;                                               //�v���C���[�̃��X�|�[���ʒu��ύX
        AIScript.respawnPosy = -1.8f;
    }
    void Dogavatar()
    {
        DogavatarObj = PhotonNetwork.Instantiate("Dog_Stage2", new Vector2(3, -1.8f), transform.rotation);
        DogavatarObj.GetComponent<PlayerCtrl_Dog>().SetMyAvatar();

        GameObject BG = DogavatarObj.transform.Find("Canvas").gameObject;                   //stage3�Ŏg��BG
        BG.SetActive(false);
        putinObj.SetActive(false);

        PlayerCtrl_Dog AIScript = DogavatarObj.GetComponent<PlayerCtrl_Dog>();
        AIScript.respawnPosx = 3;                                               //�v���C���[�̃��X�|�[���ʒu��ύX
        AIScript.respawnPosy = -1.8f;
    }
    void Clay()
    {
        AIanim = GameObject.Find("AI_Stage2(Clone)").GetComponent<Animator>();
        AIanim.runtimeAnimatorController = changeAnim[0];             //Animator��Controller��ύX[Sculpt]

        Doganim = GameObject.Find("Dog_Stage2(Clone)").GetComponent<Animator>();
        Doganim.runtimeAnimatorController = changeAnim[1];            //Animator��Controller��ύX[Sculpt]

    }
    void Paints()
    {
        AIanim = GameObject.Find("AI_Stage2(Clone)").GetComponent<Animator>();
        AIanim.runtimeAnimatorController = changeAnim[2];    //Animator��Controller��ύX[Texture]

        Doganim = GameObject.Find("Dog_Stage2(Clone)").GetComponent<Animator>();
        Doganim.runtimeAnimatorController = changeAnim[3];   //Animator��Controller��ύX[Texture]
    }

    public void Stage()                                      //���X�e�[�W�ړ����o//[PlayerCtrl]����Ă΂��
    {
        panel = 1;
        Invoke("Newstage", 4);
    }
    void Newstage()
    {
        panel = 2;
        AIavatarObj = GameObject.Find("AI_Stage2(Clone)");
        AIavatarObj.transform.position = new Vector2(292, 20);                 //stage3�ֈړ�

        DogavatarObj = GameObject.Find("Dog_Stage2(Clone)");
        DogavatarObj.transform.position = new Vector2(290, 22);

        GameObject BG = AIavatarObj.transform.Find("Canvas").gameObject;       //BG�\��
        BG.SetActive(true);
        GameObject BG1 = DogavatarObj.transform.Find("Canvas").gameObject;
        BG1.SetActive(true);
    }
    public void ShortStory()                                  //[PlayerCtrl]����Ă΂��
    {
        enemyObj.SetActive(true);
        shortstory = 1;
    }
    public void FinishLoad()                                  //�e�v���C���[Ctrl����Ă΂��
    {
        comScript.SendData(Stage2Ctrl.CMD_STAGE, "");         //�C�x���g����
    }
}