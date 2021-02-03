using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    public int maxHP = 10;//最大生命值
    private int currentHP;//当前生命值
    public float speed = 4f;//移动速度

    public AudioSource audioSource;//一次性音效源
    public AudioSource walkAudioSource;//持续性循环走路音效源

    public AudioClip playerInjury;//受伤音效
    public AudioClip playerShoot;//发射音效
    public AudioClip walkSound;//走路音效

    private Vector3 rebornPosition;//重生位置

    public int CurrentHP
    {
        get { return currentHP; }
    }
    
    public float invicibleTime = 2.0f;//角色无敌时间
    private bool isInvicible;//是否无敌
    private float invicibleTimer;//计时器

    private Vector2 lookDirection = new Vector2(1, 0);//声明一个朝向的二维向量（向量的方向性）。
    private Animator animator;

    public GameObject bulletPrefab;//子弹预制体
    public float force = 300;//发射的力度

    // Start is called before the first frame update
    void Start()
    {
        //其实尖括号里的是一个泛型
        //但凡是调用方法的，必须要有小括号，传递参数视方法定义的情况而定
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        //改变游戏的帧率
        //Application.targetFrameRate = 30;
        animator = GetComponent<Animator>();
        //audioSource = GetComponent<AudioSource>();
        rebornPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal,vertical);//用一个二维向量来存储玩家的输入
        //根据玩家的输入来更改朝向，x或y不为0时就说明有移动输入
        //approximately函数是用来做近似值比较的，因为float类型的数值有误差
        if(!Mathf.Approximately(move.x,0) || !Mathf.Approximately(move.y, 0))
        {
            //赋值loodDirection也可以用 lookDirection = move;来搞定
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();//标准化
            //为防止多个音效同时播放，需要通过是否正在播放来限定播放音效的唯一性
            if (!walkAudioSource.isPlaying)
            {
                //Play()方法是没办法指定AudioClip的，所以需要用代码赋值AudioClip组件
                walkAudioSource.clip = walkSound;
                walkAudioSource.Play();
            }
        }
        else
        {
            walkAudioSource.Stop();
        }
        //接下来改变动画条件变量
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);//magnitude是取向量的模长

        Vector2 position = transform.position;
        //position.x = position.x + speed * horizontal * Time.deltaTime;
        //position.y = position.y + speed * vertical * Time.deltaTime;
        position = position + speed * move * Time.deltaTime;//horizonta和vertical都放在move向量里了，所以可以直接用

        //transform.position = position;
        rigidbody2d.MovePosition(position);

        //无敌时间
        if(isInvicible)
        {
            invicibleTimer = invicibleTimer - Time.deltaTime;
            if(invicibleTimer <= 0)
            {
                isInvicible = false;
            }
        }
        //按下H发射
        if (Input.GetKeyDown(KeyCode.J))
        {
            Shoot();
        }

        //与NPC对话的方法
        if(Input.GetKeyDown(KeyCode.K))
        {
            //方法的几个变量分别为：发射位置、发射方向、射线距离、需要检测的层级
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, 
                lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if(hit.collider != null)
            {
                //碰撞器检测成功后，就用它来获取NPC身上的对话框脚本。
                NPCDialog npcDialog = hit.collider.GetComponent<NPCDialog>();
                if(npcDialog != null)
                {
                    npcDialog.DisplayDialog();
                }
            }
        }
    }

    //改变生命值的方法
    public void ChangeHP(int addHP)
    {
        //如果受到了伤害就进入无敌，并且开始计时
        if(addHP < 0)
        {
            //当前如果已经是无敌的话就直接返回
            if(isInvicible)
            {
                return;
            }
            isInvicible = true;
            invicibleTimer = invicibleTime;
            PlayAudio(playerInjury);
            animator.SetTrigger("Hit");//受伤动画
        }
        //用Clamp函数来规定血量变化的范围
        currentHP = Mathf.Clamp(currentHP + addHP,0,maxHP);
        //调用HPbar类中的血条变化方法
        //运算过程中只要有一个变量强转为某个类型，则结果就是这个类型
        HPBar.instance.ChangeHp(currentHP/(float)maxHP);
        if(currentHP <= 0)
        {
            Reborn();
        }
    }
    //发射子弹
    private void Shoot()
    {
        if(!HPBar.instance.hasTask)
        {
            return;
        }
        GameObject bulletObject = Instantiate(bulletPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);//实例化
        Bullet bullet = bulletObject.GetComponent<Bullet>();//获取子弹脚本
        bullet.Shoot(lookDirection, force);//调用子弹的发射方法
        animator.SetTrigger("Launch");//播放发射动画
        PlayAudio(playerShoot);
    }
    //音频播放方法
    public void PlayAudio(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
    //重生方法
    private void Reborn()
    {
        ChangeHP(maxHP);
        transform.position = rebornPosition;
    }
}
