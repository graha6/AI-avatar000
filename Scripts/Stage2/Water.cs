using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Water : MonoBehaviourPunCallbacks
{
    Image img;
    float red = 1;                                 //img��red�l
    Animator anim;

    public GameObject[] waters = null;             //�q�I�u�W�F�N�g
    Image imgs, imgs1;                             //waters[]��Image
    Animator anims, anims1,animVepor, animVepor1;  //waters[]��Animator,vepor[]��Animator
    public GameObject[] vepor = null;              //���C�I�u�W�F�N�g


    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();                    //�e�I�u�W�F�N�g�̏���
        img.color = new Color(red, 255, 255, 255);
        anim = GetComponent<Animator>();

        imgs = waters[0].GetComponent<Image>();         //�q�I�u�W�F�N�g�̏���
        imgs1 = waters[1].GetComponent<Image>();
        imgs.color = new Color(red, 255, 255, 255);
        imgs1.color = new Color(red, 255, 255, 255);
        anims = waters[0].GetComponent<Animator>();
        anims1 = waters[1].GetComponent<Animator>();

        vepor[0].SetActive(false);                      //�������̏���
        vepor[1].SetActive(false);
        animVepor = vepor[0].GetComponent<Animator>();
        animVepor1 = vepor[1].GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Snowboll")
        {
            red -= 0.5f * Time.deltaTime;
            img.color = new Color(red, 255, 255, 255);  //Color��ύX
            anim.enabled = false;                       //Animator��OFF
            this.tag = "Ground";                        //tag��ύX

            imgs.color = new Color(red, 255, 255, 255);     
            anims.enabled = false;
            imgs1.color = new Color(red, 255, 255, 255);
            anims1.enabled = false;

            Invoke("Other", 2);
        }
    }

    void Other()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = false;

        vepor[0].SetActive(true);
        vepor[1].SetActive(true);
        animVepor.enabled = true;                       //Animator��ON
        animVepor1.enabled = true;
    }
}
