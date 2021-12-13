/****************************************************
    文件：BaseData.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/5/13 17:54:9
	功能：配置数据类
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public class BaseData 
{
    public int id;
}
public class MapCfg : BaseData
{
    public string mapName;
    public string sceneName;
    public int power;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
    public List<MonsterData> monsterLst;
    public int coin;
    public int exp;
    public int crystal;
}
public class AutoGuideCfg : BaseData
{
    public int npcId;
    public string dilogArr;
    public int actId;
    public int coin;
    public int exp;
}
public class StrongCfg : BaseData
{
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}
public class TaskRewardCfg : BaseData
{
    public string taskName;
    public int count;
    public int exp;
    public int coin;
}
public class TaskRewardData : BaseData
{
    public int prg;
    public bool taked;
}
public class SkillCfg : BaseData
{
    public string skillName;
    public int skillTime;
    public int cdTime;
    public int aniAction;
    public string fx;
    public bool isCombo;
    public bool isCollide;
    public bool isBreak;
    public DamageType dmgType;
    public List<int> skillMoveList;
    public List<int> skillActionList;
    public List<int> skillDamageList;
}
public class SkillActionCfg : BaseData
{
    public int delayTime;
    public float radius;//伤害计算范围
    public int angle;//伤害有效角度
}
public class SkillMoveCfg : BaseData
{
    public int delayTime;
    public int moveTime;
    public float moveDis;
}
public class MonsterCfg : BaseData
{
    public string mName;
    public string resPath;
    public MonsterType monsterType;
    public bool isStop;
    public int skillID;
    public float atkDis;
    public BattleProps bps;
}
public class MonsterData : BaseData
{
    public int mWave;//批次
    public int mIndex;//序号
    public MonsterCfg mCfg;
    public Vector3 mBornPos;
    public Vector3 mBornRote;
    public int mLevel;
}
public class BattleProps
{
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;
}