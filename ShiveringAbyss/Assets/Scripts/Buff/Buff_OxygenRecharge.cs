using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_OxygenRecharge : Buff
{
     float addOxygenAmount = 5;
    public override void Launch()
    {
        PlayerManager.Instance.AddOxygen(addOxygenAmount);//加氧气
        print("补充氧气");
        OnStateFinished += State_OxygenRecharge_OnStateFinished;//注册一个事件
    }

    private void State_OxygenRecharge_OnStateFinished()
    {
        //恢复
        Destroy(this);//buff结束移除组件，不是删除GameObject
    }
}

/*使用方法
     void OnBuff_addOxygen(InputValue val)
    {
        print("氧气量为" + PlayerManager.Instance.getOxygenAmount());
        if (!GetComponent<Buff_OxygenRecharge>())
        {
            gameObject.AddComponent<Buff_OxygenRecharge>().SetUp(GetComponent<PlayerMovement>(), 3f).Launch();

        }
    }
 */