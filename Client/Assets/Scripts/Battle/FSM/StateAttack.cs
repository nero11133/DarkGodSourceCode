/****************************************************
    文件：StateAttack.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/22 15:50:33
	功能：攻击状态
*****************************************************/

using UnityEngine;

public class StateAttack : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentState = AniState.Attack;
        entity.skillCfg = ResSvc.Instance.GetSkillCfg((int)args[0]);
        PECommon.Log("Enter StateAttack");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        entity.ExitCurrentSkill();
    }

    public void Process(EntityBase entity, params object[] args)
    {
        //技能特效
        //技能伤害
        if (entity.entityType == EntityType.Player)
        {
            entity.canRlsSkill = false;
        }
        entity.SkillAttack((int)args[0]);
        
    }
}