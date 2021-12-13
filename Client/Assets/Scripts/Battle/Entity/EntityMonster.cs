/****************************************************
    文件：EntityMonster.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/25 13:31:33
	功能：怪物逻辑实体
*****************************************************/

using UnityEngine;

public class EntityMonster : EntityBase 
{

    public EntityMonster()
    {
        entityType = EntityType.Monster;
    }

    public MonsterData md;

    private float checkTime = 2;
    private float checkCountTime = 0;
    private float AtkTime = 2;
    private float AtkCountTime = 0;
    private bool runAI = true;

    public override void SetBattleProps(BattleProps props)
    {
        int level = md.mLevel;
        BattleProps p = new BattleProps
        {
            hp = props.hp * level,
            ad = props.ad * level,
            ap = props.ap * level,
            addef = props.addef * level,
            apdef = props.apdef * level,
            dodge = props.dodge * level,
            pierce = props.pierce * level,
            critical = props.critical * level,
        };
        Hp = p.hp;
        Props = p;
    }

    public override void TickAILogic()
    {
        if (!runAI)
        {
            return;
        }
        if (currentState == AniState.Idle || currentState == AniState.Move)
        {
            //if (battleMgr.isGamePause)
            //{
            //    Idle();
            //    return;
            //}
            float delta = Time.deltaTime;
            checkCountTime += delta;
            if (checkCountTime < checkTime)
            {
                return;
            }
            else
            {
                //计算目标方向
                Vector2 dir = CalcTargetDir();
                //判断在不在攻击范围
                if (dir == Vector2.zero) return;
                if (!InAtkRange())
                {
                    //不在，设置方向，进入移动状态
                    SetDir(dir);
                    Move();
                }
                else
                {
                    //在，停止移动，进行攻击
                    SetDir(Vector2.zero);
                    //判断攻击间隔
                    AtkCountTime += checkCountTime;
                    if (AtkCountTime >= AtkTime)
                    {
                        SetAtkDir(dir);
                        Attack(md.mCfg.skillID);
                        AtkCountTime = 0;
                    }
                    else
                    {
                        Idle();
                    }
                }
                checkCountTime = 0;
                checkTime = PETools.GetRDInt(1, 5) * 1.0f / 10;
            }
        }
        
    }

    public override Vector2 CalcTargetDir()
    {
        EntityPlayer player = battleMgr.entitySelfPlayer;
        if (player==null||player.currentState==AniState.Die)
        {
            runAI = false;
            SetDir(Vector2.zero);
            Idle();
            return Vector2.zero;
        }
        else
        {
            Vector3 self = GetPos();
            Vector3 target = player.GetPos();
            return new Vector2(target.x - self.x, target.z - self.z).normalized;
        }
    }

    private bool InAtkRange()
    {
        EntityPlayer player = battleMgr.entitySelfPlayer;
        if (player == null || player.currentState == AniState.Die)
        {
            runAI = false;
            return false;
        }
        else
        {
            Vector3 self = GetPos();
            Vector3 target = player.GetPos();
            self.y = 0;
            target.y = 0;
            float dis = Vector3.Distance(self, target);
            if (dis <= md.mCfg.atkDis)
            {
                return true;
            }
            return false;
        }
    }

    public override bool GetBreakState()
    {
        if (md.mCfg.isStop)
        {
            if (skillCfg != null)
            {
                return skillCfg.isBreak;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public override void SetHpVal(int oldVal, int newVal)
    {
        if (md.mCfg.monsterType == MonsterType.Boss)
        {
            if (controller != null)
            {
                BattleSys.Instance.playerCtrlWnd.SetBossHpVal(oldVal, newVal, Props.hp);
            }
            
        }
        else
        {
            base.SetHpVal(oldVal, newVal);
        }
        
    }
}