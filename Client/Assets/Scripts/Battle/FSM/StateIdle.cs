/****************************************************
	文件：StateIdle.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/17 19:57   	
	功能：待机状态
*****************************************************/
using UnityEngine;

public class StateIdle : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentState = AniState.Idle;
        entity.SetDir(Vector2.zero);
        PECommon.Log("Enter StateIdle");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        PECommon.Log("Exit StateIdle");
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if (entity.nextSkillID != 0)
        {
            entity.Attack(entity.nextSkillID);
        }
        else
        {
            if (entity.entityType == EntityType.Player)
            {
                entity.canRlsSkill = true;
            }
            if (entity.GetDir() != Vector2.zero)
            {
                entity.Move();
                entity.SetDir(entity.GetDir());
            }
            else
            {
                entity.SetBlend(Constants.BlendIdle);
            }
        }
        
        PECommon.Log("Process StateIdle");
        
    }
}

