using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////
// �ʐM�����N���X
// MonoBehaviourPunCallbacks���p����PUN�̃R�[���o�b�N������
////////////////////////////////////////////////////////////////////////////////
public class ComServer : MonoBehaviourPunCallbacks
{

    private int MasterFlg = 0;                              // 1:Master 0:Client
    private const string strName = "GameMaster";
    private const string strRoom = "Room";
    private const int buffLen = 6;                          // �o�b�t�@������


    private int ErrorNo = 0;                                // �G���[No 0:�G���[�Ȃ� 1-:�G���[�ԍ�
    private string[] rcvBuff;                               // �f�[�^�p�o�b�t�@
    private int rcvIdxW;                                    // ����Index
    private int rcvIdxR;                                    // �Ǎ�Index    
    private string strActNo;                                // ActorNo(PcNo)
    private int comState = 0;                               // -1:ERROR 
                                                            // 0:���ڑ� 
                                                            // 1:�}�X�^�T�[�o�[�ڑ���OK
                                                            // 2:�Q�[���T�[�o�[�ڑ� 
                                                            // 3:�Q�[���T�[�o�[�ڑ�OK
                                                            // 4:�}�X�^�N���C�A���g�ݒ�
                                                            // 5:�ڑ������i�ʐM�j

    private string strLogText;                                 // D

    private const int ERR_NONE = 0;
    private const int ERR_MASTAR_DISCONNECT = 1;
    private const int ERR_ROOM_DISCONNECT = 2;

    public struct PlayerInfo
    {
        public int actNo;                                   // �A�N�^�\No
        public int comState;                                // �v���C���[��� 0:DisConnect 1:Connect 
    }

    private PlayerInfo[] conPlayerList;                     // �ڑ����v���C���[���X�g
    private int conPlayerNum = 0;                           // �ڑ����v���C���[��

    public static readonly string CMD_TRANS = "01";

    ////////////////////////////////////////////////////////////////////////////////
    // Start����
    ////////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        strLogText = "";
        ErrorNo = 0;
        //        rcvBuff = new string[buffLen];                      // ��M�o�b�t�@
        rcvIdxW = 0;
        rcvIdxR = 0;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // Update����
    ////////////////////////////////////////////////////////////////////////////////
    void Update()
    {

        switch (comState)
        {
            case 1:
                //Debug.Log("�}�X�^�\join");
                PhotonNetwork.JoinOrCreateRoom(strRoom, new RoomOptions(), TypedLobby.Default); // ���[���Q������
                comState = 2;
                break;
            case 3:
                //Debug.Log("�N���C�A���gjoin");
                strActNo = PhotonNetwork.LocalPlayer.ActorNumber.ToString("D2");
                comState = 4;
                break;
            case 4:
                if (MasterFlg == 1)         //������̕ی�
                {
                    // �}�X�^�N���C�A���g�`�F�b�N
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("�}�X�^�[�A�C");
                        // �}�X�^�N���C�A���g�֐ݒ�
                        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                    }
                }
                comState = 5;
                break;
        }

    }

    ////////////////////////////////////////////////////////////////////////////////
    // �ʐM�J�n����
    // �����F  int MstFlg 1:Master 0:Client
    // �߂�l�F�Ȃ�
    ////////////////////////////////////////////////////////////////////////////////
    public void ComStart(int MstFlg = 0)
    {
        MasterFlg = MstFlg;

        PhotonNetwork.NickName = strName;                   // �v���C���[���̐ݒ�
        PhotonNetwork.ConnectUsingSettings();               // PhotonServerSettings�ݒ���e�Ń}�X�^�[�T�[�o�[�֐ڑ�

        rcvBuff = new string[buffLen];                      // ��M�o�b�t�@
        for (int idx = 0; idx < buffLen; idx++)
        {              // �o�b�t�@������
            rcvBuff[idx] = "";
        }

        strLogText = "START(" + MasterFlg + ")";
        strActNo = "";
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �}�X�^�[�T�[�o�[�ڑ�����������
    //�iConnectUsingSettings()�̃R�[���o�b�N�j
    ////////////////////////////////////////////////////////////////////////////////
    public override void OnConnectedToMaster()
    {
        comState = 1;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �}�X�^�[�T�[�o�[�ڑ����s������
    //�iConnectUsingSettings()�̃R�[���o�b�N�j
    ////////////////////////////////////////////////////////////////////////////////
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        ErrorNo = ERR_MASTAR_DISCONNECT;    // �}�X�^�T�[�o�[�ڑ����s
        comState = -1;
        // ���g���C����H�H�H�@comState = 0;??
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �Q�[���T�[�o�[�ڑ�����������
    //�iJoinOrCreateRoom()�̃R�[���o�b�N�j
    ////////////////////////////////////////////////////////////////////////////////
    public override void OnJoinedRoom()
    {
        //setMessageQueueRunning(false);        //[Stage1]�Ƀ��b�Z�[�W�𑗐M
        comState = 3;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �Q�[���T�[�o�[�ڑ����s������
    //�iJoinOrCreateRoom()�̃R�[���o�b�N�j
    ////////////////////////////////////////////////////////////////////////////////
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        ErrorNo = ERR_ROOM_DISCONNECT;    // �Q�[���T�[�o�[�ڑ����s
        comState = -1;
        // ���g���C����H�H�H�@comState = 0;??
    }

    ////////////////////////////////////////////////////////////////////////////////
    // isComEnable�擾����
    // �����F  �Ȃ�
    // �߂�l�Fbool  isComEnable TRUE:�ʐM�� FALSE:�ʐM�s�� 
    ////////////////////////////////////////////////////////////////////////////////
    public bool isComEnable()
    {
        bool bRet = false;

        if (comState > 4)
        {
            bRet = true;
        }
        return bRet;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �ؒf����
    // �����F  �Ȃ�
    // �߂�l�F�Ȃ�
    ////////////////////////////////////////////////////////////////////////////////
    public void DisConnect()
    {
        PhotonNetwork.Disconnect();
    }

    ////////////////////////////////////////////////////////////////////////////////
    // ActorNo�擾����
    // �����F  �Ȃ�
    // �߂�l�Fint  ActorNo
    ////////////////////////////////////////////////////////////////////////////////
    public int getActorNo()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �f�[�^���M����
    // �����F  string  ���M�f�[�^������
    // �߂�l�F�Ȃ�
    ////////////////////////////////////////////////////////////////////////////////
    public void SendData(string cmd, string msg)
    {

        string buff;

        if (MasterFlg == 1)
        {
            buff = strActNo + cmd + msg;
            photonView.RPC(nameof(RpcSendMessage), RpcTarget.AllViaServer, buff);       //RpcTarget ��ύX
            strLogText = "SEND(M)::" + MasterFlg + "::" + buff;
        }

        else
        {
            buff = strActNo + cmd + msg;
            photonView.RPC(nameof(RpcSendMessage), RpcTarget.AllViaServer, buff);       //RpcTarget ��ύX
            strLogText = "SEND(C)::" + MasterFlg + "::" + buff;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    // ��M�f�[�^�`�F�b�N����
    // �����F  �Ȃ�
    // �߂�l�Fbool TRUE:��M�f�[�^����  FALSE:��M�f�[�^�Ȃ�
    ////////////////////////////////////////////////////////////////////////////////
    public bool chkRcvData()
    {

        bool retFlg = false;

        if (rcvIdxW != rcvIdxR)
        {
            retFlg = true;
            strLogText = "chkRcvData::" + rcvIdxW + "::" + rcvIdxR + "::" + retFlg;
        }

        return retFlg;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // ��M�f�[�^�擾����
    // �����F  �Ȃ�
    // �߂�l�Fstring ��M�f�[�^
    // ���l�F�f�[�^��M�`�F�b�N��Ƀf�[�^�擾���s�����ƁI
    ////////////////////////////////////////////////////////////////////////////////
    public string getRcvData()
    {

        string retBuff;
        retBuff = rcvBuff[rcvIdxR];

        if (rcvIdxW != rcvIdxR)
        {
            rcvIdxR++;
            if (rcvIdxR >= buffLen)
            {
                rcvIdxR = 0;
            }
        }

        strLogText = "getRcvData::" + rcvIdxW + "::" + rcvIdxR + "::" + retBuff;

        return retBuff;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �}�X�^�N���C�A���g�`�F�b�N����
    // �����F  �Ȃ�
    // �߂�l�Fbool TRUE:�}�X�^�N���C�A���g FALSE:�N���C�A���g
    ////////////////////////////////////////////////////////////////////////////////
    public bool chkMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public string getComState()
    {
        string retStr = strLogText;
        strLogText = "";
        return retStr;
    }

    public int getErrorNo()
    {
        return ErrorNo;
    }


    ////////////////////////////////////////////////////////////////////////////////
    // �v���C���[���`�F�b�N����
    // �����F  �Ȃ�
    // �߂�l�Fbool TRUE:�}�X�^�N���C�A���g FALSE:�N���C�A���g
    ////////////////////////////////////////////////////////////////////////////////
    public int getPlayerNum()
    {
        return PhotonNetwork.PlayerList.Length;
        //        return conPlayerNum;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �v���C���[���X�g�쐬����
    // �����F  �Ȃ�
    // �߂�l�Fint ���X�g�쐬���v���C���[��
    ////////////////////////////////////////////////////////////////////////////////
    public int createPlayerList()
    {
        int idx = 0;

        conPlayerNum = PhotonNetwork.PlayerList.Length;
        conPlayerList = new PlayerInfo[conPlayerNum];

        foreach (var player in PhotonNetwork.PlayerList)
        {
            conPlayerList[idx].actNo = player.ActorNumber;
            conPlayerList[idx].comState = 1;
            idx++;
        }
        return conPlayerNum;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �v���C���[���X�g�f�[�^�擾����
    // �����F  �Ȃ�
    // �߂�l�Fbool True:OK False:NG
    ////////////////////////////////////////////////////////////////////////////////
    public bool getPlayerList(int idx, ref string actNo, ref int comState)
    {
        bool bRet = false;

        if (idx < conPlayerNum)
        {
            actNo = conPlayerList[idx].actNo.ToString("D2");
            comState = conPlayerList[idx].comState;
            bRet = true;
        }
        return bRet;
    }

    // ���v���C���[�����[������ޏo�������ɌĂ΂��R�[���o�b�N
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        for (int idx = 0; idx < conPlayerNum; idx++)
        {
            if (conPlayerList[idx].actNo == otherPlayer.ActorNumber)
            {
                conPlayerList[idx].comState = 0;
                break;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    // �f�[�^��M�擾����
    // �����F  string ���M�f�[�^������
    // �߂�l�F�Ȃ�
    ////////////////////////////////////////////////////////////////////////////////
    [PunRPC]
    private void RpcSendMessage(string msg)
    {
        rcvBuff[rcvIdxW] = msg;
        rcvIdxW++;
        if (rcvIdxW >= buffLen)
        {
            rcvIdxW = 0;
        }
    }

    ////�ǉ�
    ////////////////////////////////////////////////////////////////////////////////
    // ���b�Z�[�W�L���[OM/OFF�ݒ菈��
    // �����F  flg true:ON  false:OFF
    // �߂�l�F�Ȃ�
    ////////////////////////////////////////////////////////////////////////////////
    //�l�b�g���[�N�I�u�W�F�N�g��ێ�����V�[�����j�������ƁA�l�b�g���[�N�I�u�W�F�N�g�������ɔj������Ă�
    //���̖��̉���̈�
    public void setMessageQueueRunning(bool flg)
    {
        PhotonNetwork.IsMessageQueueRunning = flg;
    }

}