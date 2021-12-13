/****************************************************
	文件：GuideSys.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/05/24 21:32   	
	功能：任务引导系统
*****************************************************/
using PEProtocol;


public class GuideSys
{

    private static GuideSys instance;
    public static GuideSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GuideSys();
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
        PECommon.Log("GuideSys Init Done");
    }

    public void ReqGuide(MsgPack pack)
    {
        ReqGuide data = pack.msg.reqGuide;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspGuide
        };
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        GuideCfg guideCfg = cfgSvc.GetGuideTaskCfg(data.guideId);
        if (pd.guideId == data.guideId)
        {
            //检查是否是智者点拨任务
            if (data.guideId == 1001)
            {
                TaskSys.Instance.CalcTaskPrg(pd, 1);
            }
            //更新引导id
            pd.guideId++;
            //更新玩家数据
            pd.coin += guideCfg.coin;
            PECommon.CalcExp(pd, guideCfg.exp);
            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.rspGuide = new RspGuide
                {
                    guideId = pd.guideId,
                    coin = pd.coin,
                    lv = pd.lv,
                    exp = pd.exp
                };
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ServerDataError;
        }
        pack.session.SendMsg(msg);
    }

   
}

