using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_OxygenRecharge : Buff
{
     float addOxygenAmount = 5;
    public override void Launch()
    {
        PlayerManager.Instance.AddOxygen(addOxygenAmount);//������
        print("��������");
        OnStateFinished += State_OxygenRecharge_OnStateFinished;//ע��һ���¼�
    }

    private void State_OxygenRecharge_OnStateFinished()
    {
        //�ָ�
        Destroy(this);//buff�����Ƴ����������ɾ��GameObject
    }
}

/*ʹ�÷���
     void OnBuff_addOxygen(InputValue val)
    {
        print("������Ϊ" + PlayerManager.Instance.getOxygenAmount());
        if (!GetComponent<Buff_OxygenRecharge>())
        {
            gameObject.AddComponent<Buff_OxygenRecharge>().SetUp(GetComponent<PlayerMovement>(), 3f).Launch();

        }
    }
 */