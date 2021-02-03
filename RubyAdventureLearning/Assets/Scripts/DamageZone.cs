using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        //获取RubyController脚本，注意脚本是挂在角色上的，也就是这里的collision。
        RubyController rubyController = collision.GetComponent<RubyController>();
        Debug.Log(rubyController);
        //对rubyController获取进行判空处理，让代码更严谨
        if (rubyController != null)
        {
            //调用血量改变参数
            rubyController.ChangeHP(-1);
            Debug.Log(rubyController.CurrentHP);
        }
    }
}
