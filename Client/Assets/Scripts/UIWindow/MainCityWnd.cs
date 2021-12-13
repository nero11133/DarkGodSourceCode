/****************************************************
    文件：MainCityWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/5/4 16:37:13
	功能：主城UI界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using UnityEngine.EventSystems;

public class MainCityWnd : WindowRoot 
{
    #region UIDefine
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;

    public Text txtFight;
    public Text txtPower;
    public Text txtLv;
    public Text txtName;
    public Text txtExpPrg;

    public Image imgPowerPrg;

    public Transform expPrgTrans;

    public Animation animMenu;

    public Button btnMenu;
    public Button btnGuide;
    #endregion

    private bool MenuState = true;
    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos = Vector2.zero;
    private float pointDis;

    private AutoGuideCfg curtTaskData = null;

    #region MainFun
    protected override void InitWnd()
    {
        base.InitWnd();
        defaultPos = imgDirBg.transform.position;
        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        SetActive(imgDirPoint, false);
        RefreshUI(GameRoot.Instance.PlayerData);
        RegisterTouchEvts();
    }

    public void RefreshUI(PlayerData playerData)
    {
        SetText(txtFight, PECommon.GetFightByProps(playerData));
        SetText(txtPower, "体力:" + playerData.power + "/" + PECommon.GetPowerLimit(playerData.lv));
        SetText(txtLv, playerData.lv);
        SetText(txtName, playerData.name);
        imgPowerPrg.fillAmount = playerData.power * 1.0f / PECommon.GetPowerLimit(playerData.lv);
        
        #region expprg
        int expPrgValue = (int)(playerData.exp * 1.0f / PECommon.GetExpUpValueBylv(playerData.lv) * 100);
        SetText(txtExpPrg, expPrgValue + "%");
        int index = expPrgValue / 10;
        for (int i = 0; i < expPrgTrans.childCount; i++)
        {
            Image image = expPrgTrans.GetChild(i).GetComponent<Image>();
            if (i < index)
            {
                image.fillAmount = 1;
            }
            else if (i == index)
            {
                image.fillAmount = expPrgValue % 10 / 10 * 1.0f;
            }
            else
            {
                image.fillAmount = 0;
            }
        }
        GridLayoutGroup gridLayoutGroup = expPrgTrans.GetComponent<GridLayoutGroup>();
        float globalRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;
        float screenWidth = globalRate * Screen.width;
        float width = (screenWidth - 180) / 10;
        gridLayoutGroup.cellSize = new Vector2(width, 7);
        #endregion

        //设置自动任务图标
        curtTaskData = resSvc.GetGuideTaskCfg(playerData.guideId);
        if (curtTaskData != null)
        {
            SetGuideBtnIcon(curtTaskData.npcId);
        }
        else
        {
            SetGuideBtnIcon(-1);
        }
    }

    private void SetGuideBtnIcon(int npcId)
    {
        string spPath = "";
        Image image = btnGuide.image;
        switch (npcId)
        {
            case Constants.NPCWiseMan:
                spPath = PathDefine.WiseManHead;
                break;
            case Constants.NPCGeneral:
                spPath = PathDefine.GeneralHead;
                break;
            case Constants.NPCArtisan:
                spPath = PathDefine.ArtisanHead;
                break;
            case Constants.NPCTrader:
                spPath = PathDefine.TraderHead;
                break;
            default:
                spPath = PathDefine.TaskHead;
                break;
        }
        SetSprite(image, spPath);
    }
    #endregion

    #region ClickEvts

    public void ClickFubenBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIOpenPage);
        MainCitySys.Instance.EnterFuben();
    }

    public void ClickTaskBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIOpenPage);
        MainCitySys.Instance.OpenTaskWnd();
    }

    public void ClickBuyPowerBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(0);
    }

    public void ClickMKCoinBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(1);
    }

    public void ClickMenuBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIExtenBtn);
        MenuState = !MenuState;
        AnimationClip animationClip = null;
        if (MenuState)
        {
            animationClip = animMenu.GetClip("OpenMCMenu");
        }
        else
        {
            animationClip = animMenu.GetClip("CloseMCMenu");
        }
        animMenu.Play(animationClip.name);
    }
    
    public void ClickHeadBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIOpenPage);
        MainCitySys.Instance.OpenInfoWnd();
    }

    public void ClickGuideBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        if (curtTaskData != null)
        {
            MainCitySys.Instance.RunTask(curtTaskData);
        }
        else
        {
            GameRoot.AddTips("更多引导任务，还在开发中。。。");
        }
    }

    public void ClickStrongBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIOpenPage);
        MainCitySys.Instance.OpenStrongWnd();
    }

    public void ClickChatBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        MainCitySys.Instance.OpenChatWnd();
    }

    public void RegisterTouchEvts()
    {
        OnClickDown(imgTouch.gameObject, (PointerEventData eventData) =>
        {
            imgDirBg.transform.position = eventData.position;
            startPos = eventData.position;
            SetActive(imgDirPoint);
        });
        OnClickUp(imgTouch.gameObject, (PointerEventData eventData) =>
        {
            imgDirBg.transform.position = defaultPos;
            imgDirPoint.transform.localPosition = Vector2.zero;
            SetActive(imgDirPoint, false);
            //方向信息传递
            MainCitySys.Instance.SetPlyerDir(Vector2.zero);
        });
        OnDrag(imgTouch.gameObject, (PointerEventData eventData) =>
        {
            Vector2 dir = eventData.position - startPos;
            float len = dir.magnitude;
            if (len > pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = eventData.position;
            }
            //方向信息传递
            MainCitySys.Instance.SetPlyerDir(dir.normalized);
        });
    }
    #endregion

    public void ClickQuitBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        MainCitySys.Instance.OpenQuitWnd();
    }

    private void Update()
    {
        //float x = Input.GetAxisRaw("Horizontal");
        //float y = Input.GetAxisRaw("Vertical");
        //if (x == 0 && y == 0)
        //{
        //    MainCitySys.Instance.SetPlyerDir(Vector2.zero);
        //}
        //else
        //{
        //    MainCitySys.Instance.SetPlyerDir(new Vector2(x, y));
        //}
    }
}