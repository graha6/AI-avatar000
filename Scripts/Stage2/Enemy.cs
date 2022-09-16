using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;           //追加

public class Enemy : MonoBehaviour
{
    Transform target = null;
    private NavMeshAgent agent;

    public int player;          //1:AIプレイヤーに対応
                                //2:Dogプレイヤーに対応

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(292, 20);
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;       //必須
        agent.updateUpAxis = false;         //必須
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (player)
        {
            case 1:
                target = GameObject.Find("AI_Stage2(Clone)").transform; //目標座標設定
                agent.SetDestination(target.position);                  //追尾開始
                break;
            case 2:
                target = GameObject.Find("Dog_Stage2(Clone)").transform;
                agent.SetDestination(target.position);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (player)                                                 //対応プレイヤー以外に接触しても消えない
        {
            case 1:
                if (collision.gameObject.name == "AI_Stage2(Clone)")
                {
                    this.gameObject.SetActive(false);
                    transform.position = new Vector2(292, 20);          //初動座標
                }
                break;
            case 2:
                if (collision.gameObject.name == "Dog_Stage2(Clone)")
                {
                    this.gameObject.SetActive(false);
                    transform.position = new Vector2(292, 20);
                }
                break;
        }

    }
}