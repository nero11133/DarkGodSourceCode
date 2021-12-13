/****************************************************
    文件：BattleEndWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/7/23 19:24:55
	功能：战斗结束窗口
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class BattleEndWnd : WindowRoot 
{
    private FBEndType endType = FBEndType.None;

    #region UI Define
    public Transform transReward;
    public Button btnExit;
    public Button btnClose;
    public Text txtTime;
    public Text txtRestHp;
    public Text txtReward;
    #endregion

    protected override void InitWnd()
    {
        base.InitWnd();
        RefreshUI();
    }

    private void RefreshUI()
    {
        switch (endType)
        {
            case FBEndType.Pause:
                SetActive(transReward, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject);
                break;
            case FBEndType.Win:
                SetActive(transReward, false);
                SetActive(btnExit.gameObject,false);
                SetActive(btnClose.gameObject, false);

                MapCfg mapCfg = resSvc.GetMapCfg(fbId);
                int coin = mapCfg.coin;
                int exp = mapCfg.exp;
                int crystal = mapCfg.crystal;
                int min = costTime / 60;
                int sec = costTime % 60;
                SetText(txtTime, "通关时间: " + min + ":" + sec);
                SetText(txtRestHp, "剩余血量: " + restHp);
                SetText(txtReward, "通关奖励: " + Constants.Color(coin + "金币", TxtColor.Yellow) + " " + Constants.Color(exp + "经验", TxtColor.Green) + " " + Constants.Color(crystal + "水晶", TxtColor.Blue));
                timerSvc.AddTimeTask((int tid) =>
                {
                    SetActive(transReward);
                    timerSvc.AddTimeTask((int tid1) =>
                    {
                        audioSvc.PlayUIAudio(Constants.AudioFBItemEnter);
                        timerSvc.AddTimeTask((int tid2) =>
                        {
                            audioSvc.PlayUIAudio(Constants.AudioFBItemEnter);
                            timerSvc.AddTimeTask((int tid3) =>
                            {
                                audioSvc.PlayUIAudio(Constants.AudioFBItemEnter);
                                timerSvc.AddTimeTask((int tid4) =>
                                {
                                    audioSvc.PlayUIAudio(Constants.FBWin);
                                }, 0.3);
                            }, 0.3);
                        }, 0.3);
                    }, 0.3);
                }, 1);
                break;
            case FBEndType.Lose:
                SetActive(transReward, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject,false);
                audioSvc.PlayUIAudio(Constants.FBLose);
                break;
        }
    }

    public void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        BattleSys.Instance.battleMgr.isGamePause = false;
        SetWndowState(false);
        //Time.timeScale = 1;
        
    }

    public void ClickExitBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        //进入主城，销毁当前战斗
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
        //Time.timeScale = 1;
    }

    public void ClickSureBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        //进入主城，销毁当前战斗
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
        // 进入副本界面
        FubenSys.Instance.SetFubenWndState();
    }

    public void SetFBEndType(FBEndType fBEndType)
    {
        endType = fBEndType;
    }

    private int fbId;
    private int costTime;
    private int restHp;
    public void SetBattleEndData(int fbId,int costTime,int restHp)
    {
        this.fbId = fbId;
        this.costTime = costTime;
        this.restHp = restHp;
    }
}
public enum FBEndType
{
    None,
    Pause,
    Win,
    Lose,
}