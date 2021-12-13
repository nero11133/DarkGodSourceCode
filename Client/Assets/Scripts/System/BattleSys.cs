/****************************************************
    文件：BattleSys.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/16 18:59:46
	功能：战斗业务系统
*****************************************************/

using UnityEngine;
using PEProtocol;

public class BattleSys : SystemRoot 
{
    public static BattleSys Instance;
    public BattleMgr battleMgr;

    public PlayerCtrlWnd playerCtrlWnd;
    public BattleEndWnd battleEndWnd;

    private int fbId;
    private double startTime;

    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init BattleSys");
    }

    public void StartBattle(int mapId)
    {
        fbId = mapId;
        GameObject go = new GameObject
        {
            name = "BattleRoot"
        };
        go.transform.SetParent(GameRoot.Instance.transform);
        battleMgr = go.AddComponent<BattleMgr>();
        battleMgr.Init(mapId,()=> {
            startTime = timerSvc.GetNowTime();
            SetPlayerCtrlWndState();
        });
        
    }

    public void EndBattle(bool isWin,int restHP)
    {
        playerCtrlWnd.SetWndowState(false);
        GameRoot.Instance.dynamicWnd.RmvAllHpItem();
        if (isWin)
        {
            double endTime = timerSvc.GetNowTime();
            //发送结算请求todo
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqFubenFightEnd,
                reqFubenFightEnd = new ReqFubenFightEnd
                {
                    win = isWin,
                    fbid = fbId,
                    restHp = restHP,
                    costTime = (int)(endTime - startTime) / 1000
                }
            };
            netSvc.SendMsg(msg);
        }
        else
        {
            SetBattleEndWndState(FBEndType.Lose);
        }
    }

    public void DestroyBattle()
    {
        SetPlayerCtrlWndState(false);
        SetBattleEndWndState(FBEndType.None, false);
        GameRoot.Instance.dynamicWnd.RmvAllHpItem();
        Destroy(battleMgr.gameObject);
    }

    public void RspFubenFightEnd(GameMsg msg)
    {
        RspFubenFightEnd data = msg.rspFubenFightEnd;
        GameRoot.Instance.SetPlayerDataByFBEnd(data);
        battleEndWnd.SetBattleEndData(data.fbid, data.costTime, data.restHp);
        SetBattleEndWndState(FBEndType.Win);
    }

    public void SetBattleEndWndState(FBEndType fBEndType,bool state = true)
    {
        battleEndWnd.SetFBEndType(fBEndType);
        battleEndWnd.SetWndowState(state);
    }

    public void SetPlayerCtrlWndState(bool state = true)
    {
        playerCtrlWnd.SetWndowState(state);
    }

    public void SetMoveDir(Vector2 dir)
    {
        battleMgr.SetMoveDir(dir);
    }

    public void ReqReleaseSkill(int index)
    {
        battleMgr.ReqReleaseSkill(index);
    }

    public Vector2 GetDir()
    {
        return playerCtrlWnd.GetDir();
    }
}