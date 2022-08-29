using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{//buff的基类
    public delegate void FinishState();//buff状态结束时的委托声明

    public event FinishState OnStateFinished;//buff结束时调用事件的声明

    public float DurationTime { get; set; }//buff持续时间

    public PlayerMovement TargetUnit { get; set; }//buff目标是playerManager

    public float CurrentDurationTimer { get; set; } = 0;//计时变量



    //<summary>
    //初始化此状态
    //</summary>
    //<param name="targetUnit">此状态要作用到的单位</param>
    //<param name="durationTime">此状态的持续时间</param>
    public Buff SetUp(PlayerMovement targetUnit,float durationTime)
    {
        this.TargetUnit = targetUnit;
        this.DurationTime = durationTime;
        return this;//返回自身是想用链式的写法，在一行写完所有的功能
    }

    private void Update()
    {
        CurrentDurationTimer += Time.deltaTime;//计时器
        if(CurrentDurationTimer >= DurationTime)
        {
            OnStateFinished?.Invoke();//计时结束invoke执行buff结束
        }
    }

    public virtual void Launch()//真正执行buff的函数，在子类中重写，所以是虚类
    {

    }
}
