/****************************************************
	文件：PowerSys.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/09 16:02   	
	功能：体力恢复系统
*****************************************************/
using PEProtocol;
using System.Collections.Generic;

public class PowerSys
{
    private static PowerSys instance;
    public static PowerSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PowerSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        TimerSvc.Instance.AddTimeTask(CalcPowerAdd, PECommon.PowerAddSpace, PETimeUnit.Minute, 0);
        PECommon.Log("PowerSys Init Done");
    }

    private void CalcPowerAdd(int tid)
    {
        //计算体力增长
        PECommon.Log("All OnLine Player Add Power...");
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PshPower,
            pshPower = new PshPower(),
        };

        //所有在线玩家获得实时体力增长推送数据
        Dictionary<ServerSession, PlayerData> onlineDic = cacheSvc.GetOnLineCache();
        foreach (var item in onlineDic)
        {
            PlayerData pd = item.Value;
            ServerSession session = item.Key;
            int powerMax = PECommon.GetPowerLimit(pd.lv);
            if (pd.power >= powerMax)
            {
                continue;
            }
            else
            {
                pd.power += PECommon.PowerAddCount;
                pd.time = TimerSvc.Instance.GetNowTime();
                if (pd.power > powerMax)
                {
                    pd.power = powerMax;
                }
            }
            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.pshPower.power = pd.power;
                session.SendMsg(msg);
            }
        }
    }
}

