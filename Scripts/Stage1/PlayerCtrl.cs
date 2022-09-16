using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviourPun   //�ύX
{
    public float defaultrun = 50f, defaultjump = 1900f;                 //�v���C���[�ړ�

    Rigidbody2D rb;
    Animator anim;
                                            //SpriteRenderer spr;                     //sprite(�摜�Œ�
    Transform tr;
    public Sprite fixedspr;                 //�ؔ����쎞�̉摜�Œ�
    public float respawnPosx, respawnPosy;  //���X�^�[�g���W
    ParticleSystem particle;

    private bool isGround,                  //�n�ʂɐڐG���Ă��邩
                 isOperation,               //�v���C���[����\��
                 isLever,                   //���o�[����\��
                                            //isWoodenbox, isN,          //woodenbox(�ؔ�)����\��,�s�\��
                 isScale,                   //�摜���]���邩
                 isCrush;                   //�ׂ���Ă��邩


    private GameObject hitLever = null;       //OnTriggerEnter2D�Ŏ擾����Lever�I�u�W�F�N�g
                                              //private GameObject woodenbox = null;    //OnCollisionEnter2D�Ŏ擾����woodenbox�I�u�W�F�N�g


    [SerializeField] ContactFilter2D filter2d_down, //player�̉���(jump����)�͈�
                                     filter2d_up;   //player�̏��(�ׂ��ꔻ��=isCrush)�͈�

                                                    //Rigidbody2D woodrb;                             //woodenbox(�ؔ�)�̃R���|�\�l���g
                                                    //public BoxCollider2D colLR;                     //woodenbox�I�u�W�F�N�g�̍��E�ꕔ��collider

    [SerializeField] GameObject _myCamera = default;
    bool _myAvatarFlg = false;

    GameObject respawmFlagObj = null;         //���X�|�[�����W�ύX����I�u�W�F�N�g

    ParticleSystem DogParticle = null;        //Dog��ParticleSystem



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //spr = GetComponent<SpriteRenderer>();
        tr = GetComponent<Transform>();
        particle = GetComponentInChildren<ParticleSystem>();    //�q�I�u�W�F�N�g����擾


        isGround = false;
        isOperation = false;
        isLever = false;
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
        if (!photonView.IsMine)return;
            
        
        //�n�ʂƂ̐ڐG������擾
        isGround = rb.IsTouching(filter2d_down);

        ////jump����
        if (Input.GetKeyDown("space") & isGround)
        {
            if (Input.GetKey(KeyCode.Z))        //��W�����v
            {
                rb.AddForce(Vector2.up * defaultjump * 1.35f);
            }
            else
            {
                rb.AddForce(Vector2.up * defaultjump);
            }
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

        ////Lever����
        if (Input.GetKeyDown(KeyCode.V) & isLever)
        {
            if (hitLever != null)                           //�uLever�I�u�W�F�N�g�͈͊O�ł͂Ȃ����v
            {
                hitLever.GetComponent<CircleCollider2D>().enabled = false;
                hitLever.GetComponent<Lever>().Change();    //�������̏����ō��ւ�
                hitLever.GetComponent<Lever>().Action();
            }
        }

        /*
        //�ؔ�����ɂ��ؔ���ԕύX�Ɖe������
        //�ؔ���͂�(�e�q�ɂȂ�)��������
        if (Input.GetKeyDown(KeyCode.M) & isWoodenbox)
        {

            if (woodenbox != null)              //�uwoodenbox�I�u�W�F�N�g�͈͊O�ł͂Ȃ����v
            {
                if (woodenbox.gameObject.tag == "Box")
                {
                    woodrb = woodenbox.GetComponent<Rigidbody2D>();
                    woodrb.isKinematic = true;
                    woodenbox.gameObject.transform.parent = this.gameObject.transform;  //�q�ɂȂ�I�u�W�F�N�g=�e�I�u�W�F�N�g

                    GetComponent<Animator>().enabled = false;
                    spr.sprite = fixedspr;      //�摜�m��Œ�
                    isScale = false;            //�摜���]��false
                    run = 20;
                    jump = 0;
                }
            }
        }

        //�ؔ��𗣂�  //�����O�Ɉ�x�͂ޑ����woodenbox�����o����K�v������G���[���o�邩��I
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
        //���o�[
        if (collision.gameObject.tag == "Lever")
        {
            isLever = true;
            hitLever = collision.gameObject;                        //�擾�����I�u�W�F�N�g��ۑ�
        }
        //���X�|�[���ʒu�ύX
        if (collision.gameObject.tag == "RespawnFlag")
        {
            respawmFlagObj = collision.gameObject;                  //�擾�����I�u�W�F�N�g��ۑ�
            respawmFlagObj.GetComponent<RespawmFlag>().Changerespawn();
        }
        //�ϐ��ύX
        if (collision.gameObject.name == "final RespawnFlag")
        {
            isOperation = false;
            respawmFlagObj = collision.gameObject;                  //�擾�����I�u�W�F�N�g��ۑ�
            respawmFlagObj.GetComponent<RespawmFlag>().Changefloat();
        }
        //�G�\��
        if (collision.gameObject.name == "enemyflg wall")
        {
            GameObject Enemy=GameObject.Find("enemy Canvas").transform.Find("Enemy ai").gameObject;  //�\��OFF���ꂽ�I�u�W�F�N�g���擾
            Enemy enemyScript = Enemy.GetComponent<Enemy>();                                         //�G�I�u�W�F�N�g�̃X�N���v�g
            Enemy.SetActive(true);
            enemyScript.player = 1;
        }
        //�G�ڐG
        if (collision.gameObject.name == "Enemy ai" || collision.gameObject.name == "Enemy dog")
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
        //���X�|�[��
        if (collision.gameObject.tag == "Return")
        {
            Respawn();
        }
        //����
        if (collision.gameObject.name == "Clay")
        {
            //�����ɕω�&���o
            particle.Play();                                        //�p�[�e�B�N���Đ�
            DogParticle = GameObject.Find("Dog_Stage2(Clone)").GetComponentInChildren<ParticleSystem>();
            DogParticle.Play();                                     //Dog�̃p�[�e�B�N���Đ�

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
            DogParticle = GameObject.Find("Dog_Stage2(Clone)").GetComponentInChildren<ParticleSystem>();
            DogParticle.Play();                                     //Dog�̃p�[�e�B�N���Đ�

            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.window = 2;

            GameObject PaintsObj = collision.gameObject;
            PaintsObj.GetComponent<Animator>().enabled = true;      //�l�����̃A�j���[�V����
        }
        //short story
        if (collision.gameObject.name == "storyflg wall")
        {
            isOperation = true;                                      //���E����s��
            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.ShortStory();
        }
        //allstage�N���A�I�N���W�b�g�V�[����
        if (collision.gameObject.name == "finishflg wall")
        {
            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.FinishLoad();
        }
    }
    public void StageGo()                                            //[Stage2Ctrl]����Ă΂��
    {
        isOperation = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Lever")
        {
            isLever = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Box")
        {
            //isN = true;
            //woodenbox = collision.gameObject;         //�擾�����I�u�W�F�N�g��ۑ�

            isCrush = rb.IsTouching(filter2d_up);     //�ؔ��Ƃ̐ڐG������擾

            //���X�|�[��
            if (isCrush)
            {
                Respawn();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //stage�ړ�
        if (collision.gameObject.name == "stageflg wall")
        {
            isOperation = true;                                      //���E����s��
            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.Stage();
        }
        //���X�|�[��
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

    public void Respawn()
    {
        Vector2 pos;
        pos = this.transform.position;            // Position�擾
        pos.x = respawnPosx;                      //�����ʒu�ɖ߂�
        pos.y = respawnPosy;
        this.transform.position = pos;            // Position�擾
    }
    public void SetMyAvatar()                     //[Stage1Ctrl],[Stage2Ctrl]����Ă΂��
    {
        _myAvatarFlg = true;
    }
}