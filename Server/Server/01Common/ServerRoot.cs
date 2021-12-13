/****************************************************
	文件：ServerRoot.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/04/27 16:21   	
	功能：服务器初始化
*****************************************************/


public class ServerRoot
{
    private static ServerRoot instance;
    public static ServerRoot Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ServerRoot();
            }
            return instance;
        }
    }

    public void Init()
    {
        //数据层
        DBMgr.Instance.Init();

        //服务层
        NetSvc.Instance.Init();
        CacheSvc.Instance.Init();
        CfgSvc.Instance.Init();
        TimerSvc.Instance.Init();

        //业务系统层
        LoginSys.Instance.Init();
        GuideSys.Instance.Init();
        StrongSys.Instance.Init();
        ChatSys.Instance.Init();
        BuySys.Instance.Init();
        PowerSys.Instance.Init();
        TaskSys.Instance.Init();
        FubenSys.Instance.Init();
      
    }
    public void Update()
    {
        NetSvc.Instance.Update();
        TimerSvc.Instance.Update();
    }

    private int sessionId = 0;
    public int GetSessionId()
    {
        if (sessionId == int.MaxValue)
        {
            sessionId = 0;
        }
        return sessionId++;
    }
}

