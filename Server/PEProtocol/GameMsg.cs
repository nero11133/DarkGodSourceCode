/****************************************************
	文件：GameMsg.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/04/27 17:53   	
	功能：网络通信协议（客户端服务端通用）
*****************************************************/
using PENet;
using System;

namespace PEProtocol
{
    [Serializable]
    public class GameMsg:PEMsg
    {
        public ReqLogin reqLogin;
        public RspLogin rspLogin;

        public ReqRename reqRename;
        public RspRename rspRename;

        public ReqGuide reqGuide;
        public RspGuide rspGuide;

        public ReqStrong reqStrong;
        public RspStrong rspStrong;

        public SndChat sndChat;
        public PshChat pshChat;

        public ReqBuy reqBuy;
        public RspBuy rspBuy;

        public PshPower pshPower;

        public ReqTakeTask reqTakeTask;
        public RspTakeTask rspTakeTask;

        public PshTaskPrg pshTaskPrg;

        public ReqFubenFight reqFubenFight;
        public RspFubenFight rspFubenFight;

        public ReqFubenFightEnd reqFubenFightEnd;
        public RspFubenFightEnd rspFubenFightEnd;
    }
    #region 登录相关
    [Serializable]
    public class ReqLogin
    {
        public string acct;
        public string pass;
    }
    [Serializable]
    public class RspLogin
    {
        public PlayerData playerData;
    }
    [Serializable]
    public class ReqRename
    {
        public string name;
    }
    [Serializable]
    public class RspRename
    {
        public string name;
    }
    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int power;
        public int coin;
        public int diamond;
        public int crystal;

        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int dodge;//闪避概率
        public int pierce;//穿透比率
        public int critical;//暴击概率

        public int guideId;//自动任务id

        public int[] strongArr;//强化升级数据

        public string[] taskArr;//任务奖励数据

        public long time;

        public int fuben;
        //toadd
    }
    #endregion

    #region 引导相关
    [Serializable]
    public class ReqGuide
    {
        public int guideId;
    }

    [Serializable]
    public class RspGuide
    {
        public int guideId;
        public int lv;
        public int coin;
        public int exp;
    }
    #endregion

    #region 强化相关
    [Serializable]
    public class ReqStrong
    {
        public int pos;
    }
    [Serializable]
    public class RspStrong
    {
        public int coin;
        public int crtstal;
        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int[] strongArr;
    }
    #endregion

    #region 聊天相关
    [Serializable]
    public class SndChat
    {
        public string chat;
    }
    [Serializable]
    public class PshChat
    {
        public string name;
        public string chat;
    }
    #endregion

    #region 交易相关
    [Serializable]
    public class ReqBuy
    {
        public int buyType;
        public int cost;
    }
    [Serializable]
    public class RspBuy
    {
        public int buyType;
        public int diamond;
        public int coin;
        public int power;
    }
    [Serializable]
    public class PshPower
    {
        public int power;
    }
    #endregion

    #region 任务奖励相关
    [Serializable]
    public class ReqTakeTask
    {
        public int taskID;
    }
    [Serializable]
    public class RspTakeTask
    {
        public int coin;
        public int exp;
        public int lv;
        public string[] taskArr;
    }
    [Serializable]
    public class PshTaskPrg
    {
        public string[] taskArr;
    }
    #endregion

    #region 副本战斗相关
    [Serializable]
    public class ReqFubenFight
    {
        public int fbid;
    }
    [Serializable]
    public class RspFubenFight
    {
        public int fbid;
        public int power;
    }

    [Serializable]
    public class ReqFubenFightEnd
    {
        public bool win;
        public int fbid;
        public int restHp;
        public int costTime;
    }
    [Serializable]
    public class RspFubenFightEnd
    {
        public bool win;
        public int fbid;
        public int restHp;
        public int costTime;

        //副本奖励
        public int coin;
        public int lv;
        public int exp;
        public int crystal;
        public int fuben;
    }
    #endregion

    public class SrvCfg
    {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 17666;
    }

    public enum ErrorCode
    {
        None=0,//没有错误
        ServerDataError,//服务器数据异常
        UpdateDBError,//更新数据库出错
        ClientDataError,//客户端数据异常
        AcctIsOnline,//账号已经上线
        WrongPass,//密码错误
        NameIsExist,//名字已经存在

        LackCoin,
        LackCrystal,
        LackLv,
        LackDiamond,
        LackPower,
    }

    public enum CMD
    {
        None=0,
        //登录相关 100
        ReqLogin=101,
        RspLogin=102,

        ReqRename=103,
        RspRename=104,

        //主城相关 200
        ReqGuide=201,
        RspGuide=202,

        ReqStrong=203,
        RspStrong=204,

        SndChat=205,
        PshChat=206,

        ReqBuy=207,
        RspBuy=208,

        PshPower=209,

        ReqTakeTask=210,
        RspTakeTask=211,

        PshTaskPrg=212,

        ReqFubenFight=301,
        RspFubenFight=302,

        ReqFubenFightEnd = 303,
        RspFubenFightEnd = 304,
    }
}
