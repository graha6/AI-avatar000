using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////
// 通信処理クラス
// MonoBehaviourPunCallbacksを継承しPUNのコールバックを受取る
////////////////////////////////////////////////////////////////////////////////
public class ComServer : MonoBehaviourPunCallbacks
{

    private int MasterFlg = 0;                              // 1:Master 0:Client
    private const string strName = "GameMaster";
    private const string strRoom = "Room";
    private const int buffLen = 6;                          // バッファ文字列長


    private int ErrorNo = 0;                                // エラーNo 0:エラーなし 1-:エラー番号
    private string[] rcvBuff;                               // データ用バッファ
    private int rcvIdxW;                                    // 書込Index
    private int rcvIdxR;                                    // 読込Index    
    private string strActNo;                                // ActorNo(PcNo)
    private int comState = 0;                               // -1:ERROR 
                                                            // 0:未接続 
                                                            // 1:マスタサーバー接続待OK
                                                            // 2:ゲームサーバー接続 
                                                            // 3:ゲームサーバー接続OK
                                                            // 4:マスタクライアント設定
                                                            // 5:接続完了（通信可）

    private string strLogText;                                 // D

    private const int ERR_NONE = 0;
    private const int ERR_MASTAR_DISCONNECT = 1;
    private const int ERR_ROOM_DISCONNECT = 2;

    public struct PlayerInfo
    {
        public int actNo;                                   // アクタ―No
        public int comState;                                // プレイヤー状態 0:DisConnect 1:Connect 
    }

    private PlayerInfo[] conPlayerList;                     // 接続中プレイヤーリスト
    private int conPlayerNum = 0;                           // 接続中プレイヤー数

    public static readonly string CMD_TRANS = "01";

    ////////////////////////////////////////////////////////////////////////////////
    // Start処理
    ////////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        strLogText = "";
        ErrorNo = 0;
        //        rcvBuff = new string[buffLen];                      // 受信バッファ
        rcvIdxW = 0;
        rcvIdxR = 0;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // Update処理
    ////////////////////////////////////////////////////////////////////////////////
    void Update()
    {

        switch (comState)
        {
            case 1:
                //Debug.Log("マスタ―join");
                PhotonNetwork.JoinOrCreateRoom(strRoom, new RoomOptions(), TypedLobby.Default); // ルーム参加処理
                comState = 2;
                break;
            case 3:
                //Debug.Log("クライアントjoin");
                strActNo = PhotonNetwork.LocalPlayer.ActorNumber.ToString("D2");
                comState = 4;
                break;
            case 4:
                if (MasterFlg == 1)         //万が一の保険
                {
                    // マスタクライアントチェック
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("マスター就任");
                        // マスタクライアントへ設定
                        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                    }
                }
                comState = 5;
                break;
        }

    }

    ////////////////////////////////////////////////////////////////////////////////
    // 通信開始処理
    // 引数：  int MstFlg 1:Master 0:Client
    // 戻り値：なし
    ////////////////////////////////////////////////////////////////////////////////
    public void ComStart(int MstFlg = 0)
    {
        MasterFlg = MstFlg;

        PhotonNetwork.NickName = strName;                   // プレイヤー名称設定
        PhotonNetwork.ConnectUsingSettings();               // PhotonServerSettings設定内容でマスターサーバーへ接続

        rcvBuff = new string[buffLen];                      // 受信バッファ
        for (int idx = 0; idx < buffLen; idx++)
        {              // バッファ初期化
            rcvBuff[idx] = "";
        }

        strLogText = "START(" + MasterFlg + ")";
        strActNo = "";
    }

    ////////////////////////////////////////////////////////////////////////////////
    // マスターサーバー接続成功時処理
    //（ConnectUsingSettings()のコールバック）
    ////////////////////////////////////////////////////////////////////////////////
    public override void OnConnectedToMaster()
    {
        comState = 1;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // マスターサーバー接続失敗時処理
    //（ConnectUsingSettings()のコールバック）
    ////////////////////////////////////////////////////////////////////////////////
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        ErrorNo = ERR_MASTAR_DISCONNECT;    // マスタサーバー接続失敗
        comState = -1;
        // リトライする？？？　comState = 0;??
    }

    ////////////////////////////////////////////////////////////////////////////////
    // ゲームサーバー接続成功時処理
    //（JoinOrCreateRoom()のコールバック）
    ////////////////////////////////////////////////////////////////////////////////
    public override void OnJoinedRoom()
    {
        //setMessageQueueRunning(false);        //[Stage1]にメッセージを送信
        comState = 3;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // ゲームサーバー接続失敗時処理
    //（JoinOrCreateRoom()のコールバック）
    ////////////////////////////////////////////////////////////////////////////////
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        ErrorNo = ERR_ROOM_DISCONNECT;    // ゲームサーバー接続失敗
        comState = -1;
        // リトライする？？？　comState = 0;??
    }

    ////////////////////////////////////////////////////////////////////////////////
    // isComEnable取得処理
    // 引数：  なし
    // 戻り値：bool  isComEnable TRUE:通信可 FALSE:通信不可 
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
    // 切断処理
    // 引数：  なし
    // 戻り値：なし
    ////////////////////////////////////////////////////////////////////////////////
    public void DisConnect()
    {
        PhotonNetwork.Disconnect();
    }

    ////////////////////////////////////////////////////////////////////////////////
    // ActorNo取得処理
    // 引数：  なし
    // 戻り値：int  ActorNo
    ////////////////////////////////////////////////////////////////////////////////
    public int getActorNo()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // データ送信処理
    // 引数：  string  送信データ文字列
    // 戻り値：なし
    ////////////////////////////////////////////////////////////////////////////////
    public void SendData(string cmd, string msg)
    {

        string buff;

        if (MasterFlg == 1)
        {
            buff = strActNo + cmd + msg;
            photonView.RPC(nameof(RpcSendMessage), RpcTarget.AllViaServer, buff);       //RpcTarget を変更
            strLogText = "SEND(M)::" + MasterFlg + "::" + buff;
        }

        else
        {
            buff = strActNo + cmd + msg;
            photonView.RPC(nameof(RpcSendMessage), RpcTarget.AllViaServer, buff);       //RpcTarget を変更
            strLogText = "SEND(C)::" + MasterFlg + "::" + buff;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    // 受信データチェック処理
    // 引数：  なし
    // 戻り値：bool TRUE:受信データあり  FALSE:受信データなし
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
    // 受信データ取得処理
    // 引数：  なし
    // 戻り値：string 受信データ
    // 備考：データ受信チェック後にデータ取得を行うこと！
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
    // マスタクライアントチェック処理
    // 引数：  なし
    // 戻り値：bool TRUE:マスタクライアント FALSE:クライアント
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
    // プレイヤー数チェック処理
    // 引数：  なし
    // 戻り値：bool TRUE:マスタクライアント FALSE:クライアント
    ////////////////////////////////////////////////////////////////////////////////
    public int getPlayerNum()
    {
        return PhotonNetwork.PlayerList.Length;
        //        return conPlayerNum;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // プレイヤーリスト作成処理
    // 引数：  なし
    // 戻り値：int リスト作成時プレイヤー数
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
    // プレイヤーリストデータ取得処理
    // 引数：  なし
    // 戻り値：bool True:OK False:NG
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

    // 他プレイヤーがルームから退出した時に呼ばれるコールバック
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
    // データ受信取得処理
    // 引数：  string 送信データ文字列
    // 戻り値：なし
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

    ////追加
    ////////////////////////////////////////////////////////////////////////////////
    // メッセージキューOM/OFF設定処理
    // 引数：  flg true:ON  false:OFF
    // 戻り値：なし
    ////////////////////////////////////////////////////////////////////////////////
    //ネットワークオブジェクトを保持するシーンが破棄されると、ネットワークオブジェクトも同時に破棄されてる
    //その問題の回避の為
    public void setMessageQueueRunning(bool flg)
    {
        PhotonNetwork.IsMessageQueueRunning = flg;
    }

}