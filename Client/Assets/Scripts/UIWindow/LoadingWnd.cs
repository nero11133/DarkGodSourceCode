/****************************************************
    文件：LoadingWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/23 20:48:41
	功能：加载界面窗口
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LoadingWnd : WindowRoot 
{
    public Image imgFg;
    public Image imgPoint;
    public Text textTips;
    public Text textPrg;

    private float imgFGWith;

    protected override void InitWnd()
    {
        base.InitWnd();
        imgFGWith = imgFg.rectTransform.sizeDelta.x;
        imgFg.fillAmount = 0;
        imgPoint.rectTransform.localPosition = new Vector3(-(imgFGWith / 2), 0, 0);
        SetText(textTips, "这是一条游戏Tips");
        SetText(textPrg, "0%");
    }
    public void SetPrg(float prg)
    {
        imgFg.fillAmount = prg;
        float posX = imgFGWith * prg - imgFGWith / 2;
        imgPoint.rectTransform.localPosition = new Vector3(posX, 0, 0);
        SetText(textPrg, prg * 100 + "%");
    }
}