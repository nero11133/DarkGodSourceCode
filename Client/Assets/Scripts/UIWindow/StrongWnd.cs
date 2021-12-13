/****************************************************
    文件：StrongWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/5/25 18:18:44
	功能：强化升级界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class StrongWnd : WindowRoot 
{
    #region UI Define
    public Image imgCurtPos;
    public Text txtStarLv;
    public Transform starTransGrp;
    public Text txtPropHp1;
    public Text txtPropHp2;
    public Text txtPropHurt1;
    public Text txtPropHurt2;
    public Text txtPropDef1;
    public Text txtPropDef2;
    public Image imgPropArr1;
    public Image imgPropArr2;
    public Image imgPropArr3;

    public Transform costInfoTrans;

    public Text txtNeedLv;
    public Text txtcostCoin;
    public Text txtcostCrystal;
    public Text txtcoin;
    #endregion

    #region Data Area
    public Transform posBtnTrans;
    private Image[] images = new Image[6];
    private PlayerData playerData;
    private int curtIndex = 0;
    private StrongCfg nextSD;
    #endregion

    protected override void InitWnd()
    {
        base.InitWnd();
        playerData = GameRoot.Instance.PlayerData;
        RegClickEvts();
        ClickPosItem(0);
    }

    private void RegClickEvts()
    {
        for (int i = 0; i < posBtnTrans.childCount; i++)
        {
            Image image = posBtnTrans.GetChild(i).GetComponent<Image>();
            OnClick(image.gameObject, (object args) =>
             {
                 audioSvc.PlayUIAudio(Constants.AudioUIClick);
                 ClickPosItem((int)args);
             },i);
            images[i] = image;
        }
    }

    private void ClickPosItem(int index)
    {
        //PECommon.Log("click item"+index);
        curtIndex = index;
        for (int i = 0; i < images.Length; i++)
        {
            Transform trans = images[i].transform;
            if (i == curtIndex)
            {
                SetSprite(images[curtIndex], PathDefine.ItemArrorBg);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(245, 94);
            }
            else
            {
                SetSprite(images[i], PathDefine.ItemPlatBg);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 78);
            }
        }
        RefreshItem();
    }

    private void RefreshItem()
    {
        SetText(txtcoin, playerData.coin);
        switch (curtIndex)
        {
            case 0:
                SetSprite(imgCurtPos, PathDefine.ItemHead);
                break;
            case 1:
                SetSprite(imgCurtPos, PathDefine.ItemBody);
                break;
            case 2:
                SetSprite(imgCurtPos, PathDefine.ItemYaobu);
                break;
            case 3:
                SetSprite(imgCurtPos, PathDefine.ItemHand);
                break;
            case 4:
                SetSprite(imgCurtPos, PathDefine.ItemLeg);
                break;
            case 5:
                SetSprite(imgCurtPos, PathDefine.ItemFoot);
                break;

        }
        SetText(txtStarLv, playerData.strongArr[curtIndex]+"星级");
        int curtStarLv = playerData.strongArr[curtIndex];
        int nextStarLv = curtStarLv + 1;
        for (int i = 0; i < starTransGrp.childCount; i++)
        {
            Image image = starTransGrp.GetChild(i).GetComponent<Image>();
            if (i < curtStarLv)
            {
                SetSprite(image, PathDefine.ItemStar2);
            }
            else
            {
                SetSprite(image, PathDefine.ItemStar1);
            }
        }
        int sumAddHp1 = resSvc.GetPropAddValPreLv(curtIndex, nextStarLv, 1);
        int sumAddHurt1 = resSvc.GetPropAddValPreLv(curtIndex, nextStarLv, 2);
        int sumAddDef1 = resSvc.GetPropAddValPreLv(curtIndex, nextStarLv, 3);
        SetText(txtPropHp1,"生命  +"+sumAddHp1);
        SetText(txtPropHurt1, "伤害  +" + sumAddHurt1);
        SetText(txtPropDef1, "防御  +" + sumAddDef1);

        
        nextSD = resSvc.GetStrongData(curtIndex, nextStarLv);
        if (nextSD != null)
        {
            SetActive(imgPropArr1);
            SetActive(imgPropArr2);
            SetActive(imgPropArr3);
            SetActive(txtPropHp2);
            SetActive(txtPropHurt2);
            SetActive(txtPropDef2);
            SetActive(costInfoTrans);

            SetText(txtPropHp2, "强化后 +" + nextSD.addhp);
            SetText(txtPropHurt2, "强化后 +" + nextSD.addhurt);
            SetText(txtPropDef2, "强化后 +" + nextSD.adddef);
            SetText(txtNeedLv, "需要等级：" + nextSD.minlv);
            SetText(txtcostCoin, "需要消耗：       " + nextSD.coin);
            SetText(txtcoin, playerData.coin);
            SetText(txtcostCrystal, nextSD.crystal + "/" + playerData.crystal);
        }
        else
        {
            SetActive(imgPropArr1, false);
            SetActive(imgPropArr2, false);
            SetActive(imgPropArr3, false);
            SetActive(txtPropHp2, false);
            SetActive(txtPropHurt2, false);
            SetActive(txtPropDef2, false);
            SetActive(costInfoTrans, false);

        }

    }

    public void UpdateUI()
    {
        audioSvc.PlayUIAudio(Constants.AudioFBItemEnter);
        ClickPosItem(curtIndex);
    }

    public void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        SetWndowState(false);
    }

    public void ClickStrongBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        if (playerData.strongArr[curtIndex] < 10)
        {
            if (playerData.lv < nextSD.minlv)
            {
                GameRoot.AddTips("玩家等级不够");
                return;
            }
            if (playerData.coin < nextSD.coin)
            {
                GameRoot.AddTips("玩家金币不够");
                return;
            }
            if (playerData.crystal < nextSD.crystal)
            {
                GameRoot.AddTips("玩家水晶不够");
                return;
            }
            netSvc.SendMsg(new GameMsg
            {
                cmd = (int)CMD.ReqStrong,
                reqStrong = new ReqStrong
                {
                    pos = curtIndex
                }
            });
        }
        else
        {
            GameRoot.AddTips("星级已经升满");
        }
    }
}