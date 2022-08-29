using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_MoveSlow : Buff
{
    float originSpeed;
    float minusSpeed = 2;

    public override void Launch()
    {
        print("减速buff");
        originSpeed = TargetUnit.getMovementSpeed();//储存原速度
        TargetUnit.setMovementSpeed(0);//减速至0
        OnStateFinished += Buff_MoveSlow_OnStateFinished;
    }

    private void Buff_MoveSlow_OnStateFinished()
    {
        print("减速buff失效");
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