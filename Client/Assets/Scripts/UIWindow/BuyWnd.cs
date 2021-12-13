/****************************************************
    文件：BuyWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/8 15:43:19
	功能：购买窗口
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class BuyWnd : WindowRoot 
{
    private int buyType;//0:购买体力  1:购买金币
    public Text txtInfo;
    public Button btnSure;

    protected override void InitWnd()
    {
        base.InitWnd();
        btnSure.interactable = true;
        RefreshUI();
    }

    private void RefreshUI()
    {
        switch (buyType)
        {
            case 0:
                txtInfo.text = "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" + Constants.Color("100体力", TxtColor.Green) + "?";
                break;
            case 1:
                txtInfo.text = "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" + Constants.Color("1000金币", TxtColor.Yellow) + "?";
                break;

        }
    }

    public void SetBuyType(int type)
    {
        buyType = type;
    }

    public void ClickSureBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        //发送网络消息 
        GameMsg msg = new GameMsg()
        {
            cmd = (int)CMD.ReqBuy,
            reqBuy = new ReqBuy()
            {
                buyType = this.buyType,
                cost = 10
            }
        };
        netSvc.SendMsg(msg);
        btnSure.interactable = false;
    }

    public void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        SetWndowState(false);
    }
}