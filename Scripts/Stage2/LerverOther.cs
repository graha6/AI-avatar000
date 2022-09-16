using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LerverOther : MonoBehaviourPunCallbacks
{
    public int other;

    float wTime;
    // Start is called before the first frame update
    void Start()
    {
        wTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
       switch (other)
        {
            case 1:
                wTime += Time.deltaTime;
                if (wTime > 4)
                {
                    CircleCollider2D col = this.GetComponent<CircleCollider2D>();
                    col.isTrigger = true;
                    wTime = 0;
                }               
               break;
            case 2:
                this.transform.rotation = Quaternion.Euler(50, 100, 0);
                break;
            case 3:
                this.transform.rotation = Quaternion.Euler(50, -30, 0);
                break;
            case 4:
                this.transform.position = new Vector2(100, 27);
                break;
        }        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Water_bule")
        {
            CapsuleCollider2D col = this.GetComponent<CapsuleCollider2D>();
        }
        if (collision.gameObject.tag == "Return")
        {
            this.gameObject.transform.position = new Vector2(27.5f, 3.16f);
            //this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 6);
        }
    }
}
