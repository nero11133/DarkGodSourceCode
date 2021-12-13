/****************************************************
    文件：FubenWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/15 16:37:55
	功能：副本选择界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class FubenWnd : WindowRoot 
{
    public Button[] fbBtnArr;
    public Transform pointerTrans;
    private PlayerData pd;

    protected override void InitWnd()
    {
        base.InitWnd();
        pd = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    public void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        SetWndowState(false);
    }

    public void ClickFubenBtn(int fbid)
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        MapCfg mapCfg = resSvc.GetMapCfg(fbid);
        int power = mapCfg.power;
        if (pd.power < power)
        {
            GameRoot.AddTips("体力不足");
        }
        else
        {
            netSvc.SendMsg(new GameMsg
            {
                cmd = (int)CMD.ReqFubenFight,
                reqFubenFight = new ReqFubenFight
                {
                    fbid = fbid
                }
            });
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < fbBtnArr.Length; i++)
        {
            if (i < pd.fuben%10000)
            {
                SetActive(fbBtnArr[i].gameObject);
                if (i == pd.fuben % 10000 - 1)
                {
                    pointerTrans.SetParent(fbBtnArr[i].transform);
                    pointerTrans.localPosition = new Vector3(25, 100, 0);
                }
            }
            else
            {
                SetActive(fbBtnArr[i].gameObject, false);
            }
        }
    }
}