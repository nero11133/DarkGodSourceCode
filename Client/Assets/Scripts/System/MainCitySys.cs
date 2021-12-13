/****************************************************
    文件：MainCitySys.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/5/4 16:38:41
	功能：主城业务系统
*****************************************************/

using UnityEngine;
using UnityEngine.AI;
using PEProtocol;

public class MainCitySys : SystemRoot
{
    public static MainCitySys Instance;

    public MainCityWnd mainCityWnd;
    public InfoWnd infoWnd;
    public GuideWnd guideWnd;
    public StrongWnd strongWnd;
    public ChatWnd chatWnd;
    public BuyWnd buyWnd;
    public TaskWnd taskWnd;

    public GameQuitWnd gameQuitWnd;

    private PlayerController playerCtrl;
    private Transform charShowCamTrans;

    private float startRotate = 0;
    private AutoGuideCfg curtTask;
    private Transform[] npcTrans;
    private NavMeshAgent nav;

    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init MainCitySys");
    }

    public void EnterMainCity()
    {
        //加载主城场景
        MapCfg mapData = resSvc.GetMapCfg(Constants.SceneMainCityId);
        resSvc.AsyncLoadScene(mapData.sceneName, () =>
        {
            //打开主城ui
            mainCityWnd.SetWndowState();
            //加载游戏角色
            LoadPlayer(mapData);
            //加载主城bgm
            audioSvc.PlayBGM(Constants.BGMainCity);

            GameRoot.Instance.GetComponent<AudioListener>().enabled = false;

            GameObject mapRoot = GameObject.FindGameObjectWithTag("MapRoot");
            MainCityMap mainCityMap = mapRoot.GetComponent<MainCityMap>();
            npcTrans = mainCityMap.npcTrans;

            //人物展示相机
            if (charShowCamTrans != null)
            {
                charShowCamTrans.gameObject.SetActive(false);
            }
        });

    }

    private void LoadPlayer(MapCfg mapData)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssassinCityPlayerPrefab, true);
        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        //相机初始化
        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();

        nav = player.GetComponent<NavMeshAgent>();
    }

    public void SetPlyerDir(Vector2 dir)
    {
        
        if (dir == Vector2.zero)
        {
            if (isNavGuide == false)
            {
                playerCtrl.SetBlend(Constants.BlendIdle);
            }
        }
        else
        {
            playerCtrl.SetBlend(Constants.BlendMove);
            StopNavTask();
        }
        playerCtrl.Dir = dir;
    }

    #region InfoWnd
    public void OpenInfoWnd()
    {
        StopNavTask();
        infoWnd.SetWndowState();
        //设置人物展示相机
        if (charShowCamTrans == null)
        {
            charShowCamTrans = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }
        charShowCamTrans.localPosition = playerCtrl.transform.position + playerCtrl.transform.forward * 3.8f + new Vector3(0, 1.2f, 0);
        charShowCamTrans.localEulerAngles = new Vector3(0, 180 + playerCtrl.transform.localEulerAngles.y, 0);
        charShowCamTrans.localScale = Vector3.one;
        charShowCamTrans.gameObject.SetActive(true);
    }

    public void CloseInfoWnd()
    {
        if (charShowCamTrans != null)
        {
            charShowCamTrans.gameObject.SetActive(false);
            infoWnd.SetWndowState(false);
        }
    }

    public void SetStartRotate()
    {
        startRotate = playerCtrl.transform.localEulerAngles.y;
    }
    public void SetPlayerRotate(float rorate)
    {
        playerCtrl.transform.localEulerAngles = new Vector3(0, startRotate + rorate, 0);
    }
    #endregion

    #region StrongWnd
    public void OpenStrongWnd()
    {
        StopNavTask();
        strongWnd.SetWndowState();
    }
    #endregion

    #region ChatWnd
    public void OpenChatWnd()
    {
        StopNavTask();
        chatWnd.SetWndowState();
    }

    public void PshChat(GameMsg msg)
    {
        chatWnd.AddChatMsg(msg.pshChat.name, msg.pshChat.chat);
    }
    #endregion

    #region BuyWnd
    public void OpenBuyWnd(int type)
    {
        StopNavTask();
        buyWnd.SetBuyType(type);
        buyWnd.SetWndowState();
    }
    public void RspBuy(GameMsg msg)
    {
        RspBuy data = msg.rspBuy;
        GameRoot.Instance.SetPlayerDataByBuy(data);
        mainCityWnd.RefreshUI(GameRoot.Instance.PlayerData);
        GameRoot.AddTips("购买成功!");
        buyWnd.SetWndowState(false);
        if (msg.pshTaskPrg != null)
        {
            GameRoot.Instance.SetPlayerDataByTaskPrg(msg.pshTaskPrg);
            if (taskWnd.GetWndState())
            {
                taskWnd.RefreshUI();
            }
        }
    }
    public void PshPower(GameMsg msg)
    {
        PshPower data = msg.pshPower;
        GameRoot.Instance.SetPlayerDataByPower(data);
        if (mainCityWnd.GetWndState())
        {
            mainCityWnd.RefreshUI(GameRoot.Instance.PlayerData);
        }
    }
    #endregion

    #region TaskWnd
    public void OpenTaskWnd()
    {
        StopNavTask();
        taskWnd.SetWndowState();
    }
    public void RspTakeTask(GameMsg msg)
    {
        RspTakeTask data = msg.rspTakeTask;
        GameRoot.Instance.SetPlayerDataByTask(data);
        taskWnd.RefreshUI();
        mainCityWnd.RefreshUI(GameRoot.Instance.PlayerData);
    }
    public void PshTaskPrg(GameMsg msg)
    {
        PshTaskPrg data = msg.pshTaskPrg;
        GameRoot.Instance.SetPlayerDataByTaskPrg(data);
        if (taskWnd.GetWndState())
        {
            taskWnd.RefreshUI();
        }
    }
    #endregion

    #region Fuben
    public void EnterFuben()
    {
        StopNavTask();
        FubenSys.Instance.SetFubenWndState();
    }
    #endregion

    #region Guide
    private bool isNavGuide = false;
    public void RunTask(AutoGuideCfg guideCfg)
    {
        if (guideCfg != null)
        {
            curtTask = guideCfg;
        }
        //解析任务数据
        if (curtTask.npcId != -1)
        {
            nav.enabled = true;
            float distance = Vector3.Distance(playerCtrl.transform.position, npcTrans[curtTask.npcId].position);
            if (distance < 0.5f)
            {
                isNavGuide = false;
                nav.isStopped = true;
                nav.enabled = false;
                playerCtrl.SetBlend(Constants.BlendIdle);
                OpenGuideWnd();
            }
            else
            {
                isNavGuide = true;
                nav.enabled = true;
                nav.speed = Constants.PlayerMoveSpeed;
                nav.SetDestination(npcTrans[curtTask.npcId].position);
                playerCtrl.SetBlend(Constants.BlendMove);
            }
        }
        else
        {
            OpenGuideWnd();
        }
    }

    private void OpenGuideWnd()
    {

        //Debug.Log("OpenGuideWnd");
        guideWnd.SetWndowState();
    }

    private void StopNavTask()
    {
        if (isNavGuide)
        {
            isNavGuide = false;
            nav.isStopped = true;
            nav.enabled = false;
            playerCtrl.SetBlend(Constants.BlendIdle);
        }
    }

    private void IsArriveNavPos()
    {
        float distance = Vector3.Distance(playerCtrl.transform.position, npcTrans[curtTask.npcId].position);
        if (distance < 0.5f)
        {
            isNavGuide = false;
            nav.isStopped = true;
            nav.enabled = false;
            playerCtrl.SetBlend(Constants.BlendIdle);
            OpenGuideWnd();
        }
    }

    public AutoGuideCfg GetGuideCfg()
    {
        return curtTask;
    }

    public void RspGuide(GameMsg msg)
    {
        RspGuide data = msg.rspGuide;
        GameRoot.AddTips(Constants.Color("任务奖励 金币：" + curtTask.coin + "  经验：" + curtTask.exp, TxtColor.Blue));
        switch (curtTask.actId)
        {
            case 0:
                //与智者对话
                break;
            case 1:
                //进入副本
                EnterFuben();
                break;
            case 2:
                //进入强化界面
                OpenStrongWnd();
                break;
            case 3:
                //进入体力购买
                OpenBuyWnd(0);
                break;
            case 4:
                //进入金币铸造
                OpenBuyWnd(1);
                break;
            case 5:
                //进入世界聊天界面
                OpenChatWnd();
                break;

        }
        GameRoot.Instance.SetPlayerDataByRspGuide(data);
        mainCityWnd.RefreshUI(GameRoot.Instance.PlayerData);
    }
    #endregion

    public void RspStrong(GameMsg msg)
    {
        int zhanliPre = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.Instance.SetPlayerDataByRspStrong(msg.rspStrong);
        int zhanliNow = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.AddTips(Constants.Color("战力提升" + (zhanliNow - zhanliPre), TxtColor.Blue));
        strongWnd.UpdateUI();
        mainCityWnd.RefreshUI(GameRoot.Instance.PlayerData);
    }

    public void OpenQuitWnd()
    {
        gameQuitWnd.SetWndowState();
    }

    private void Update()
    {
        if (isNavGuide)
        {
            playerCtrl.SetCam();
            IsArriveNavPos();
        }
    }
}