/****************************************************
    文件：ChatWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/4 16:34:36
	功能：聊天界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PEProtocol;

public class ChatWnd : WindowRoot 
{
    public Image imgWorld;
    public Image imgGuild;
    public Image imgFriend;
    public Text txtChat;
    public InputField iptChat;

    private int chatType;
    private List<string> chatList = new List<string>();

    protected override void InitWnd()
    {
        base.InitWnd();
        chatType = 0;
        RefreshUI();
    }

    public void AddChatMsg(string name,string chat)
    {
        chatList.Add(Constants.Color(name + "：", TxtColor.Blue) + chat);
        if (chatList.Count > 12)
        {
            chatList.RemoveAt(0);
        }
        if (GetWndState())
        {
            RefreshUI();
        }
        
    }

    public void RefreshUI()
    {
        if (chatType == 0)
        {
            string chatMsg = "";
            for (int i = 0; i < chatList.Count; i++)
            {
                chatMsg += chatList[i] + "\n";
            }
            SetText(txtChat, chatMsg);
            SetSprite(imgWorld, "ResImages/btntype1");
            SetSprite(imgGuild, "ResImages/btntype2");
            SetSprite(imgFriend, "ResImages/btntype2");
        }
        else if (chatType == 1)
        {
            SetText(txtChat, "尚未加入公会");
            SetSprite(imgWorld, "ResImages/btntype2");
            SetSprite(imgGuild, "ResImages/btntype1");
            SetSprite(imgFriend, "ResImages/btntype2");
        }
        else if (chatType == 2)
        {
            SetText(txtChat, "尚未好友信息");
            SetSprite(imgWorld, "ResImages/btntype2");
            SetSprite(imgGuild, "ResImages/btntype2");
            SetSprite(imgFriend, "ResImages/btntype1");
        }
    }

    public void ClickWorldBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        chatType = 0;
        RefreshUI();
    }

    public void ClickGuildBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        chatType = 1;
        RefreshUI();
    }

    public void ClickFriendBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        chatType = 2;
        RefreshUI();
    }

    private bool canSend = true;
    public void ClickSendBtn()
    {
        if (canSend == false)
        {
            GameRoot.AddTips("5秒钟内只能发送一次消息");
            return;
        }
        if(iptChat.text!=null&&iptChat.text!=""&&iptChat.text!=" ")
        {
            if (iptChat.text.Length > 12)
            {
                GameRoot.AddTips("输入的信息不能超过12个字符");
            }
            else
            {
                //发送网络消息
                GameMsg msg = new GameMsg
                {
                    cmd = (int)CMD.SndChat,
                    sndChat = new SndChat
                    {
                        chat = iptChat.text
                    }
                };
                netSvc.SendMsg(msg);
                canSend = false;
                iptChat.text = "";
                timerSvc.AddTimeTask((int tid) =>
                {
                    canSend = true;
                }, 5, PETimeUnit.Second);
            }
        }
        else
        {
            GameRoot.AddTips("尚未输入聊天信息");
        }
    }

    public void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        SetWndowState(false);
    }
}