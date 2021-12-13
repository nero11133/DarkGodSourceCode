/****************************************************
    文件：GameRoot.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/23 18:21:31
	功能：游戏启动入口
*****************************************************/

using UnityEngine;
using PEProtocol;

public class GameRoot : MonoBehaviour 
{
    public static GameRoot Instance;

    public LoadingWnd loadingWnd;
    public DynamicWnd dynamicWnd;

    private PlayerData playerData;
    public PlayerData PlayerData
    {
        get { return playerData; }
    }

    public void SetPlayerData(PlayerData data)
    {
        playerData = data;
    }

    public void SetPlayerDataByRspGuide(RspGuide data)
    {
        playerData.lv = data.lv;
        playerData.coin = data.coin;
        playerData.exp = data.exp;
        playerData.guideId = data.guideId;
    }

    public void SetPlayerName(string name)
    {
        playerData.name = name;
    }

    public void SetPlayerDataByRspStrong(RspStrong data)
    {
        playerData.coin = data.coin;
        playerData.crystal = data.crtstal;
        playerData.hp = data.hp;
        playerData.ad = data.ad;
        playerData.ap = data.ap;
        playerData.addef = data.addef;
        playerData.apdef = data.apdef;
        playerData.strongArr = data.strongArr;
    }

    public void SetPlayerDataByBuy(RspBuy data)
    {
        playerData.coin = data.coin;
        playerData.diamond = data.diamond;
        playerData.power = data.power;
    }

    public void SetPlayerDataByPower(PshPower data)
    {
        PlayerData.power = data.power;
    }

    public void SetPlayerDataByTask(RspTakeTask data)
    {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.taskArr = data.taskArr;
    }

    public void SetPlayerDataByTaskPrg(PshTaskPrg data)
    {
        PlayerData.taskArr = data.taskArr;
    }

    public void SetPlayerDataByFBStart(RspFubenFight data)
    {
        PlayerData.power = data.power;
    }

    public void SetPlayerDataByFBEnd(RspFubenFightEnd data)
    {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        playerData.crystal = data.crystal;
        playerData.fuben = data.fuben;
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        PECommon.Log("Game Start...");
        ClearUIRoot();
        Init();
    }

    private void ClearUIRoot()
    {
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
        
    }

    private void Init()
    {
        //服务模块初始化
        NetSvc netSvc = GetComponent<NetSvc>();
        netSvc.InitSvc();
        ResSvc resSvc = GetComponent<ResSvc>();
        resSvc.InitSvc();

        AudioSvc audioSvc = GetComponent<AudioSvc>();
        audioSvc.InitSvc();
        TimerSvc timerSvc = GetComponent<TimerSvc>();
        timerSvc.InitSvc();

        //业务系统初始化
        LoginSys loginSys = GetComponent<LoginSys>();
        loginSys.InitSys();
        MainCitySys mainCitySys = GetComponent<MainCitySys>();
        mainCitySys.InitSys();
        FubenSys fubenSys = GetComponent<FubenSys>();
        fubenSys.InitSys();
        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();

        dynamicWnd.SetWndowState();

        //进入登录界面并加载相应UI
        loginSys.EnterLogin();

        
    }

    public static void AddTips(string tips)
    {
        Instance.dynamicWnd.AddTips(tips);
    }
}