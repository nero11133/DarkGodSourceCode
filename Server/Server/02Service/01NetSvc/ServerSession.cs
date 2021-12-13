/****************************************************
	文件：ServerSession.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/04/27 17:55   	
	功能：网络会话链接
*****************************************************/

using PENet;
using PEProtocol;

public class ServerSession : PESession<GameMsg>
{
    public int sessionId = 0;

    protected override void OnConnected()
    {
        sessionId = ServerRoot.Instance.GetSessionId();
        PECommon.Log("Client Connect,SessionId:"+sessionId);
       
    }
    protected override void OnDisConnected()
    {
        LoginSys.Instance.ClearOffLineData(this);
        PECommon.Log("Client DisConnect,SessionId:"+sessionId);
    }
    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("Client Rsp: CMD:"+((CMD)msg.cmd).ToString()+"  SessionId:"+sessionId );
        NetSvc.Instance.AddMsg(new MsgPack(this, msg));
        
    }
}

