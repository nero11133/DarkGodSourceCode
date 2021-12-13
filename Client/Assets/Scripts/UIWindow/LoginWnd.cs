/****************************************************
    文件：LoginWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/25 19:15:54
	功能：登录注册窗口界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class LoginWnd : WindowRoot 
{
    public InputField iptAcct;
    public InputField iptPass;
    public Button btnEnter;
    public Button btnNotice;

    protected override void InitWnd()
    {
        base.InitWnd();
        if (PlayerPrefs.HasKey("Acct") && PlayerPrefs.HasKey("Pass"))
        {
            iptAcct.text = PlayerPrefs.GetString("Acct");
            iptPass.text = PlayerPrefs.GetString("Pass");
        }
        else
        {
            iptAcct.text = "";
            iptPass.text = "";
        }
    }
    

    public void ClickEnterBtn()
    {
        string _acct = iptAcct.text;
        string _pass = iptPass.text;
        if (_acct != "" && _pass != "")
        {
            audioSvc.PlayUIAudio(Constants.AudioLoginBtn);
            PlayerPrefs.SetString("Acct", _acct);
            PlayerPrefs.SetString("Pass", _pass);
            //todo 发送网络消息，请求登录
            GameMsg msg = new GameMsg()
            {
                reqLogin = new ReqLogin()
                {
                    acct = _acct,
                    pass = _pass
                },
                cmd = (int)CMD.ReqLogin
            };
            netSvc.SendMsg(msg);
            ////toremove
            //LoginSys.Instance.RspLogin();
        }
        else
        {
            audioSvc.PlayUIAudio(Constants.AudioLoginBtn);
            GameRoot.AddTips("账号或密码为空！");
        }
    }
    public void ClickNoticeBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        GameRoot.AddTips("功能开发中。。。");
    }

    public void ClickExitBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        LoginSys.Instance.OpenQuitWnd();
    }
}