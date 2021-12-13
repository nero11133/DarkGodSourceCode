/****************************************************
    文件：GuideWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/5/19 20:23:29
	功能：引导对话界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class GuideWnd : WindowRoot 
{
    public Text txtName;
    public Text txtTalk;
    public Image imgIcon;

    private AutoGuideCfg guideCfg;
    private PlayerData playerData;
    private int index;
    private string[] dilogArr;

    protected override void InitWnd()
    {
        base.InitWnd();
        guideCfg = MainCitySys.Instance.GetGuideCfg();
        playerData = GameRoot.Instance.PlayerData;
        index = 1;
        dilogArr = guideCfg.dilogArr.Split('#');
        SetTalk();
    }

    private void SetTalk()
    {
        string[] talkArr = dilogArr[index].Split('|');
        if (talkArr[0] == "0")
        {
            //自己
            SetText(txtName, playerData.name);
            SetSprite(imgIcon, PathDefine.SelfIcon);
        }
        else
        {
            //对话npc
            switch (guideCfg.npcId)
            {
                case 0:
                    SetText(txtName, "智者");
                    SetSprite(imgIcon, PathDefine.WiseManIcon);
                    break;
                case 1:
                    SetText(txtName, "将军");
                    SetSprite(imgIcon, PathDefine.GeneralIcon);
                    break;
                case 2:
                    SetText(txtName, "工匠");
                    SetSprite(imgIcon, PathDefine.ArtisanIcon);
                    break;
                case 3:
                    SetText(txtName, "商人");
                    SetSprite(imgIcon, PathDefine.TraderIcon);
                    break;
                default:
                    SetText(txtName, "裂开公子");
                    SetSprite(imgIcon, PathDefine.GuideIcon);
                    break;
            }
        }
        SetText(txtTalk, talkArr[1].Replace("$name", playerData.name));
        imgIcon.SetNativeSize();
    }

    public void ClickNextBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        index++;
        if (index == dilogArr.Length)
        {
            //发送任务引导完成信息
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqGuide,
                reqGuide = new ReqGuide
                {
                    guideId = guideCfg.id
                }
            };
            netSvc.SendMsg(msg);
            SetWndowState(false);
        }
        else
        {
            SetTalk();
        }
    }
}