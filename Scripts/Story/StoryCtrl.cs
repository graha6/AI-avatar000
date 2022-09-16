using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryCtrl : MonoBehaviour
{
    private int story;          //0:boPanel操作
                                //1:chara操作＆boPanel操作
                                //2:storyキャプチャ操作①
                                //3:storyキャプチャ操作①②
                                //4:storyキャプチャ操作②③③
                                //5:シーン遷移
    float wTime;
    public float speed;         //alpha,alpha_story

    GameObject boPanel;
    float alpha;

    GameObject[] BG;            //tagにて全取得

    public GameObject[] storyObj;
    float alpha_story;          //storyObj[2]のcolor

    // Start is called before the first frame update
    void Start()
    {
        alpha = 0;
        boPanel = GameObject.Find("blackout");
        boPanel.GetComponent<Image>().color = new Color(0, 0, 0, alpha);

        BG = GameObject.FindGameObjectsWithTag("BG");

        wTime = 0;
        alpha_story = 1;
        storyObj[2].GetComponent<Image>().color = new Color(255, 255, 255, alpha_story);

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos;

        switch (story)
        {
            case 0:
                wTime += Time.deltaTime;
                if (wTime < 7 & wTime > 5)
                {
                    boPanel.GetComponent<Image>().color = new Color(0, 0, 0, alpha);    //暗転表現
                    alpha += speed * Time.deltaTime;
                }

                if (wTime > 8)
                {
                    foreach (GameObject bgObj in BG)                                    //取得したオブジェクト全てに処理
                    {
                        bgObj.GetComponent<BgCtrl>().Change();
                    }
                    boPanel.GetComponent<Image>().color = new Color(0, 0, 0, alpha);    //目覚め表現
                    alpha -= speed * Time.deltaTime;
                }
                if (wTime > 10)
                {
                    story = 1;
                    wTime = 0;
                }
                break;
            case 1:
                GameObject chara = GameObject.Find("ai");
                pos = chara.transform.position;
                pos.y = pos.y - 0.05f;
                if (pos.y < -10)
                {
                    pos.y = -10;
                    wTime += Time.deltaTime;
                    if (wTime > 1)
                    {
                        story = 2;
                        wTime = 0;
                    }
                }
                chara.transform.position = pos;
                foreach (GameObject bgObj in BG)    //取得したオブジェクト全てに処理
                {
                    bgObj.GetComponent<BgCtrl>().Fin();
                }
                break;
            case 2:
                pos = storyObj[0].transform.position;
                pos.y = pos.y + 0.1f;               //①下方から出現
                if (pos.y > 0)
                {
                    pos.y = 0;
                    wTime += Time.deltaTime;
                    if (wTime > 2)
                    {
                        story = 3;
                        wTime = 0;
                    }
                }
                storyObj[0].transform.position = pos;
                break;
            case 3:
                pos = storyObj[0].transform.position;
                pos.x = pos.x - 0.1f;               //①左方に退場
                if (pos.x < -20)
                {
                    pos.x = -20;
                }
                storyObj[0].transform.position = pos;

                pos = storyObj[1].transform.position;
                pos.x = pos.x - 0.1f;               //②右方から出現
                if (pos.x < 0)
                {
                    pos.x = 0;
                    wTime += Time.deltaTime;
                    if (wTime > 2)
                    {
                        story = 4;
                        wTime = 0;
                    }
                }
                storyObj[1].transform.position = pos;
                break;
            case 4:
                pos = storyObj[1].transform.position;
                pos.y = pos.y + 0.1f;               //②上方に退場
                if (pos.y > 13)
                {
                    pos.y = 13;
                }
                storyObj[1].transform.position = pos;

                pos = storyObj[2].transform.position;
                pos.y = pos.y + 0.1f;               //③下方から出現
                if (pos.y > 0)
                {
                    pos.y = 0;
                    wTime += Time.deltaTime;
                    if (wTime > 5)
                    {
                        storyObj[2].GetComponent<Image>().color = new Color(255, 255, 255, alpha_story);
                        alpha_story -= speed * Time.deltaTime;      //③f.o
                    }
                    if (wTime > 8.5f)
                    {
                        story = 5;
                    }
                }
                storyObj[2].transform.position = pos;
                break;
            case 5:
                SceneManager.LoadScene("Stage1");
                break;

        }

    }
}