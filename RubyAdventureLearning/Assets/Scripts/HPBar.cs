using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image hpMask;//血条遮罩图片
    private float originalSize;//遮罩初始大小

    //单例，需要加上static关键字，全局使用。
    public static HPBar instance { get; private set; }

    //之所以在HPBar里定义，是因为这个类是一个单例，且该变量要在多个类里用。
    //正常情况下应该是有一个GameManager类，把它做成单例，在里面定义这些游戏事件逻辑。
    public bool hasTask;//是否有任务。
    public int fixedNum;//修理好的机器人数量。

    private void Awake()
    {
        //单例需要在Awake里实例化出来才能用。
        //this关键字代表类本身。
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        originalSize = hpMask.rectTransform.rect.width;//给遮罩初始大小赋初值
    }
    //血条改变方法
    public void ChangeHp(float fillPercentage)
    {
        //调用rectTransform方法中根据锚点改变UI大小的方法
        hpMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * fillPercentage);
    }
}
