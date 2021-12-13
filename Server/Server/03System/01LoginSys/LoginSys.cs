/****************************************************
	文件：LoginSys.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/04/27 16:28   	
	功能：登录业务系统
*****************************************************/

using PENet;
using PEProtocol;


public class LoginSys
{
    private static LoginSys instance;
    public static LoginSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoginSys();
            }
            return instance;
        }
    }

    public void Init()
    {
        PECommon.Log("LoginSys Init Done");
    }
    public void ReqLogin(MsgPack pack)
    {
        ReqLogin data = pack.msg.reqLogin;
        //当前账号是否已经上线
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspLogin,
          
        };
        if (CacheSvc.Instance.IsAcctOnline(data.acct))
        {
            //已上线：返回错误信息
            msg.err = (int)ErrorCode.AcctIsOnline;
        }
        else
        {
            //未上线：
            //账号是否存在
            
            PlayerData pd = CacheSvc.Instance.GetPlayerData(data.acct, data.pass);
            if (pd == null)
            {
                //密码错误
                msg.err = (int)ErrorCode.WrongPass;
            }
            else
            {
                //计算离线体力增长
                int power = pd.power;
                long nowTime = TimerSvc.Instance.GetNowTime();
                long milliseconds = nowTime - pd.time;
                int addPower = (int)(milliseconds / (1000 * 60 * PECommon.PowerAddSpace)) * PECommon.PowerAddCount;
                if (addPower > 0)
                {
                    int powerMax = PECommon.GetPowerLimit(pd.lv);
                    if (pd.power < powerMax)
                    {
                        pd.power += addPower;
                        if (pd.power > powerMax)
                        {
                            pd.power = powerMax;
                        }
                    }
                }
                if (power != pd.power)
                {
                    CacheSvc.Instance.UpdatePlayerData(pd.id, pd);
                }
                msg.rspLogin = new RspLogin { playerData = pd };
                //缓存账号数据
                CacheSvc.Instance.AcctOnLine(data.acct, pd, pack.session);
            }
            
            
        }

        //回应客户端

        pack.session.SendMsg(msg);
    }

    public void ReqRename(MsgPack pack)
    {
        //todo
        //名字是否存在
        //存在返回错误码
        //不存在 更新缓存数据
        ReqRename data = pack.msg.reqRename;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspRename
        };
        if (CacheSvc.Instance.IsNameExist(data.name))
        {
            msg.err = (int)ErrorCode.NameIsExist;
        }
        else
        {
            PlayerData playerData = CacheSvc.Instance.GetPlayerDataBySession(pack.session);
            playerData.name = data.name;
            if (CacheSvc.Instance.UpdatePlayerData(playerData.id, playerData))
            {
                msg.rspRename = new RspRename
                {
                    name = data.name
                };
            }
            else
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
        }
        pack.session.SendMsg(msg);
    }

    public void ClearOffLineData(ServerSession session)
    {
        PlayerData pd = CacheSvc.Instance.GetPlayerDataBySession(session);
        if (pd != null)
        {
            pd.time = TimerSvc.Instance.GetNowTime();
            if (!CacheSvc.Instance.UpdatePlayerData(pd.id, pd))
            {
                PECommon.Log("Update OffLine Time Error", LogType.Error);
            }
        }
        CacheSvc.Instance.AcctOffLine(session);
    }
}

