using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Water : MonoBehaviourPunCallbacks
{
    Image img;
    float red = 1;                                 //imgのred値
    Animator anim;

    public GameObject[] waters = null;             //子オブジェクト
    Image imgs, imgs1;                             //waters[]のImage
    Animator anims, anims1,animVepor, animVepor1;  //waters[]のAnimator,vepor[]のAnimator
    public GameObject[] vepor = null;              //蒸気オブジェクト


    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();                    //親オブジェクトの処理
        img.color = new Color(red, 255, 255, 255);
        anim = GetComponent<Animator>();

        imgs = waters[0].GetComponent<Image>();         //子オブジェクトの処理
        imgs1 = waters[1].GetComponent<Image>();
        imgs.color = new Color(red, 255, 255, 255);
        imgs1.color = new Color(red, 255, 255, 255);
        anims = waters[0].GetComponent<Animator>();
        anims1 = waters[1].GetComponent<Animator>();

        vepor[0].SetActive(false);                      //蒸発煙の処理
        vepor[1].SetActive(false);
        animVepor = vepor[0].GetComponent<Animator>();
        animVepor1 = vepor[1].GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Snowboll")
        {
            red -= 0.5f * Time.deltaTime;
            img.color = new Color(red, 255, 255, 255);  //Colorを変更
            anim.enabled = false;                       //AnimatorをOFF
            this.tag = "Ground";                        //tagを変更

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
        animVepor.enabled = true;                       //AnimatorをON
        animVepor1.enabled = true;
    }
}
