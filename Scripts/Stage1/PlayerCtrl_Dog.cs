using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerCtrl_Dog : MonoBehaviourPun   //�ύX
{
    public float defaultrun = 50f, defaultjump = 1900f;                 //�v���C���[�ړ�

    Rigidbody2D rb;
    Animator anim;
    //SpriteRenderer spr;                     //Flip(���E���])
    Transform tr;
    public Sprite fixedspr;                 //�ؔ����쎞�̉摜�Œ�
    public float respawnPosx, respawnPosy;  //���X�^�[�g���W
    ParticleSystem particle;
    public GameObject pointlightObj;
    private bool isGround,                  //�n�ʂɐڐG���Ă��邩
                                            isOperation,               //�v���C���[����\��
                                                                       //isWoodenbox, isN,          //woodenbox(�ؔ�)����\��,�s�\��
                 isScale,                   //�摜���]���邩
                 isCrush;                   //�ׂ���Ă��邩

    [SerializeField]
    ContactFilter2D filter2d_down, //player�̉���(jump����)�͈�
                                     filter2d_up;  //player�̏��(�ׂ��ꔻ��=isCrush)�͈�

    //private GameObject woodenbox = null;            //OnCollisionEnter2D�Ŏ擾����woodenbox�I�u�W�F�N�g
    //Rigidbody2D woodrb;                             //woodenbox(�ؔ�)�̃R���|�\�l���g
    //public BoxCollider2D colLR;                     //woodenbox�I�u�W�F�N�g�̍��E�ꕔ��collider

    [SerializeField] GameObject _myCamera = default;
    bool _myAvatarFlg = false;

    GameObject respawmFlagObj = null;         //���X�|�[�����W�ύX����I�u�W�F�N�g

    ParticleSystem AIParticle = null;        //AI��ParticleSystem

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //spr = GetComponent<SpriteRenderer>();
        tr = GetComponent<Transform>();
        particle = GetComponentInChildren<ParticleSystem>();    //�q�I�u�W�F�N�g����擾
        pointlightObj.gameObject.SetActive(false);
        isGround = false;
        isOperation = false;
        //isWoodenbox = false;
        //isN = false;
        isScale = true;                     //isWoodenbox���ȊO�͏펞
        isCrush = false;

        if (_myAvatarFlg == false)          //�A�o�^�[����������J��������
        {
            Destroy(_myCamera);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //�d�v�I
        //���͑��삪���̃v���C���[�L�����ɉe������Ȃ��悤����
        if (!photonView.IsMine) return;

        //�n�ʂƂ̐ڐG������擾
        isGround = rb.IsTouching(filter2d_down);

        ////jump����
        if (Input.GetKeyDown("space") & isGround)
        {
            rb.AddForce(Vector2.up * defaultjump);
        }

        //�n�ʐڐG���莞�ɏ���
        if (isGround)
        {
            anim.SetBool("isJump", false);
            anim.SetBool("isFall", false);
        }

        //Player�̕������x�����o
        float velX = rb.velocity.x;
        float velY = rb.velocity.y;

        //�W�����v���̈ړ����x�x�N�g����������
        if (Mathf.Abs(velX) > 3)
        {
            if (velX > 3)
            {
                rb.velocity = new Vector2(3, velY);
            }
            if (velX < -3)
            {
                rb.velocity = new Vector2(-3, velY);
            }
        }

        //�������x����anim�̔��菈��
        if (velY > 0.5f)
        {
            anim.SetBool("isJump", true);
        }
        if (velY < -0.5f)
        {
            anim.SetBool("isFall", true);
        }

        /*
        //�ؔ�����ɂ��ؔ���ԕύX�Ɖe������
        //�ؔ���͂�(�e�q�ɂȂ�)����(�������ł��ĂȂ�)
        if (Input.GetKeyDown(KeyCode.M) & isWoodenbox)
        {
            if (woodenbox != null)      //�uwoodenbox�I�u�W�F�N�g�͈͊O�ł͂Ȃ����v
            {
                if (woodenbox.gameObject.tag == "Box")
                {
                    woodrb = woodenbox.GetComponent<Rigidbody2D>();
                    woodrb.isKinematic = true;
                    woodenbox.gameObject.transform.parent = this.gameObject.transform;  //�q�ɂȂ�I�u�W�F�N�g=�e�I�u�W�F�N�g

                    GetComponent<Animator>().enabled = false;
                    spr.sprite = fixedspr;      //�摜�m��Œ�
                    isScale = false;           //�摜���]��false
                    run = 20;
                    jump = 0;
                }
            }
        }

        //�ؔ��𗣂�
        if (Input.GetKeyDown(KeyCode.N) & isN)
        {
            woodrb.isKinematic = false;
            isWoodenbox = false;
            woodenbox.gameObject.transform.parent = null;    //�e���痣���

            GetComponent<Animator>().enabled = true;
            isScale = true;
            run = 50;
            jump = 1800;
        }
        */


    }
    void FixedUpdate()
    {
        //�d�v�I
        //���͑��삪���̃v���C���[�L�����ɉe������Ȃ��悤����
        if (!photonView.IsMine) return;

        if (isOperation) return;
        ////run����
        float x = Input.GetAxisRaw("Horizontal");     //x=-1(��)�E�P(�E)
        rb.AddForce(Vector2.right * x * defaultrun);

        anim.SetFloat("runSpeed", Mathf.Abs(x * defaultrun));//��Βl�ɂ���čׂ��Ȑ��l��K�v�Ƃ��Ȃ��ėǂ�

        if (isScale)
        {
            //�摜���]
            if (x > 0)
            {
                tr.localScale = new Vector3(-1, 1, 1);
            }
            else if (x < 0)
            {
                tr.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //���X�|�[���ʒu�ύX
        if (collision.gameObject.tag == "RespawnFlag")
        {
            respawmFlagObj = collision.gameObject;                  //�擾�����I�u�W�F�N�g��ۑ�
            respawmFlagObj.GetComponent<RespawmFlag>().Changerespawn1();
        }
        //�ϐ��ύX
        if (collision.gameObject.name == "final RespawnFlag")
        {
            respawmFlagObj = collision.gameObject;                  //�擾�����I�u�W�F�N�g��ۑ�
            respawmFlagObj.GetComponent<RespawmFlag>().Changefloat1();
        }
        //�G�\��
        if (collision.gameObject.name == "enemyflg wall")
        {
            GameObject Enemy = GameObject.Find("enemy Canvas").transform.Find("Enemy dog").gameObject;  //�\��OFF���ꂽ�I�u�W�F�N�g���擾
            Enemy enemyScript = Enemy.GetComponent<Enemy>();                                            //�G�I�u�W�F�N�g�̃X�N���v�g
            Enemy.SetActive(true);
            enemyScript.player = 2;
        }
        //�G�ڐG
        if (collision.gameObject.name == "Enemy ai"|| collision.gameObject.name== "Enemy dog")
        {
            Respawn();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        /*//�ؔ�(�I�u�W�F�N�g�̍��E���̂�)
        if (collision = colLR)
        {
            isWoodenbox = true;
        }*/

        //�e���|�[�g(�ʁX��)
        if (collision.gameObject.name == "teleport")
        {
            _myAvatarFlg = false;
            this.gameObject.SetActive(false);
            SceneManager.LoadScene("Stage2");
        }
        if (collision.gameObject.tag == "Return")
        {
            Respawn();
        }
        //����
        if (collision.gameObject.name == "Clay")
        {
            //�����ɕω�&���o
            particle.Play();                                        //�p�[�e�B�N���Đ�
            AIParticle = GameObject.Find("AI_Stage2(Clone)").GetComponentInChildren<ParticleSystem>();
            AIParticle.Play();                                      //AI�̃p�[�e�B�N���Đ�

            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.window = 1;
            GameObject ClayObj = collision.gameObject;
            ClayObj.GetComponent<Animator>().enabled = true;        //�l�����̃A�j���[�V����
        }
        //����
        if (collision.gameObject.name == "Paints")
        {
            //�����ɕω�&���o
            particle.Play();                                        //�p�[�e�B�N���Đ�
            AIParticle = GameObject.Find("AI_Stage2(Clone)").GetComponentInChildren<ParticleSystem>();
            AIParticle.Play();                                      //AI�̃p�[�e�B�N���Đ�

            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.window = 2;
            GameObject PaintsObj = collision.gameObject;
            PaintsObj.GetComponent<Animator>().enabled = true;      //�l�����̃A�j���[�V����
        }
       
        //short story
        if (collision.gameObject.name == "storyflg wall")
        {
            isOperation = true;                                      //���E����s��
        }
        //allstage�N���A�I�N���W�b�g�V�[����
        if (collision.gameObject.name == "finishflg wall")
        {
            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.FinishLoad();
        }
        //�ÈłŌ��郉�C�gON
        if (collision.gameObject.name == "lightflg wall")
        {
            pointlightObj.gameObject.SetActive(true);
        }
        //���C�gOFF
        if (collision.gameObject.name == "lightOffflg wall")
        {
            pointlightObj.gameObject.SetActive(false);
        }
    }
    public void StageGo()                                            //[Stage2Ctrl]����Ă΂��
    {
        isOperation = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Box")
        {
            //isN = true;
            //woodenbox = collision.gameObject;       //�擾�����I�u�W�F�N�g��ۑ�

            isCrush = rb.IsTouching(filter2d_up);     //�ؔ��Ƃ̐ڐG������擾

            if (isCrush)
            {
                Respawn();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ////���X�|�[��
        if (collision.gameObject.tag == "Return")
        {
            Respawn();
        }
        //����
        if (collision.gameObject.name == "invisible wall")
        {
            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.window = 3;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        /*if (collision.gameObject.tag == "Box")
        {
            isWoodenbox = false;
            isN = false;
        }*/
    }

    void Respawn()
    {
        Vector2 pos;
        pos = this.transform.position;            // Position�擾
        pos.x = respawnPosx;                      //�����ʒu�ɖ߂�
        pos.y = respawnPosy;
        this.transform.position = pos;            // Position�擾
    }
    public void SetMyAvatar()                     //[Stage1Ctrl]����Ă΂��
    {
        _myAvatarFlg = true;
    }
}