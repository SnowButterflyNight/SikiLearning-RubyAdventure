using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f;
    private Rigidbody2D rigidbody2d;
    public bool isVertical;//是否垂直
    private int direction = 1;//方向（水平or垂直）
    public float changeTime = 3f;//方向改变的间隔
    private float changeTimer;
    private Animator animator;//动画播放器

    public bool isBroken;//机器人是不是坏的

    public ParticleSystem smokeEffect;//烟雾特效
    private AudioSource audioSource;
    public AudioClip fixSound;//修复音效

    public AudioClip[] HitSound;//受击音效数组

    public GameObject hitEffect;//受击特效

    //public bool isBoss;//是不是Boss
    //public GameObject bulletPrefab;
    //public float force = 300;
    //public RubyController rubyController;
    //private Vector2 lookDirection = new Vector2(1, 0);

    // Start is called before the first frame update
    void Start()
    {
        //rubyController = GetComponent<RubyController>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        changeTimer = changeTime;
        animator = GetComponent<Animator>();
        //设置条件变量的格式为：控制器.设置方法(“条件变量名”，类里对应的变量名)
        //animator.SetFloat("Direction", direction);//设置方向
        //animator.SetBool("Vertical",isVertical);//设置水平还是垂直
        MoveAnimation();
        isBroken = true;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isBroken)
        {
            //修好的时候就不移动了
            return;
        }

        changeTimer -= Time.deltaTime;
        if(changeTimer <= 0)
        {
            direction = -direction;
            //给动画控制器的条件变量更改数值
           //animator.SetFloat("Direction", direction);
            MoveAnimation();
            changeTimer = changeTime;
        }
        Vector2 position = rigidbody2d.position;
        if(isVertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
        }
        rigidbody2d.MovePosition(position);

        //if(isBoss)
        //{
        //    BossShoot();
        //}
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //注意collision如何调用getcomponent方法
        RubyController rubyController = collision.gameObject.GetComponent<RubyController>();
        if(rubyController != null)
        {
            rubyController.ChangeHP(-1);
        }
    }
    //控制移动动画方向的方法
    private void MoveAnimation()
    {
        if (isVertical)
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", direction);
        }
        else
        {
            animator.SetFloat("MoveX", direction);
            animator.SetFloat("MoveY", 0);
        }
    }

    //机器人修复方法
    public void FixRobot()
    {
        Instantiate(hitEffect, transform.position, Quaternion.identity);//实例化受击特效
        isBroken = false;
        rigidbody2d.simulated = false;//不再与任何物体发生交互
        animator.SetTrigger("Repaired");//播放修复动画
        smokeEffect.Stop();//停止雾效
        audioSource.Stop();//停止行走音效的播放
        audioSource.volume = 0.5f;//设置音量大小
        int randomAudio = Random.Range(0, 2);
        audioSource.PlayOneShot(HitSound[randomAudio]);
        Invoke("PlayFixedSound", 1f);//延时播放修复音效
        HPBar.instance.fixedNum++;//修理数加一
    }
    //播放修复音效
    private void PlayFixedSound()
    {
        audioSource.PlayOneShot(fixSound);
    }
    //敌人BOSS发射子弹方法
    //private void BossShoot()
    //{
    //    if (!isBroken)
    //    {
    //        return;
    //    }
    //    GameObject bulletObject = Instantiate(bulletPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);//实例化
    //    EnemyBullet bullet = bulletObject.GetComponent<EnemyBullet>();//获取敌人子弹脚本
    //    bullet.Shoot(lookDirection, force);//调用子弹的发射方法
    //}
}
