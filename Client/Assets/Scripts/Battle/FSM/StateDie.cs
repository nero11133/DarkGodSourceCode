/****************************************************
    文件：StateDie.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/30 15:46:2
	功能：死亡状态
*****************************************************/

using UnityEngine;

public class StateDie : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentState = AniState.Die;
        entity.RmvSkillCb();
        entity.RmvBornCb();
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetAction(Constants.ActionDie);
        if (entity.entityType == EntityType.Monster)
        {
            entity.GetCC().enabled = false;
            TimerSvc.Instance.AddTimeTask((int tid) =>
            {
                entity.SetActive(false);
            }, Constants.DieAniTime);
        }
       
    }
}