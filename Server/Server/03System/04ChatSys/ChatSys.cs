/****************************************************
	文件：ChatSys.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/08 15:42   	
	功能：聊天系统
*****************************************************/
using PEProtocol;
using System.Collections.Generic;

public class ChatSys
{
    private static ChatSys instance;
    public static ChatSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChatSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("ChatSys Init Done");
    }

    public void SndChat(MsgPack pack)
    {
        SndChat data = pack.msg.sndChat;
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PshChat,
            pshChat = new PshChat
            {
                name = pd.name,
                chat = data.chat
            }
        };
        //更新任务进度
        TaskSys.Instance.CalcTaskPrg(pd, 6);
        //广播所有在线客户端
        List<ServerSession> list = cacheSvc.GetOnLineSessions();
        byte[] bytes = PENet.PETool.PackNetMsg(msg);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SendMsg(bytes);
        }
    }
}

