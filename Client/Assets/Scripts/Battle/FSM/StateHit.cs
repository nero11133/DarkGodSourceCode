/****************************************************
    文件：StateHit.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/30 16:53:11
	功能：受击状态
*****************************************************/

using UnityEngine;

public class StateHit : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentState = AniState.Hit;
        entity.RmvSkillCb();
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        if (entity.entityType == EntityType.Player)
        {
            entity.canControl = true;
        }
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if (entity.entityType == EntityType.Player)
        {
            entity.canRlsSkill = false;
            entity.canControl = false;
        }
        //受击音效
        if (entity.entityType == EntityType.Player)
        {
            AudioSource audioSource = entity.GetAudio();
            AudioSvc.Instance.PlayCharAudio(Constants.AssassinHit, audioSource);
        }
        entity.SetAction(Constants.ActionHit);
        entity.SetDir(Vector2.zero);
        int bornCbId= TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            entity.SetAction(Constants.ActionDefault);
            entity.Idle();
            entity.RmvBornCbId(tid);
        }, GetHitAniTime(entity));
        entity.bornCbLst.Add(bornCbId);
    }

    private float GetHitAniTime(EntityBase entity)
    {
        AnimationClip[] clips = entity.GetAnimationClips();
        for (int i = 0; i < clips.Length; i++)
        {
            string clipName = clips[i].name;
            if(clipName.Contains("hit")|| clipName.Contains("Hit")|| clipName.Contains("HIT"))
            {
                return clips[i].length;
            }
        }
        //保护值
        return 1;
    }
}