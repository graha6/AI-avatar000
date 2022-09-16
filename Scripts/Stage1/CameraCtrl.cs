using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public Transform player;// �v���C���[��Transform
    
    //��ʒ[�ł̓J�������~�܂�悤�ɐݒ�//�㕔�����������Ȃ��悤�ɐݒ�
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
             Mathf.Clamp(player.position.x, defaultcamaraMinPos.x, defaultcamaraMaxPos.x), // �J�����̍��E�𐧌�
             Mathf.Clamp(player.position.y, defaultcamaraMinPos.y, defaultcamaraMaxPos.y), // �J�����̏㉺�𐧌�
             -10f);
    }
}
