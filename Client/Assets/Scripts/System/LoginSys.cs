/****************************************************
    文件：LoginSys.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/23 18:24:22
	功能：登录注册业务系统
*****************************************************/

using UnityEngine;
using PEProtocol;

public class LoginSys : SystemRoot 
{
    public static LoginSys Instance;

    public LoginWnd loginWnd;
    public CreateWnd createWnd;
    public GameQuitWnd gameQuitWnd;

    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init LoginSys");
    }
    /// <summary>
    /// 进入登录场景
    /// </summary>
    public void EnterLogin()
    {
        //异步加载登录场景
        //并显示加载进度
        //加载完成后打开登录注册界面
        resSvc.AsyncLoadScene(Constants.SceneLogin, () => {
            loginWnd.SetWndowState();
            audioSvc.PlayBGM(Constants.BGLogin);
          
        });

    }

    public void RspLogin(GameMsg msg)
    {
        GameRoot.AddTips("登录成功！");
        GameRoot.Instance.SetPlayerData(msg.rspLogin.playerData);
        if (msg.rspLogin.playerData.name == "")
        {
            //打开角色创建界面
            createWnd.SetWndowState();
        }
        else
        {

            //进入主城
            MainCitySys.Instance.EnterMainCity();
        }
       
        //关闭登录界面
        loginWnd.SetWndowState(false);
    }

    public void RspRename(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerName(msg.rspRename.name);

        //登录到主城 打开主城界面
        MainCitySys.Instance.EnterMainCity();
        //关闭创建窗口
        createWnd.SetWndowState(false);
    }

    public void OpenQuitWnd()
    {
        gameQuitWnd.SetWndowState();
    }
}