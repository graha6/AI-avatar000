using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;           //�ǉ�

public class Enemy : MonoBehaviour
{
    Transform target = null;
    private NavMeshAgent agent;

    public int player;          //1:AI�v���C���[�ɑΉ�
                                //2:Dog�v���C���[�ɑΉ�

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(292, 20);
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;       //�K�{
        agent.updateUpAxis = false;         //�K�{
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (player)
        {
            case 1:
                target = GameObject.Find("AI_Stage2(Clone)").transform; //�ڕW���W�ݒ�
                agent.SetDestination(target.position);                  //�ǔ��J�n
                break;
            case 2:
                target = GameObject.Find("Dog_Stage2(Clone)").transform;
                agent.SetDestination(target.position);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (player)                                                 //�Ή��v���C���[�ȊO�ɐڐG���Ă������Ȃ�
        {
            case 1:
                if (collision.gameObject.name == "AI_Stage2(Clone)")
                {
                    this.gameObject.SetActive(false);
                    transform.position = new Vector2(292, 20);          //�������W
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