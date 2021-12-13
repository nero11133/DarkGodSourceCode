/****************************************************
	文件：StrongSys.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/08 15:41   	
	功能：强化升级系统
*****************************************************/
using PEProtocol;

public class StrongSys
{
    private static StrongSys instance;
    public static StrongSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StrongSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("StrongSys Init Done");
    }

    public void ReqStrong(MsgPack pack)
    {
        ReqStrong reqStrong = pack.msg.reqStrong;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspStrong
        };
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        int curtStarLv = pd.strongArr[reqStrong.pos];
        StrongCfg nextSd = cfgSvc.GetStrongData(reqStrong.pos, curtStarLv + 1);
        //条件判断
        if (pd.lv < nextSd.minlv)
        {
            msg.err = (int)ErrorCode.LackLv;
        }else if (pd.coin < nextSd.coin)
        {
            msg.err = (int)ErrorCode.LackCoin;
        }else if (pd.crystal < nextSd.crystal)
        {
            msg.err = (int)ErrorCode.LackCrystal;
        }
        else
        {
            //资源扣除
            pd.coin -= nextSd.coin;
            pd.crystal -= nextSd.crystal;
            //增加属性
            pd.hp += nextSd.addhp;
            pd.ad += nextSd.addhurt;
            pd.ap += nextSd.addhurt;
            pd.addef += nextSd.adddef;
            pd.apdef += nextSd.adddef;
            pd.strongArr[reqStrong.pos] += 1;
            //更新任务进度
            TaskSys.Instance.CalcTaskPrg(pd, 3);

            //更新数据库
            if (cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.rspStrong = new RspStrong
                {
                    coin = pd.coin,
                    crtstal = pd.crystal,
                    hp = pd.hp,
                    ad = pd.ad,
                    ap = pd.ap,
                    addef = pd.addef,
                    apdef = pd.apdef,
                    strongArr = pd.strongArr,
                };
            }
            else
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
        }
        pack.session.SendMsg(msg);

    }
}

