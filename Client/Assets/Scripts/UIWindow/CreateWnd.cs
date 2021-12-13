/****************************************************
    文件：CreateWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/26 16:59:30
	功能：角色创建界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class CreateWnd : WindowRoot
{
    public InputField iptName;

    protected override void InitWnd()
    {
        base.InitWnd();
        // 显示一个随机名字
        iptName.text = resSvc.GetRDName(false);
    }

    public void ClickRDNameBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        iptName.text = resSvc.GetRDName(false);
    }
    public void ClickEnterBtn()
    {
        if (iptName.text != "")
        {
            //todo发送名字数据到服务器，登录主城
            audioSvc.PlayUIAudio(Constants.AudioUIClick);
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqRename,
                reqRename = new ReqRename
                {
                    name = iptName.text
                }
            };
            netSvc.SendMsg(msg);
        }
        else
        {
            audioSvc.PlayUIAudio(Constants.AudioUIClick);
            GameRoot.AddTips("当前名字不符合规则");
        }
    }
}