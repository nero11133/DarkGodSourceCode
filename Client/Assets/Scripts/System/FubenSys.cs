/****************************************************
    文件：FubenSys.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/15 16:39:42
	功能：副本业务系统
*****************************************************/

using UnityEngine;
using PEProtocol;

public class FubenSys : SystemRoot 
{
    public static FubenSys Instance;

    public FubenWnd fubenWnd;

    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init FubenSys");
    }

    #region FubenWnd
    public void SetFubenWndState(bool state=true)
    {
        fubenWnd.SetWndowState(state);
    }

    public void RspFubenFight(GameMsg msg)
    {
        RspFubenFight data = msg.rspFubenFight;
        GameRoot.Instance.SetPlayerDataByFBStart(data);
        SetFubenWndState(false);
        MainCitySys.Instance.mainCityWnd.SetWndowState(false);
        BattleSys.Instance.StartBattle(data.fbid);
    }
    #endregion
}