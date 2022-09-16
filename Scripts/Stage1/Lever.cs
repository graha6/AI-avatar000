using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Lever : MonoBehaviourPunCallbacks
{
    Transform tr;
    public Sprite ChangeSpr;

    public GameObject waste, woodenbox, longwoodenbox, waterObj,
                        wallObj,
                        lightObj;

    public GameObject stage2Waste = null;

    public float x, y;      //生成位置(ワールド座標)

    public int action;      //0:wasteObjが落ちてくる→表示off
                            //1:woodenboxObjが落ちてくる
                            //2:longwoodenboxObjが落ちてくる
                            //3:waterObjが出てくる[update=1]
                            //4:lightObjをOFF,wallObjをOFF
                            //5:lightObjをON    

    public int update;      //1:action3より連動


    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();

    }
    void Update()
    {


        switch (update)
        {
            case 1:
                Vector2 pos;
                pos = waterObj.transform.position;
                pos.y = pos.y + 0.02f;
                if (pos.y > -1.8f)
                {
                    pos.y = -1.8f;
                }
                waterObj.transform.position = pos;
                break;

            default:
                break;
        }
    }


    ////画像反転
    public void Change()
    {
        tr.localScale = new Vector3(-1, 1, 1);
    }

    ////レバー作動内容
    public void Action()
    {
        Vector2 v1 = new Vector2(x, y);

        switch (action)
        {
            case 0:
                GameObject wasteObj = PhotonNetwork.Instantiate(waste.name, v1, transform.rotation);
                LerverOther wasteScript = wasteObj.GetComponent<LerverOther>();
                wasteScript.other = 1;
                break;                                       //case 0で完結

            case 1:
                PhotonNetwork.Instantiate(woodenbox.name, v1, transform.rotation);
                break;                                       //case 1で完結

            case 2:
                PhotonNetwork.Instantiate(longwoodenbox.name, v1, transform.rotation);
                break;                                       //case 2で完結

            case 3:
                stage2Waste.SetActive(false);
                update = 1;
                break;                                       //case 3で完結

            case 4:
                LerverOther wallScript = wallObj.GetComponent<LerverOther>();
                wallScript.other = 4;
                LerverOther lightScript = lightObj.GetComponent<LerverOther>();
                lightScript.other = 2;

                break;                                       //case 4で完結
            case 5:
                lightScript = lightObj.GetComponent<LerverOther>();
                lightScript.other = 3;

                break;                                       //case 5で完結
        }
    }
}