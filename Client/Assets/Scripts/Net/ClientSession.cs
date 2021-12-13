/****************************************************
    文件：ClientSession.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/27 19:13:54
	功能：网络通信协议
*****************************************************/

using UnityEngine;
using PENet;
using PEProtocol;

public class ClientSession : PESession<GameMsg> 
{
    protected override void OnConnected()
    {
        PECommon.Log("Server Connect");
        GameRoot.AddTips("服务器链接成功");
    }
    protected override void OnDisConnected()
    {
        PECommon.Log("Server DisConnect");
        GameRoot.AddTips("服务器断开链接");
    }
    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("Server Rsp: CMD:"+((CMD)msg.cmd).ToString() );
        NetSvc.Instance.AddNetPkg(msg);
    }
}