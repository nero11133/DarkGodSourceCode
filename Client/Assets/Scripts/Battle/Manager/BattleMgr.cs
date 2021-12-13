/****************************************************
    文件：BattleMgr.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/16 19:5:29
	功能：战场管理器
*****************************************************/

using UnityEngine;
using System.Collections.Generic;
using PEProtocol;
using System;

public class BattleMgr : MonoBehaviour
{
    private ResSvc resSvc;
    private AudioSvc audioSvc;

    private StateMgr stateMgr;
    private SkillMgr skillMgr;
    private MapMgr mapMgr;

    public EntityPlayer entitySelfPlayer;
    private Dictionary<string, EntityMonster> monstersDic = new Dictionary<string, EntityMonster>();

    private MapCfg mapCfg;

    public void Init(int mapId,Action action)
    {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        //初始化管理器
        stateMgr = gameObject.AddComponent<StateMgr>();
        stateMgr.Init();

        skillMgr = gameObject.AddComponent<SkillMgr>();
        skillMgr.Init();

        //加载场景地图
        mapCfg = resSvc.GetMapCfg(mapId);
        resSvc.AsyncLoadScene(mapCfg.sceneName, () =>
        {
            //初始化地图数据
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr = map.GetComponent<MapMgr>();
            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;
            mapMgr.Init(this);

            Camera.main.transform.localPosition = mapCfg.mainCamPos;
            Camera.main.transform.localEulerAngles = mapCfg.mainCamRote;

            audioSvc.PlayBGM(Constants.BGHuangYe);

            LoadPlayer(mapCfg);
            entitySelfPlayer.Idle();
            //激活第一批怪物
            ActiveCurrentBatchMonster();
            if (action != null)
            {
                action();
            }
        });
    }

    public bool triggerCheck = true;
    public bool isGamePause = false;
    private void Update()
    {
        foreach (var item in monstersDic)
        {
            EntityMonster em = item.Value;
            em.TickAILogic();
        }
        if (mapMgr != null)
        {
            if (monstersDic.Count == 0 && triggerCheck)
            {
                bool isEnd = mapMgr.SetNextTriggerOn();
                triggerCheck = false;
                if (isEnd)
                {
                    //关卡结束，胜利
                    EndBattle(true, entitySelfPlayer.Hp);
                }
            }
        }
    }

    public void EndBattle(bool isWin,int restHP)
    {
        audioSvc.StopBGM();
        BattleSys.Instance.EndBattle(isWin, restHP);
        isGamePause = true;
    }

    private void LoadPlayer(MapCfg mapCfg)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssassinBattlePlayerPrefab, true);
        player.transform.position = mapCfg.playerBornPos;
        player.transform.localEulerAngles = mapCfg.playerBornRote;
        player.transform.localScale = Vector3.one;

        PlayerData pd = GameRoot.Instance.PlayerData;
        BattleProps battleProps = new BattleProps
        {
            hp = pd.hp,
            ad = pd.ad,
            ap = pd.ap,
            addef = pd.addef,
            apdef = pd.apdef,
            dodge = pd.dodge,
            pierce = pd.pierce,
            critical = pd.critical
        };

        entitySelfPlayer = new EntityPlayer
        {
            stateMgr = stateMgr,
            skillMgr = skillMgr,
            battleMgr = this,
        };
        entitySelfPlayer.SetBattleProps(battleProps);
        entitySelfPlayer.Name = "AssassinBattle";

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.Init();
        entitySelfPlayer.SetCtrl(playerController);
    }

    public void LoadMonsterByWaveId(int wave)
    {
        for (int i = 0; i < mapCfg.monsterLst.Count; i++)
        {
            MonsterData md = mapCfg.monsterLst[i];
            if (md.mWave == wave)
            {
                GameObject m = resSvc.LoadPrefab(md.mCfg.resPath, true);
                m.name = "m" + md.mWave + "_" + md.mIndex;
                m.transform.localEulerAngles = md.mBornRote;
                m.transform.localPosition = md.mBornPos;
                m.transform.localScale = Vector3.one;

                EntityMonster entityMonster = new EntityMonster
                {
                    stateMgr = stateMgr,
                    skillMgr = skillMgr,
                    battleMgr = this,
                };
                //设置初始属性
                entityMonster.md = md;
                entityMonster.Name = m.name;
                entityMonster.SetBattleProps(md.mCfg.bps);
                MonsterController monsterController = m.GetComponent<MonsterController>();
                monsterController.Init();
                entityMonster.SetCtrl(monsterController);
                m.SetActive(false);
                monstersDic.Add(m.name, entityMonster);
                if (md.mCfg.monsterType == MonsterType.Normal)
                {
                    GameRoot.Instance.dynamicWnd.AddHpItem(m.name, entityMonster.Hp, monsterController.hpRoot);
                }
                else if (md.mCfg.monsterType == MonsterType.Boss)
                {
                    BattleSys.Instance.playerCtrlWnd.SetBossHpBarState(true);
                }
                
            }
        }
    }

    public void ActiveCurrentBatchMonster()
    {
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            foreach (var item in monstersDic)
            {
                item.Value.SetActive(true);
                item.Value.Born();
                int bornCbId = TimerSvc.Instance.AddTimeTask((int id) =>
                {
                    //出生一秒后进入idle
                    item.Value.Idle();
                    item.Value.RmvBornCbId(id);
                }, 1);
                item.Value.bornCbLst.Add(bornCbId);
            }
        }, 500, PETimeUnit.Millisecond);
    }

    public List<EntityMonster> GetEntityMonsters()
    {
        List<EntityMonster> monsterList = new List<EntityMonster>();
        foreach (var item in monstersDic)
        {
            monsterList.Add(item.Value);
        }
        return monsterList;
    }

    public void RmvMonster(string mName)
    {
        if (monstersDic.ContainsKey(mName))
        {
            monstersDic.Remove(mName);
            GameRoot.Instance.dynamicWnd.RmvHpItem(mName);
        }
    }

    #region 技能释放与角色控制
    public void SetMoveDir(Vector2 dir)
    {
        //设置玩家移动
        if (entitySelfPlayer.canControl == false)
        {
            return;
        }
        if (entitySelfPlayer.currentState == AniState.Idle || entitySelfPlayer.currentState == AniState.Move)
        {
            if (dir == Vector2.zero)
            {
                entitySelfPlayer.Idle();
            }
            else
            {
                entitySelfPlayer.Move();
                entitySelfPlayer.SetDir(dir);
            }
        }
        
    }

    public Vector2 GetDir()
    {
        return BattleSys.Instance.GetDir();
    }

    public void ReqReleaseSkill(int index)
    {
        switch (index)
        {
            case 0:
                ReleaseNormalAtk();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;
        }
    }

    public double lastAtkTime = 0;
    public int comboIndex = 0;
    private int[] comboArr = new[] { 111, 112, 113, 114, 115 };
    private void ReleaseNormalAtk()
    {
        //PECommon.Log("Click NormalAtk");
        if (entitySelfPlayer.currentState == AniState.Attack)
        {
            //在500ms以内进行第二次点击，存连招数据
            double nowAtkTime = TimerSvc.Instance.GetNowTime();
            if ((nowAtkTime - lastAtkTime) / 1000 > 0.4 && lastAtkTime != 0)
            {
                if (comboArr[comboIndex] != comboArr[comboArr.Length - 1])
                {
                    comboIndex += 1;
                    entitySelfPlayer.comboQue.Enqueue(comboArr[comboIndex]);
                    lastAtkTime = nowAtkTime;
                }
                else
                {
                    comboIndex = 0;
                    lastAtkTime = 0;
                }
            }
            //if (nowAtkTime - lastAtkTime < Constants.ComboSpace&&lastAtkTime!=0)
            //{
            //    if (comboArr[comboIndex] != comboArr[comboArr.Length - 1])
            //    {
            //        comboIndex += 1;
            //        entitySelfPlayer.comboQue.Enqueue(comboArr[comboIndex]);
            //        lastAtkTime = nowAtkTime;
            //    }
            //    else
            //    {
            //        comboIndex = 0;
            //        lastAtkTime = 0;
            //    }

            //}
        }
        else if (entitySelfPlayer.currentState == AniState.Idle || entitySelfPlayer.currentState == AniState.Move)
        {
            comboIndex = 0;
            entitySelfPlayer.Attack(comboArr[comboIndex]);
            lastAtkTime = TimerSvc.Instance.GetNowTime();
        }
        
    }

    private void ReleaseSkill1()
    {
        //PECommon.Log("Click Skill1");
        entitySelfPlayer.Attack(101);
    }

    private void ReleaseSkill2()
    {
        //PECommon.Log("Click Skill2");
        entitySelfPlayer.Attack(102);
    }

    private void ReleaseSkill3()
    {
        //PECommon.Log("Click Skill3");
        entitySelfPlayer.Attack(103);
    }
    #endregion

    public bool GetCanRlsSkill()
    {
        return entitySelfPlayer.canRlsSkill;
    }

   
}