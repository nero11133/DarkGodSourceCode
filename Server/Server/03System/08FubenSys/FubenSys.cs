/****************************************************
	文件：FubenSys.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/16 17:07   	
	功能：副本业务系统
*****************************************************/


using PEProtocol;

public class FubenSys
{
    private static FubenSys instance;
    public static FubenSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FubenSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("FubenSys Init Done");
    }

    public void ReqFubenFight(MsgPack pack)
    {
        ReqFubenFight data = pack.msg.reqFubenFight;
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        MapCfg mc = CfgSvc.Instance.GetMapCfg(data.fbid);
        int power = mc.power;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspFubenFight,
        };
        if (pd.fuben < data.fbid)
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        else if (pd.power < power)
        {
            msg.err = (int)ErrorCode.LackPower;
        }
        else
        {
            pd.power -= power;
            if (cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.rspFubenFight = new RspFubenFight
                {
                    fbid = data.fbid,
                    power = pd.power
                };
            }
            else
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
        }
        pack.session.SendMsg(msg);
    }

    public void ReqFubenFightEnd(MsgPack pack)
    {
        ReqFubenFightEnd data = pack.msg.reqFubenFightEnd;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspFubenFightEnd
        };
        //校验战斗是否合法
        if (data.win)
        {
            if (data.costTime > 0 && data.restHp > 0)
            {
                //根据fbid获取相应奖励
                MapCfg rd = CfgSvc.Instance.GetMapCfg(data.fbid);
                PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
                TaskSys.Instance.CalcTaskPrg(pd, 2);
                pd.coin += rd.coin;
                pd.crystal = rd.crystal;
                PECommon.CalcExp(pd, rd.exp);

                if (pd.fuben == data.fbid)
                {
                    pd.fuben += 1;
                }

                if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    RspFubenFightEnd rspFubenFightEnd = new RspFubenFightEnd
                    {
                        win = data.win,
                        fbid = data.fbid,
                        coin = pd.coin,
                        lv = pd.lv,
                        crystal = pd.crystal,
                        exp = pd.exp,
                        fuben = pd.fuben,
                        restHp = data.restHp,
                        costTime = data.costTime
                    };
                    msg.rspFubenFightEnd = rspFubenFightEnd;
                }
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        pack.session.SendMsg(msg);
    }
}

