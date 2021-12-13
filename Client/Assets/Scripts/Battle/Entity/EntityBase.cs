/****************************************************
	文件：EntityBase.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/17 19:55   	
	功能：逻辑实体抽象基类
*****************************************************/
using UnityEngine;
using System.Collections.Generic;

public abstract class EntityBase
{
    public EntityType entityType = EntityType.None;
    public EntityState entityState = EntityState.None;
    public AniState currentState = AniState.None;
    public StateMgr stateMgr = null;
    protected Controller controller = null;
    public SkillMgr skillMgr = null;
    public BattleMgr battleMgr = null;
    public SkillCfg skillCfg;

    //技能移动回调id
    public List<int> skMoveCbLst = new List<int>();
    //技能伤害回调Id
    public List<int> skActionCbLst = new List<int>();

    public List<int> bornCbLst = new List<int>();

    public int skEndCb = -1;

    private BattleProps props;
    public BattleProps Props
    {
        get
        {
            return props;
        }

        protected set
        {
            props = value;
        }
    }

    private int hp;
    public int Hp
    {
        get
        {
            return hp;
        }

        set
        {
            //通知ui层
            //PECommon.Log("HP Change:" + hp + "to" + value);
            SetHpVal(hp, value);
            hp = value;
        }
    }

    private string name;
    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }



    public bool canControl = true;
    public bool canRlsSkill = true;
    public Queue<int> comboQue = new Queue<int>();
    public int nextSkillID = 0;



    public void Idle()
    {
        stateMgr.ChangeState(this, AniState.Idle, null);
    }

    public void Born()
    {
        stateMgr.ChangeState(this, AniState.Born, null);
    }

    public void Move()
    {
        stateMgr.ChangeState(this, AniState.Move, null);
    }

    public void Attack(int skillID)
    {
        stateMgr.ChangeState(this, AniState.Attack, skillID);
    }

    public void Hit()
    {
        stateMgr.ChangeState(this, AniState.Hit, null);
    }

    public void Die()
    {
        stateMgr.ChangeState(this, AniState.Die, null);
    }

    public void SetCtrl(Controller ctrl)
    {
        controller = ctrl;
    }

    public void SetActive(bool active = true)
    {
        if (controller != null)
        {
            controller.gameObject.SetActive(active);
        }
    }

    public AnimationClip[] GetAnimationClips()
    {
        if (controller != null)
        {
            return controller.animator.runtimeAnimatorController.animationClips;
        }
        return null;

    }

    public virtual void SetBattleProps(BattleProps props)
    {
        Hp = props.hp;
        Props = props;
    }

    public virtual void SetBlend(float blend)
    {
        if (controller != null)
        {
            controller.SetBlend(blend);
        }
    }

    public virtual void SetDir(Vector2 dir)
    {
        if (controller != null)
        {
            controller.Dir = dir;
        }
    }

    public virtual void SetAction(int act)
    {
        if (controller != null)
        {
            controller.SetAction(act);
        }
    }

    public virtual void SetFx(string name, float time)
    {
        if (controller != null)
        {
            controller.SetFx(name, time);
        }
    }

    public virtual void SetSkillMove(bool move, float skillSpeed = 0)
    {
        if (controller != null)
        {
            controller.SetSkillMove(move, skillSpeed);
        }
    }

    public virtual void SkillAttack(int skillID)
    {
        skillMgr.SkillAttack(this, skillID);
    }

    #region 战斗信息显示
    public virtual void SetCritical(int critical)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetCritical(Name, critical);
        }

    }

    public virtual void SetDodge()
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetDodge(Name);
        }

    }

    public virtual void SetHurt(int hurt)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetHurt(Name, hurt);
        }

    }

    public virtual void SetHpVal(int oldVal, int newVal)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetHpVal(Name, oldVal, newVal);
        }

    }
    #endregion

    public virtual void SetAtkDir(Vector2 dir, bool offSet = false)
    {
        if (controller != null)
        {
            if (offSet)
            {
                controller.SetAtkDirCam(dir);
            }
            else
            {
                controller.SetAtkDirLocal(dir);
            }

        }
    }

    public void ExitCurrentSkill()
    {
        SetAction(Constants.ActionDefault);
        canControl = true;
        if (skillCfg != null)
        {
            if (!skillCfg.isBreak)
            {
                entityState = EntityState.None;
            }
            if (skillCfg.isCombo)
            {
                if (comboQue.Count > 0)
                {
                    nextSkillID = comboQue.Dequeue();
                }
                else
                {
                    nextSkillID = 0;
                }
            }
        }
       
        skillCfg = null;

    }

    public AudioSource GetAudio()
    {
        return controller.GetComponent<AudioSource>();
    }

    public CharacterController GetCC()
    {
        return controller.GetComponent<CharacterController>();
    }



    public virtual Vector2 GetDir()
    {
        return Vector2.zero;
    }

    public virtual Vector3 GetPos()
    {
        return controller.transform.position;
    }

    public virtual Transform GetTrans()
    {
        return controller.transform;
    }

    public virtual Vector2 CalcTargetDir()
    {
        return Vector2.zero;
    }

    public virtual void TickAILogic()
    {

    }

    public void RmvMoveCbId(int tid)
    {
        int index = -1;
        for (int i = 0; i < skMoveCbLst.Count; i++)
        {
            if (skMoveCbLst[i] == tid)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            skMoveCbLst.RemoveAt(index);
        }
    }

    public void RmvActionCbId(int tid)
    {
        int index = -1;
        for (int i = 0; i < skActionCbLst.Count; i++)
        {
            if (skActionCbLst[i] == tid)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            skActionCbLst.RemoveAt(index);
        }
    }

    public void RmvBornCbId(int tid)
    {
        int index = -1;
        for (int i = 0; i < bornCbLst.Count; i++)
        {
            if (bornCbLst[i] == tid)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            bornCbLst.RemoveAt(index);
        }
    }

    public void RmvSkillCb()
    {
        SetSkillMove(false);
        SetDir(Vector2.zero);
        for (int i = 0; i < skMoveCbLst.Count; i++)
        {
            int tid = skMoveCbLst[i];
            TimerSvc.Instance.DeleteTimeTask(tid);
        }
        for (int i = 0; i < skActionCbLst.Count; i++)
        {
            int tid = skActionCbLst[i];
            TimerSvc.Instance.DeleteTimeTask(tid);
        }
       

        //清空连招数据
        if (nextSkillID != 0 || comboQue.Count > 0)
        {
            nextSkillID = 0;
            comboQue.Clear();
            battleMgr.lastAtkTime = 0;
            battleMgr.comboIndex = 0;
        }
        //攻击被中断，删除定时回调
        if (skEndCb != -1)
        {
            TimerSvc.Instance.DeleteTimeTask(skEndCb);
            skEndCb = -1;
        }
        skActionCbLst.Clear();
        skMoveCbLst.Clear();
        
    }

    public void RmvBornCb()
    {
        for (int i = 0; i < bornCbLst.Count; i++)
        {
            int tid = bornCbLst[i];
            TimerSvc.Instance.DeleteTimeTask(tid);
        }
        bornCbLst.Clear();
    }

    public virtual bool GetBreakState()
    {
        return true;
    }

    
}

