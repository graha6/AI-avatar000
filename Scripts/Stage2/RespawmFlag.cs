using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawmFlag : MonoBehaviour
{
   Vector2 pos;
                                    
    public void Changerespawn()         //PlayerCtrlから呼ばれる
    {
        pos = transform.position;       //このオブジェクトの位置取得
        GameObject AIplayer = GameObject.FindGameObjectWithTag("AI_Player");
        PlayerCtrl AIScript = AIplayer.GetComponent<PlayerCtrl>();
        AIScript.respawnPosx = pos.x;   //プレイヤーのリスポーン位置を変更
        AIScript.respawnPosy = pos.y;
    }
    public void Changerespawn1()        //PlayerCtrl_Dogから呼ばれる
    {
        pos = transform.position;
        GameObject Dogplayer = GameObject.FindGameObjectWithTag("Dog_Player");
        PlayerCtrl_Dog DogScript = Dogplayer.GetComponent<PlayerCtrl_Dog>();
        DogScript.respawnPosx = pos.x;
        DogScript.respawnPosy = pos.y;
    }
    public void Changefloat()           //PlayerCtrlから呼ばれる
    {
        GameObject AIplayer = GameObject.FindGameObjectWithTag("AI_Player");
        PlayerCtrl AIScript = AIplayer.GetComponent<PlayerCtrl>();
        AIScript.defaultrun = 300;             //変数変更
        AIScript.defaultjump = 2300;
    }
    public void Changefloat1()          //PlayerCtrl_Dogから呼ばれる
    {
        GameObject Dogplayer = GameObject.FindGameObjectWithTag("Dog_Player");
        PlayerCtrl_Dog DogScript = Dogplayer.GetComponent<PlayerCtrl_Dog>();
        DogScript.defaultrun = 300;            //変数変更
        DogScript.defaultjump = 2300;
    }
}
