using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_MoveSlow : Buff
{
    float originSpeed;
    float minusSpeed = 2;

    public override void Launch()
    {
        print("����buff");
        originSpeed = TargetUnit.getMovementSpeed();//����ԭ�ٶ�
        TargetUnit.setMovementSpeed(0);//������0
        OnStateFinished += Buff_MoveSlow_OnStateFinished;
    }

    private void Buff_MoveSlow_OnStateFinished()
    {
        print("����buffʧЧ");
        TargetUnit.setMovementSpeed(originSpeed);//�ָ��ٶ�
        Destroy(this);
    }
}
/*ʹ�÷���
     void OnBuff_MoveQuick(InputValue val)
    {
        
        if (!GetComponent<Buff_MoveQuick>())
        {
            gameObject.AddComponent<Buff_MoveQuick>().SetUp(GetComponent<PlayerMovement>(), 3f).Launch();

        }
    }
*/