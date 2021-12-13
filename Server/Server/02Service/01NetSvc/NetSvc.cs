/****************************************************
	文件：NetSvc.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/04/27 16:27   	
	功能：网络服务
*****************************************************/

using PENet;
using PEProtocol;
using System.Collections.Generic;

public class MsgPack
{
    public ServerSession session;
    public GameMsg msg;
    public MsgPack(ServerSession session,GameMsg msg)
    {
        this.session = session;
        this.msg = msg;
    }
}

public class NetSvc
{
    private static NetSvc instance;
    public static NetSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetSvc();
            }
            return instance;
        }
    }

    private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();
    private static readonly string obj = "lock";

    public void Init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SrvCfg.srvIP, SrvCfg.srvPort);
        PECommon.Log("NetSvc Init Done");
    }
    public void AddMsg(MsgPack pack)
    {
        lock (obj)
        {
            msgPackQue.Enqueue(pack);
        }
    }
    public void Update()
    {
        if (msgPackQue.Count > 0)
        {
            //PECommon.Log("PackCount:" + msgPackQue.Count);
            lock (obj)
            {
                MsgPack pack = msgPackQue.Dequeue();
                HandOutMsg(pack);
            }
        }
    }
    public void HandOutMsg(MsgPack pack)
    {
        switch ((CMD)pack.msg.cmd)
        {
            case CMD.None:
                break;
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(pack);
                break;
            case CMD.ReqGuide:
                GuideSys.Instance.ReqGuide(pack);
                break;
            case CMD.ReqStrong:
                StrongSys.Instance.ReqStrong(pack);
                break;
            case CMD.SndChat:
                ChatSys.Instance.SndChat(pack);
                break;
            case CMD.ReqBuy:
                BuySys.Instance.ReqBuy(pack);
                break;
            case CMD.ReqTakeTask:
                TaskSys.Instance.ReqTakeTask(pack);
                break;
            case CMD.ReqFubenFight:
                FubenSys.Instance.ReqFubenFight(pack);
                break;
            case CMD.ReqFubenFightEnd:
                FubenSys.Instance.ReqFubenFightEnd(pack);
                break;
        }
    }
}

