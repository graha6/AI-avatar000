using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerCtrl_Dog : MonoBehaviourPun   //変更
{
    public float defaultrun = 50f, defaultjump = 1900f;                 //プレイヤー移動

    Rigidbody2D rb;
    Animator anim;
    //SpriteRenderer spr;                     //Flip(左右反転)
    Transform tr;
    public Sprite fixedspr;                 //木箱操作時の画像固定
    public float respawnPosx, respawnPosy;  //リスタート座標
    ParticleSystem particle;
    public GameObject pointlightObj;
    private bool isGround,                  //地面に接触しているか
                                            isOperation,               //プレイヤー操作可能か
                                                                       //isWoodenbox, isN,          //woodenbox(木箱)操作可能か,不可能か
                 isScale,                   //画像反転するか
                 isCrush;                   //潰されているか

    [SerializeField]
    ContactFilter2D filter2d_down, //playerの下方(jump判定)範囲
                                     filter2d_up;  //playerの上方(潰され判定=isCrush)範囲

    //private GameObject woodenbox = null;            //OnCollisionEnter2Dで取得するwoodenboxオブジェクト
    //Rigidbody2D woodrb;                             //woodenbox(木箱)のコンポ―ネント
    //public BoxCollider2D colLR;                     //woodenboxオブジェクトの左右一部のcollider

    [SerializeField] GameObject _myCamera = default;
    bool _myAvatarFlg = false;

    GameObject respawmFlagObj = null;         //リスポーン座標変更するオブジェクト

    ParticleSystem AIParticle = null;        //AIのParticleSystem

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //spr = GetComponent<SpriteRenderer>();
        tr = GetComponent<Transform>();
        particle = GetComponentInChildren<ParticleSystem>();    //子オブジェクトから取得
        pointlightObj.gameObject.SetActive(false);
        isGround = false;
        isOperation = false;
        //isWoodenbox = false;
        //isN = false;
        isScale = true;                     //isWoodenbox中以外は常時
        isCrush = false;

        if (_myAvatarFlg == false)          //アバターが抜けたらカメラ消滅
        {
            Destroy(_myCamera);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //重要！
        //入力操作が他のプレイヤーキャラに影響されないようする
        if (!photonView.IsMine) return;

        //地面との接触判定を取得
        isGround = rb.IsTouching(filter2d_down);

        ////jump操作
        if (Input.GetKeyDown("space") & isGround)
        {
            rb.AddForce(Vector2.up * defaultjump);
        }

        //地面接触判定時に処理
        if (isGround)
        {
            anim.SetBool("isJump", false);
            anim.SetBool("isFall", false);
        }

        //Playerの物理速度を検出
        float velX = rb.velocity.x;
        float velY = rb.velocity.y;

        //ジャンプ時の移動速度ベクトル制限処理
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

        //物理速度からanimの判定処理
        if (velY > 0.5f)
        {
            anim.SetBool("isJump", true);
        }
        if (velY < -0.5f)
        {
            anim.SetBool("isFall", true);
        }

        /*
        //木箱操作による木箱状態変更と影響処理
        //木箱を掴み(親子になり)引く(押すができてない)
        if (Input.GetKeyDown(KeyCode.M) & isWoodenbox)
        {
            if (woodenbox != null)      //「woodenboxオブジェクト範囲外ではない時」
            {
                if (woodenbox.gameObject.tag == "Box")
                {
                    woodrb = woodenbox.GetComponent<Rigidbody2D>();
                    woodrb.isKinematic = true;
                    woodenbox.gameObject.transform.parent = this.gameObject.transform;  //子になるオブジェクト=親オブジェクト

                    GetComponent<Animator>().enabled = false;
                    spr.sprite = fixedspr;      //画像確定固定
                    isScale = false;           //画像反転をfalse
                    run = 20;
                    jump = 0;
                }
            }
        }

        //木箱を離す
        if (Input.GetKeyDown(KeyCode.N) & isN)
        {
            woodrb.isKinematic = false;
            isWoodenbox = false;
            woodenbox.gameObject.transform.parent = null;    //親から離れる

            GetComponent<Animator>().enabled = true;
            isScale = true;
            run = 50;
            jump = 1800;
        }
        */


    }
    void FixedUpdate()
    {
        //重要！
        //入力操作が他のプレイヤーキャラに影響されないようする
        if (!photonView.IsMine) return;

        if (isOperation) return;
        ////run操作
        float x = Input.GetAxisRaw("Horizontal");     //x=-1(左)・１(右)
        rb.AddForce(Vector2.right * x * defaultrun);

        anim.SetFloat("runSpeed", Mathf.Abs(x * defaultrun));//絶対値によって細かな数値を必要としなくて良い

        if (isScale)
        {
            //画像反転
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
        //リスポーン位置変更
        if (collision.gameObject.tag == "RespawnFlag")
        {
            respawmFlagObj = collision.gameObject;                  //取得したオブジェクトを保存
            respawmFlagObj.GetComponent<RespawmFlag>().Changerespawn1();
        }
        //変数変更
        if (collision.gameObject.name == "final RespawnFlag")
        {
            respawmFlagObj = collision.gameObject;                  //取得したオブジェクトを保存
            respawmFlagObj.GetComponent<RespawmFlag>().Changefloat1();
        }
        //敵表示
        if (collision.gameObject.name == "enemyflg wall")
        {
            GameObject Enemy = GameObject.Find("enemy Canvas").transform.Find("Enemy dog").gameObject;  //表示OFFされたオブジェクトを取得
            Enemy enemyScript = Enemy.GetComponent<Enemy>();                                            //敵オブジェクトのスクリプト
            Enemy.SetActive(true);
            enemyScript.player = 2;
        }
        //敵接触
        if (collision.gameObject.name == "Enemy ai"|| collision.gameObject.name== "Enemy dog")
        {
            Respawn();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        /*//木箱(オブジェクトの左右側のみ)
        if (collision = colLR)
        {
            isWoodenbox = true;
        }*/

        //テレポート(別々で)
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
        //実績
        if (collision.gameObject.name == "Clay")
        {
            //同時に変化&演出
            particle.Play();                                        //パーティクル再生
            AIParticle = GameObject.Find("AI_Stage2(Clone)").GetComponentInChildren<ParticleSystem>();
            AIParticle.Play();                                      //AIのパーティクル再生

            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.window = 1;
            GameObject ClayObj = collision.gameObject;
            ClayObj.GetComponent<Animator>().enabled = true;        //獲得時のアニメーション
        }
        //実績
        if (collision.gameObject.name == "Paints")
        {
            //同時に変化&演出
            particle.Play();                                        //パーティクル再生
            AIParticle = GameObject.Find("AI_Stage2(Clone)").GetComponentInChildren<ParticleSystem>();
            AIParticle.Play();                                      //AIのパーティクル再生

            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.window = 2;
            GameObject PaintsObj = collision.gameObject;
            PaintsObj.GetComponent<Animator>().enabled = true;      //獲得時のアニメーション
        }
       
        //short story
        if (collision.gameObject.name == "storyflg wall")
        {
            isOperation = true;                                      //左右操作不可
        }
        //allstageクリア！クレジットシーンへ
        if (collision.gameObject.name == "finishflg wall")
        {
            Stage2Ctrl script = GameObject.Find("Stage2Ctrl").GetComponent<Stage2Ctrl>();
            script.FinishLoad();
        }
        //暗闇で光るライトON
        if (collision.gameObject.name == "lightflg wall")
        {
            pointlightObj.gameObject.SetActive(true);
        }
        //ライトOFF
        if (collision.gameObject.name == "lightOffflg wall")
        {
            pointlightObj.gameObject.SetActive(false);
        }
    }
    public void StageGo()                                            //[Stage2Ctrl]から呼ばれる
    {
        isOperation = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Box")
        {
            //isN = true;
            //woodenbox = collision.gameObject;       //取得したオブジェクトを保存

            isCrush = rb.IsTouching(filter2d_up);     //木箱との接触判定を取得

            if (isCrush)
            {
                Respawn();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ////リスポーン
        if (collision.gameObject.tag == "Return")
        {
            Respawn();
        }
        //実績
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
        pos = this.transform.position;            // Position取得
        pos.x = respawnPosx;                      //初期位置に戻る
        pos.y = respawnPosy;
        this.transform.position = pos;            // Position取得
    }
    public void SetMyAvatar()                     //[Stage1Ctrl]から呼ばれる
    {
        _myAvatarFlg = true;
    }
}