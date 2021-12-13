/****************************************************
	文件：EntityPlayer.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/18 19:15   	
	功能：玩家逻辑实体
*****************************************************/



using UnityEngine;
using System.Collections.Generic;

public class EntityPlayer : EntityBase
{
    public EntityPlayer()
    {
        entityType = EntityType.Player;
    }


    public override Vector2 GetDir()
    {
        return battleMgr.GetDir();
    }

    public override Vector2 CalcTargetDir()
    {
        EntityMonster targetMonster = FindClosedMonster();
        if (targetMonster != null)
        {
            Vector3 self = GetPos();
            Vector3 target = targetMonster.GetPos();
            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }
        else
        {
            return Vector2.zero;
        }
    }

    private EntityMonster FindClosedMonster()
    {
        List<EntityMonster> list = battleMgr.GetEntityMonsters();
        if (list == null || list.Count == 0)
        {
            return null;
        }
        EntityMonster targetMonster = null;
        Vector3 self = GetPos();
        float dis = 0;
        for (int i = 0; i < list.Count; i++)
        {
            Vector3 targetPos = list[i].GetPos();
            if (i == 0)
            {
                dis = Vector3.Distance(self, targetPos);
                targetMonster = list[i];
            }
            else
            {
                float calcDis = Vector3.Distance(self, targetPos);
                if (calcDis < dis)
                {
                    dis = calcDis;
                    targetMonster = list[i];
                }
            }
        }
        return targetMonster;
    }

    public override void SetDodge()
    {
        GameRoot.Instance.dynamicWnd.SetSelfDodge();
    }

    public override void SetHpVal(int oldVal, int newVal)
    {
        BattleSys.Instance.playerCtrlWnd.SetSelfHpBarVal(newVal);
    }
}

