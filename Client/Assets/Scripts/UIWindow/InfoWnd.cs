/****************************************************
    文件：InfoWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/5/14 20:15:1
	功能：角色信息展示界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PEProtocol;

public class InfoWnd : WindowRoot 
{
    #region UI Define
    public Text txtInfo;
    public Text txtExp;
    public Image imgExp;
    public Text txtPower;
    public Image imgPower;
    public RawImage imgCharShow;

    public Text txtJob;
    public Text txtFight;
    public Text txtHp;
    public Text txtHurt;
    public Text txtDef;

    public Text dtxtHp;
    public Text dtxtAd;
    public Text dtxtAp;
    public Text dtxtAddef;
    public Text dtxtApdef;
    public Text dtxtDodge;
    public Text dtxtPierce;
    public Text dtxtCritical;

    public Transform detailTrans;

    public Button btnClose;
    #endregion

    private Vector2 startPos;

    protected override void InitWnd()
    {
        base.InitWnd();
        RefreshUI();
        RegisterTouchEvts();
        SetActive(detailTrans, false);
    }

    private void RegisterTouchEvts()
    {
        OnClickDown(imgCharShow.gameObject, (PointerEventData data) =>
        {
            startPos = data.position;
            MainCitySys.Instance.SetStartRotate();
        });
        OnDrag(imgCharShow.gameObject, (PointerEventData data) =>
         {
             float rorate = -(startPos.x - data.position.x) * 0.4f;
             MainCitySys.Instance.SetPlayerRotate(rorate);
         });
    }

    private void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtInfo, pd.name + "LV." + pd.lv);
        SetText(txtExp, pd.exp + "/" + PECommon.GetExpUpValueBylv(pd.lv));
        imgExp.fillAmount = pd.exp * 1.0f / PECommon.GetExpUpValueBylv(pd.lv);
        SetText(txtPower, pd.power + "/" + PECommon.GetPowerLimit(pd.lv));
        imgPower.fillAmount = pd.power * 1.0f / PECommon.GetPowerLimit(pd.lv);
        SetText(txtJob, "职业     暗夜刺客");
        SetText(txtFight, "战力     " + PECommon.GetFightByProps(pd));
        SetText(txtHp, "血量     " + pd.hp);
        SetText(txtHurt, "伤害     " + (pd.ad + pd.ap));
        SetText(txtDef, "防御     " + (pd.addef + pd.apdef));

        SetText(dtxtHp, pd.hp);
        SetText(dtxtAd, pd.ad);
        SetText(dtxtAp, pd.ap);
        SetText(dtxtAddef, pd.addef);
        SetText(dtxtApdef, pd.apdef);
        SetText(dtxtDodge, pd.dodge+"%");
        SetText(dtxtPierce, pd.pierce+"%");
        SetText(dtxtCritical, pd.critical + "%");
    }

    public void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIOpenPage);
        MainCitySys.Instance.CloseInfoWnd();
    }

    public void ClickDetailCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        SetActive(detailTrans, false);
    }

    public void ClickDetailBtn()
    {
        SetActive(detailTrans);
    }
}