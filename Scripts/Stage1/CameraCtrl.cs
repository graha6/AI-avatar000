using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public Transform player;// プレイヤーのTransform
    
    //画面端ではカメラが止まるように設定//上部が見えすぎないように設定
    public Vector2 defaultcamaraMaxPos = new Vector2(240.0f,16.0f);      
    public Vector2 defaultcamaraMinPos = new Vector2(-13.5f, -20.0f);        

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
             Mathf.Clamp(player.position.x, defaultcamaraMinPos.x, defaultcamaraMaxPos.x), // カメラの左右を制限
             Mathf.Clamp(player.position.y, defaultcamaraMinPos.y, defaultcamaraMaxPos.y), // カメラの上下を制限
             -10f);
    }
}
