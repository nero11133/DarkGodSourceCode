/****************************************************
    文件：NetSvc.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/27 19:11:59
	功能：网络服务
*****************************************************/

using UnityEngine;
using PENet;
using PEProtocol;
using System.Collections.Generic;

public class NetSvc : MonoBehaviour 
{
    public static NetSvc Instance;

    private PESocket<ClientSession, GameMsg> client = null;

    private Queue<GameMsg> msgQue = new Queue<GameMsg>();
    private static readonly string obj = "lock";

    public void InitSvc()
    {
        Instance = this;
        client = new PESocket<ClientSession, GameMsg>();
        client.SetLog(true, (string msg, int lv) =>
         {
             switch (lv)
             {
                 case 0:
                     Debug.Log("Log:" + msg);
                     break;
                 case 1:
                     Debug.LogWarning("Warning:" + msg);
                     break;
                 case 2:
                     Debug.LogError("Error:" + msg);
                     break;
                 case 3:
                     Debug.Log("Info:" + msg);
                     break;

             }
         });
        client.StartAsClient(SrvCfg.srvIP, SrvCfg.srvPort);
        PECommon.Log("Client Connect");
    }
    public void SendMsg(GameMsg msg)
    {
        if (client.session != null)
        {
            client.session.SendMsg(msg);
        }
        else
        {
            GameRoot.AddTips("服务器未连接");
            InitSvc();
        }
    }

    public void AddNetPkg(GameMsg msg)
    {
        lock (obj)
        {
            msgQue.Enqueue(msg);
        }
    }

    private void Update()
    {
        if (msgQue.Count > 0)
        {
            lock (obj)
            {
                GameMsg msg = msgQue.Dequeue();
                ProcessMsg(msg);
            }
        }
    }
    private void ProcessMsg(GameMsg msg)
    {
        if ((ErrorCode)msg.err != ErrorCode.None)
        {
            switch ((ErrorCode)msg.err)
            {
                case ErrorCode.AcctIsOnline:
                    GameRoot.AddTips("账号已经上线");
                    break;
                case ErrorCode.WrongPass:
                    GameRoot.AddTips("密码错误");
                    break;
                case ErrorCode.NameIsExist:
                    GameRoot.AddTips("名字已经存在");
                    break;
                case ErrorCode.UpdateDBError:
                    PECommon.Log("更新数据库出错", LogType.Error);
                    GameRoot.AddTips("网络不稳定");
                    break;
                case ErrorCode.LackCoin:
                    GameRoot.AddTips("金币不足");
                    break;
                case ErrorCode.LackCrystal:
                    GameRoot.AddTips("水晶不足");
                    break;
                case ErrorCode.LackLv:
                    GameRoot.AddTips("等级不足");
                    break;
                case ErrorCode.LackDiamond:
                    GameRoot.AddTips("钻石不足");
                    break;
                case ErrorCode.LackPower:
                    GameRoot.AddTips("体力不足");
                    break;
                case ErrorCode.ClientDataError:
                    GameRoot.AddTips("客户端数据异常");
                    break;

            }
            return;
        }
        switch ((CMD)msg.cmd)
        {
            case CMD.RspLogin:
                LoginSys.Instance.RspLogin(msg);
                break;
            case CMD.RspRename:
                LoginSys.Instance.RspRename(msg);
                break;
            case CMD.RspGuide:
                MainCitySys.Instance.RspGuide(msg);
                break;
            case CMD.RspStrong:
                MainCitySys.Instance.RspStrong(msg);
                break;
            case CMD.PshChat:
                MainCitySys.Instance.PshChat(msg);
                break;
            case CMD.RspBuy:
                MainCitySys.Instance.RspBuy(msg);
                break;
            case CMD.PshPower:
                MainCitySys.Instance.PshPower(msg);
                break;
            case CMD.PshTaskPrg:
                MainCitySys.Instance.PshTaskPrg(msg);
                break;
            case CMD.RspTakeTask:
                MainCitySys.Instance.RspTakeTask(msg);
                break;
            case CMD.RspFubenFight:
                FubenSys.Instance.RspFubenFight(msg);
                break;
            case CMD.RspFubenFightEnd:
                BattleSys.Instance.RspFubenFightEnd(msg);
                break;
        }
    }
}