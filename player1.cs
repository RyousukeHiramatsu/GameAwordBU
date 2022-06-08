using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//=============================================================================
//      開発履歴
//--------------------------------------------------------------------
// 2022/ 5/4  北川湧斗　ボールの生成
// 2022/ 5/6  北川湧斗　プレイヤーからの操作権限の受け取り
// 2022/ 5/7  北川湧斗　ボールの消去 ＆ プレイヤーへの操作権限の受け渡し
// 2022/ 5/27 加古洋啓　エフェクトの追加
//=============================================================================
public class player1 : MonoBehaviour
{
    // ------変数宣言
    private bool isFront;       //
    private bool isBack;        //
    private bool isLeft;        //
    private bool isRight;       //
    private bool isUp;          //
    private bool isDown;        //
    private bool isSetfree;     //
    private Vector3 AroundPos;  //

    public float moveSpd;
    public float StartTimer = 2.0f;

    private new Rigidbody rigidbody;
    private new SphereCollider sphere;
    private CameraZoom cz;

    public CountDown cd;
    public GameObject player;
    public GameObject obj;
    public GameObject start;
    public Animator animator;

    public bool GamePlay = true;

    private bool numberMagic = false;

    //音
    public AudioClip soundMagic;
    AudioSource audioSource;

    //エフェクト
    public GameObject MagicEffect;

    [SerializeField] CinemachineVirtualCamera camera;

    // [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody>();
        sphere = player.GetComponent<SphereCollider>();
        cz = GameObject.Find("CM vcam1").GetComponent<CameraZoom>();
        //cd = GetComponent<CountDown>();
        animator = GetComponentInChildren<Animator>();
        camera.Follow = player.transform;
        start = GameObject.Find("GameStart");
    }

    // Update is called once per frame
    void Update()
    {

        if (0 < StartTimer)
        {
            start.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            StartTimer -= Time.deltaTime;
        }
        if (StartTimer < 0)
        {
            Destroy(start);
        float hori = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");
            float lsh = Input.GetAxis("L_Stick_H");
            float lsv = Input.GetAxis("L_Stick_V");
            float rsh = Input.GetAxis("R_Stick_H");
            float rsv = Input.GetAxis("R_Stick_V");
            float dph = Input.GetAxis("D_Pad_H");
            float dpv = Input.GetAxis("D_Pad_V");
            float tri = Input.GetAxis("L_R_Trigger");

            float fStickVertical = Input.GetAxis("Vertical");
            float fStickHorizontal = Input.GetAxis("Horizontal");

            //---------------------
            // '''''ログ表示'''''
            //---------------------
            if (Input.GetKeyDown("joystick button 0"))
            {
                Debug.Log("button0");
            }   //Bボタン
            if (Input.GetKeyDown("joystick button 1"))
            {
                Debug.Log("button1");
            }   //Aボタン
            if (Input.GetKeyDown("joystick button 2"))
            {
                Debug.Log("button2");
            }   //Xボタン
            if (Input.GetKeyDown("joystick button 3"))
            {
                Debug.Log("button3");
            }   //Yボタン
            if (Input.GetKeyDown("joystick button 4"))
            {
                Debug.Log("button4");
            }   //LBボタン
            if (Input.GetKeyDown("joystick button 5"))
            {
                Debug.Log("button5");
            }   //RBボタン
            if (Input.GetKeyDown("joystick button 6"))
            {
                Debug.Log("button6");
            }   //中心左ボタン
            if (Input.GetKeyDown("joystick button 7"))
            {
                Debug.Log("button7");
            }   //
            if (Input.GetKeyDown("joystick button 8"))
            {
                Debug.Log("button8");
            }   //Lスティック押し込み
            if (Input.GetKeyDown("joystick button 9"))
            {
                Debug.Log("button9");
            }   //Rスティック押し込み
            if ((hori != 0) || (vert != 0))
            {
                //Debug.Log("stick:" + hori.ToString("F2") + "," + vert.ToString("F2"));
            }
            if ((lsh != 0) || (lsv != 0))
            {
                // Debug.Log("L stick:" + lsh + "," + lsv);
            }        //L Stick
            if ((rsh != 0) || (rsv != 0))
            {
                //Debug.Log("R stick:" + rsh + "," + rsv);
            }        //R Stick
            if ((dph != 0) || (dpv != 0))
            {
                //Debug.Log("D Pad:" + dph + "," + dpv);
            }        //D-Pad
            if (tri > 0)
            {
                //Debug.Log("L trigger:" + tri);
            }                         //Trigger
            else if (tri < 0)
            {
                //Debug.Log("R trigger:" + tri);
            }
            else
            {
                //Debug.Log("trigger:none");
            }

            //--------------------------------------
            //'''''キャラクター移動'''''
            //--------------------------------------
            isFront = fStickVertical > 0;
            isBack = fStickVertical < 0;
            isLeft = fStickHorizontal < 0;
            isRight = fStickHorizontal > 0;

            isUp = Input.GetKey("joystick button 4");           //LBボタン...キャラクター上昇
            isDown = Input.GetKey("joystick button 5");         //RBボタン...キャラクター下降

            //--------------------------------------
            //'''''スピード制限'''''
            //--------------------------------------
            moveSpd = 20f * (Mathf.Abs(fStickVertical) + Mathf.Abs(fStickHorizontal));

            if (moveSpd > 15)          //スピード制限
            {
                moveSpd = 15;
            }
            else if (moveSpd <= 15)
            {
                moveSpd = 20f * (Mathf.Abs(fStickVertical) + Mathf.Abs(fStickHorizontal));
            }

            //--------------------------------------
            //'''''magicを射出'''''
            //--------------------------------------
            if (cd.GetIsClear() == false)
            {
                if (Input.GetKeyDown("joystick button 0") || Input.GetKeyDown(KeyCode.Space))   //Aボタン
                {
                    if (!numberMagic)
                    {
                        //アニメーション開始
                        animator.SetTrigger("attack");
                        //エフェクト追加
                        generateMagicEffect();

                        //音再生
                        audioSource.PlayOneShot(soundMagic);

                        GamePlay = false;
                        //objの生成
                        Instantiate(obj, transform.position, transform.rotation);

                        //Rigidbodyを取得
                        var rb = GetComponent<Rigidbody>();
                        //isKinematicをオンにする
                        rb.isKinematic = true;
                        //camera.LookAt = player.transform;

                        numberMagic = true;
                    }
                }
            }

            //-----クリア時に動き停止
            if (cd.GetIsClear() == true)
            {
                animator.SetTrigger("Clear");
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
            }

            if (GamePlay)
            {
                cz.CameraReset();
            }
        }
    }

    //--------------------------------------------
    //      キャラクター移動方向
    //--------------------------------------------
    private void FixedUpdate()
    {
        if (!GamePlay)
        {
            return;
        }

        Vector3 v = new Vector3(0f, 0f, 0f);

        if (isFront) v.z = moveSpd;
        if (isBack) v.z = -moveSpd;
        if (isLeft) v.x = -moveSpd;
        if (isRight) v.x = moveSpd;
        if (isUp) v.y = moveSpd;
        if (isDown) v.y = -moveSpd;
        rigidbody.velocity = v;

        animator.SetFloat("MoveSpeed", new Vector3(v.x, 0, v.z).magnitude);
        Vector3 diff = transform.position - AroundPos;
        AroundPos = transform.position;

        if (diff.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(diff);
        }

    }

    //--------------------------------------------
    //      マジックボールから呼ばれる関数
    //--------------------------------------------
    public void callMagicBall()
    {
        // Debug.Log("Call!!");
        GamePlay = true;

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        camera.Follow = player.transform;

        numberMagic = false;


        //camera.LookAt = null;
        //Debug.Log("Call!!");
    }

    public bool GetGamePlay()
    {
        return GamePlay;
    }

    private void generateMagicEffect()
    {
        GameObject effect = Instantiate(MagicEffect) as GameObject;
        effect.transform.position = player.transform.position;
    }
}
