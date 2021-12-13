/****************************************************
    文件：StateBorn.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/30 15:19:31
	功能：出生状态
*****************************************************/

using UnityEngine;

public class StateBorn : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentState = AniState.Born;
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetAction(Constants.ActionBorn);
        int bornCbId= TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            entity.SetAction(Constants.ActionDefault);
            entity.RmvBornCbId(tid);
        }, 0.5);
        entity.bornCbLst.Add(bornCbId);
    }
}