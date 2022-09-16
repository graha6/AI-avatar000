using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ////ベルコンから外れ、落っこちる処理
        Vector2 pos;
        pos = this.transform.position;
        if (pos.x < 0 & pos.x > -3)
        {
            GetComponent<PrototypeCtrl>().enabled = false;
            pos.y = pos.y - 0.08f;
            this.transform.eulerAngles = new Vector3(0, 0, 30.0f);
        }
        if (pos.y < -6)
        {
            SceneManager.LoadScene("Matching");
        }
        this.transform.position = pos;
    }

}
