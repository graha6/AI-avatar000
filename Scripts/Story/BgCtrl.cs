using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgCtrl : MonoBehaviour
{
    Vector2 pos;
    public float speed;

    Image BG;
    public Sprite ChangeSpr;    //�ύX�X�v���C�g

    public int scene;           //0:matchingScene,storyScene��p
                                //1:credittitleScene��p

    // Start is called before the first frame update
    void Start()
    {
        BG = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (scene)
        {
            case 0:
                pos = this.transform.position;
                pos.y = pos.y + speed;
                if (pos.y > 10.0f)
                {
                    pos.y = -10.0f;         //�߂�`�J��Ԃ�
                }
                this.transform.position = pos;
                break;
            case 1:
                pos = this.transform.position;
                pos.x = pos.x - speed;
                if (pos.x < -20.0f)
                {
                    pos.x = 30.0f;         //�߂�`�J��Ԃ�
                }
                this.transform.position = pos;
                break;
        }
        }
    public void Change()            //[StoryCtrl]���Ăяo�����
    {
        BG.sprite = ChangeSpr;
    }

    public void Fin()               //[StoryCtrl]���Ăяo�����
    {
        pos = this.transform.position;
        pos.y = pos.y + speed; 
        if (pos.y > 9.5f)
        {
            gameObject.SetActive(false);
        }
        this.transform.position = pos;
    }
}
