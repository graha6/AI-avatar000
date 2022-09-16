using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCtrl : MonoBehaviour
{
    Animator logo;
    Animator space;

    GameObject[] Prototype;            //tagにて全取得

    // Start is called before the first frame update
    void Start()
    {
        Prototype = GameObject.FindGameObjectsWithTag("Prototype");
        foreach (GameObject PrototypeObj in Prototype)             //取得したオブジェクト全てに処理
        {
            PrototypeObj.GetComponent<Fall>().enabled = false; ;
        }

        GameObject logoObj = GameObject.Find("logo");
        logo = logoObj.GetComponent<Animator>();
        logo.enabled = false;
        GameObject spaceObj = GameObject.Find("space");
        space = spaceObj.GetComponent<Animator>();
        space.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            logo.enabled = true;
            space.enabled = true;
            foreach (GameObject PrototypeObj in Prototype)            //取得したオブジェクト全てに処理
            {
                PrototypeObj.GetComponent<Fall>().enabled = true;
            }
        }
    }
}
     
