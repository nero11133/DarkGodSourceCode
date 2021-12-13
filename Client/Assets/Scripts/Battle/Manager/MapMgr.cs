/****************************************************
    文件：MapMgr.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/16 19:37:43
	功能：地图管理器
*****************************************************/

using UnityEngine;

public class MapMgr : MonoBehaviour 
{
    private BattleMgr battleMgr;
    private int waveIndex = 1;

    public TriggerData[] triggerDataArr;

    public void Init(BattleMgr battle)
    {
        battleMgr = battle;
        //实例化第一批怪物
        battleMgr.LoadMonsterByWaveId(waveIndex);
    }

    public void TriggerMonsterBorn(TriggerData triggerData,int waveIndex)
    {
        BoxCollider boxCollider = triggerData.GetComponent<BoxCollider>();
        boxCollider.isTrigger = false;
        battleMgr.LoadMonsterByWaveId(waveIndex);
        battleMgr.ActiveCurrentBatchMonster();
        battleMgr.triggerCheck = true;
    }

    public bool SetNextTriggerOn()
    {
        waveIndex += 1;
        for (int i = 0; i < triggerDataArr.Length; i++)
        {
            if (triggerDataArr[i].triggerWave == waveIndex)
            {
                BoxCollider boxCollider = triggerDataArr[i].GetComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                return false;
            }
        }
        return true;
    }
}