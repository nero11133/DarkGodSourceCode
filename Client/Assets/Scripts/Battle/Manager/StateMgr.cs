/****************************************************
    文件：StateMgr.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/16 19:35:59
	功能：状态管理器
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public class StateMgr : MonoBehaviour 
{
    private Dictionary<AniState, IState> fsm = new Dictionary<AniState, IState>();

    public void Init()
    {
        fsm.Add(AniState.Idle, new StateIdle());
        fsm.Add(AniState.Move, new StateMove());
        fsm.Add(AniState.Attack, new StateAttack());
        fsm.Add(AniState.Born, new StateBorn());
        fsm.Add(AniState.Die, new StateDie());
        fsm.Add(AniState.Hit, new StateHit());
        PECommon.Log("Init StateMgr");
    }

    public void ChangeState(EntityBase entity,AniState targetState, params object[] args)
    {
        if (entity.currentState == targetState)
        {
            return;
        }

        if (fsm.ContainsKey(targetState))
        {
            if (entity.currentState != AniState.None)
            {
                fsm[entity.currentState].Exit(entity, args);
               
            }
            fsm[targetState].Enter(entity, args);
            fsm[targetState].Process(entity, args);
        }
    }
}