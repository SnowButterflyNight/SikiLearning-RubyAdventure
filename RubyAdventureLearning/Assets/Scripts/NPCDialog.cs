using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialog : MonoBehaviour
{
    public GameObject dialogCanvas;//对话框游戏物体
    public float displayTime = 5.0f;//展示的时间
    private float displayTimer;//关闭对话框的计时器
    public Text dialogText;//对话框文本
    public AudioSource audioSource;
    public AudioClip completeAudioClip;//完成音效
    private bool hasPlayed;//是否已经播放过音效

    void Start()
    {
        dialogCanvas.SetActive(false);
        displayTimer = -1;//之所以用负值是因为要避免计时器一开始就满足计时条件从而倒计时，否则对话框会秒关的。
    }
    void Update()
    {
        if(displayTimer >= 0)
        {
            displayTimer -= Time.deltaTime;
            if(displayTimer < 0)
            {
                dialogCanvas.SetActive(false);
            }
        }
        
    }
    public void DisplayDialog()
    {
        displayTimer = displayTime;//只有调用显示方法的时候才给计时器附上展示时间的值
        dialogCanvas.SetActive(true);
        HPBar.instance.hasTask = true;//出现对话框后就接了任务。
        //任务完成后修改对话框内容以及播放完成音效
        if(HPBar.instance.fixedNum >= 4)
        {
            dialogText.text = "Thank you for your help!";
            if(!hasPlayed)
            {
                audioSource.PlayOneShot(completeAudioClip);
                hasPlayed = true;
            }
        }
    }
}
