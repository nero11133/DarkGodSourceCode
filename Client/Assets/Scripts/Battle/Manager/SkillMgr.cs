/****************************************************
    文件：SkillMgr.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/16 19:36:51
	功能：技能管理器
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public class SkillMgr : MonoBehaviour
{
    private ResSvc resSvc;
    private TimerSvc timerSvc;

    public void Init()
    {
        PECommon.Log("Init SkillMgr");
        resSvc = ResSvc.Instance;
        timerSvc = TimerSvc.Instance;
    }

    public void SkillAttack(EntityBase entity, int skillID)
    {
        entity.skActionCbLst.Clear();
        entity.skMoveCbLst.Clear();
        AttackEffect(entity, skillID);
        AttackDamage(entity, skillID);
    }

    /// <summary>
    /// 技能效果表现
    /// </summary>
    public void AttackEffect(EntityBase entity, int skillID)
    {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);
        if (!skillData.isCollide)
        {
            Physics.IgnoreLayerCollision(9, 10);
            timerSvc.AddTimeTask((int tid) =>
            {
                Physics.IgnoreLayerCollision(9, 10, false);
            }, skillData.skillTime, PETimeUnit.Millisecond);
        }
        if (!skillData.isBreak)
        {
            entity.entityState = EntityState.BatiState;
        }
        if (entity.entityType == EntityType.Player)
        {
            if (entity.GetDir() == Vector2.zero)
            {
                //搜索最近的怪物
                Vector2 dir = entity.CalcTargetDir();
                if (dir != Vector2.zero)
                {
                    entity.SetAtkDir(dir);
                }
            }
            else
            {
                entity.SetAtkDir(entity.GetDir(), true);
            }
        }
        
        entity.SetAction(skillData.aniAction);
        entity.SetFx(skillData.fx, skillData.skillTime);
        entity.SetDir(Vector2.zero);
        CalcSkillMove(entity, skillData);
        entity.canControl = false;

        entity.skEndCb = timerSvc.AddTimeTask((int tid) =>
         {
             entity.Idle();
         }, skillData.skillTime, PETimeUnit.Millisecond);
    }

    public void AttackDamage(EntityBase entity, int skillID)
    {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);
        List<int> actionLst = skillData.skillActionList;
        int sum = 0;
        for (int i = 0; i < actionLst.Count; i++)
        {
            int index = i;
            SkillActionCfg skillActionCfg = resSvc.GetSkillActionCfg(actionLst[i]);
            sum += skillActionCfg.delayTime;
            if (sum > 0)
            {
                int actId = timerSvc.AddTimeTask((int tid) =>
                 {
                     if (entity != null)
                     {
                         SkillAction(entity, skillData, index);
                         entity.RmvActionCbId(tid);
                     }
                    
                 }, sum, PETimeUnit.Millisecond);
                entity.skActionCbLst.Add(actId);
            }
            else
            {
                //瞬时伤害
                SkillAction(entity, skillData, index);
            }
        }
    }

    private void SkillAction(EntityBase caster, SkillCfg skillCfg,int index)
    {
        int damage = skillCfg.skillDamageList[index];
        SkillActionCfg skillActionCfg = resSvc.GetSkillActionCfg(skillCfg.skillActionList[index]);
        if (caster.entityType == EntityType.Player)
        {
            //获取场景里所有的怪物实体
            List<EntityMonster> monsterList = caster.battleMgr.GetEntityMonsters();
            
            for (int i = 0; i < monsterList.Count; i++)
            {
                EntityMonster em = monsterList[i];
                if (InRange(caster.GetPos(), em.GetPos(), skillActionCfg.radius) && InAngle(caster.GetTrans(), em.GetPos(), skillActionCfg.angle))
                {
                    //计算伤害
                    CalcDamage(caster, em, skillCfg, damage);
                }
            }
        }
        else if (caster.entityType == EntityType.Monster)
        {
            EntityPlayer target = caster.battleMgr.entitySelfPlayer;
            if (target == null)
            {
                return;
            }
            if (InRange(caster.GetPos(), target.GetPos(), skillActionCfg.radius) && InAngle(caster.GetTrans(), target.GetPos(), skillActionCfg.angle))
            {
                //计算伤害
                CalcDamage(caster, target, skillCfg, damage);
            }
        }
        

    }

    private System.Random rd = new System.Random();
    private void CalcDamage(EntityBase caster,EntityBase target,SkillCfg skillCfg,int damage)
    {
        int dmgSum = damage;
        if (skillCfg.dmgType == DamageType.AD)
        {
            //计算闪避
            int dodgeNum = PETools.GetRDInt(1, 100, rd);
            if (dodgeNum <= target.Props.dodge)
            {
                //ui显示闪避todo
                PECommon.Log("闪避rate:" + dodgeNum + "/" + target.Props.dodge);
                target.SetDodge();
                return;
            }

            //计算属性加成
            dmgSum += caster.Props.ad;

            //计算暴击
            int criticalNum = PETools.GetRDInt(1, 100, rd);
            if (criticalNum <= caster.Props.critical)
            {
                float criticalRate = 1 + (PETools.GetRDInt(1, 100, rd) / 100f);
                dmgSum = (int)(criticalRate * dmgSum);
                target.SetCritical(dmgSum);
                PECommon.Log("暴击rate:" + criticalNum + "/" + caster.Props.critical);
            }

            //计算穿甲
            int addef = (int)((1 - (caster.Props.pierce / 100f)) * target.Props.addef);
            dmgSum -= addef;
        }
        else if(skillCfg.dmgType == DamageType.AP)
        {
            //计算属性加成
            dmgSum += caster.Props.ap;

            //计算魔法抗性
            dmgSum -= target.Props.apdef;
        }
        else
        {

        }
        //最终伤害
        if (dmgSum < 0)
        {
            dmgSum = 0;
            return;
        }

        target.SetHurt(dmgSum);

        if (target.Hp <= dmgSum)
        {
            target.Hp = 0;
            //目标死亡
            target.Die();
            if (target.entityType == EntityType.Monster)
            {
                target.battleMgr.RmvMonster(target.Name);
            }
            else if (target.entityType == EntityType.Player)
            {
                target.battleMgr.EndBattle(false, 0);
                target.battleMgr.entitySelfPlayer = null;
            }
            
        }
        else
        {
            target.Hp -= dmgSum;
            if (target.entityState == EntityState.None && target.GetBreakState())
            {
                target.Hit();
            }
            
        }
    }

    private bool InRange(Vector3 from,Vector3 to,float range)
    {
        float dis = Vector3.Distance(from, to);
        if (dis <= range)
        {
            return true;
        }
        return false;
    }

    private bool InAngle(Transform trans,Vector3 to,float angle)
    {
        if (angle == 360)
        {
            return true;
        }
        else
        {
            Vector3 start = trans.forward;
            Vector3 dir = (to - trans.position).normalized;
            float ang = Vector3.Angle(start, dir);
            if (ang <= angle / 2)
            {
                return true;
            }
            return false;
        }
        
    }

    private void CalcSkillMove(EntityBase entity, SkillCfg skillData)
    {
        List<int> skillMoveLst = skillData.skillMoveList;
        int sum = 0;

        for (int i = 0; i < skillMoveLst.Count; i++)
        {
            SkillMoveCfg skillMoveData = resSvc.GetSkillMoveCfg(skillMoveLst[i]);
            sum += skillMoveData.delayTime;
            float speed = skillMoveData.moveDis / (skillMoveData.moveTime / 1000f);
            if (sum > 0)
            {
                int moveId = timerSvc.AddTimeTask((int tid) =>
                 {
                     entity.SetSkillMove(true, speed);
                     entity.RmvMoveCbId(tid);
                 }, sum, PETimeUnit.Millisecond);
                entity.skMoveCbLst.Add(moveId);
            }
            else
            {
                entity.SetSkillMove(true, speed);
            }
            sum += skillMoveData.moveTime;
            int stopId = timerSvc.AddTimeTask((int tid) =>
             {
                 entity.SetSkillMove(false);
                 entity.RmvMoveCbId(tid);
             }, sum, PETimeUnit.Millisecond);
            entity.skMoveCbLst.Add(stopId);
        }
    }
}