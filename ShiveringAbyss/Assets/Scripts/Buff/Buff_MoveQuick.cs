using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Buff_MoveQuick : Buff
{
    float originSpeed;
     float addSpeed = 2;
   
    public override void Launch()
    {
        print("加速buff");
        originSpeed = TargetUnit.getMovementSpeed();//储存原速度
        TargetUnit.setMovementSpeed(originSpeed * addSpeed);//加速
        OnStateFinished += Buff_MoveQuick_OnStateFinished;
    }

    private void Buff_MoveQuick_OnStateFinished()
    {
        print("加速buff失效");
        TargetUnit.setMovementSpeed(originSpeed);//恢复速度
        Destroy(this);
    }
}
/*使用方法
     void OnBuff_MoveQuick(InputValue val)
    {
        
        if (!GetComponent<Buff_MoveQuick>())
        {
            gameObject.AddComponent<Buff_MoveQuick>().SetUp(GetComponent<PlayerMovement>(), 3f).Launch();

        }
    }
*/