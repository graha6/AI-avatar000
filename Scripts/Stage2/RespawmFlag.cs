using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawmFlag : MonoBehaviour
{
   Vector2 pos;
                                    
    public void Changerespawn()         //PlayerCtrl����Ă΂��
    {
        pos = transform.position;       //���̃I�u�W�F�N�g�̈ʒu�擾
        GameObject AIplayer = GameObject.FindGameObjectWithTag("AI_Player");
        PlayerCtrl AIScript = AIplayer.GetComponent<PlayerCtrl>();
        AIScript.respawnPosx = pos.x;   //�v���C���[�̃��X�|�[���ʒu��ύX
        AIScript.respawnPosy = pos.y;
    }
    public void Changerespawn1()        //PlayerCtrl_Dog����Ă΂��
    {
        pos = transform.position;
        GameObject Dogplayer = GameObject.FindGameObjectWithTag("Dog_Player");
        PlayerCtrl_Dog DogScript = Dogplayer.GetComponent<PlayerCtrl_Dog>();
        DogScript.respawnPosx = pos.x;
        DogScript.respawnPosy = pos.y;
    }
    public void Changefloat()           //PlayerCtrl����Ă΂��
    {
        GameObject AIplayer = GameObject.FindGameObjectWithTag("AI_Player");
        PlayerCtrl AIScript = AIplayer.GetComponent<PlayerCtrl>();
        AIScript.defaultrun = 300;             //�ϐ��ύX
        AIScript.defaultjump = 2300;
    }
    public void Changefloat1()          //PlayerCtrl_Dog����Ă΂��
    {
        GameObject Dogplayer = GameObject.FindGameObjectWithTag("Dog_Player");
        PlayerCtrl_Dog DogScript = Dogplayer.GetComponent<PlayerCtrl_Dog>();
        DogScript.defaultrun = 300;            //�ϐ��ύX
        DogScript.defaultjump = 2300;
    }
}
