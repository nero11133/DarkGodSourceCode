/****************************************************
	文件：BuySys.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/08 16:41   	
	功能：交易购买系统
*****************************************************/

using PEProtocol;

public class BuySys
{
    private static BuySys instance;
    public static BuySys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BuySys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("BuySys Init Done");
    }

    public void ReqBuy(MsgPack pack)
    {
        ReqBuy data = pack.msg.reqBuy;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspBuy,
        };
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        if (pd.diamond < data.cost)
        {
            msg.err = (int)ErrorCode.LackDiamond;
        }
        else
        {
            pd.diamond -= data.cost;
            PshTaskPrg pshTaskPrg = null;
            switch (data.buyType)
            {
                case 0:
                    pd.power += 100;
                    //更新任务进度
                    pshTaskPrg = TaskSys.Instance.GetTaskPrg(pd, 4);
                    break;
                case 1:
                    pd.coin += 1000;
                    //更新任务进度
                    pshTaskPrg = TaskSys.Instance.GetTaskPrg(pd, 5);
                    break;
            }
            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspBuy rspBuy = new RspBuy()
                {
                    buyType = data.buyType,
                    diamond = pd.diamond,
                    coin = pd.coin,
                    power = pd.power
                };
                msg.rspBuy = rspBuy;
                //并包处理
                msg.pshTaskPrg = pshTaskPrg;
            }
        }
        pack.session.SendMsg(msg);
    }
}

