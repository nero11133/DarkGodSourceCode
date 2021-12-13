/****************************************************
    文件：ItemEntityHp.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/7/1 17:32:49
	功能：血条物体
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHp : MonoBehaviour
{
    #region UI Define
    public Image imgGray;
    public Image imgRed;

    public Animation aniCritical;
    public Text txtCritical;

    public Animation aniDodge;
    public Text txtDodge;

    public Animation aniHp;
    public Text txtHp;
    #endregion

    private int hpValue;
    private Transform rootTrans;
    private RectTransform rect;
    private float scaleRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;

    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SetCritical(999);
        //    SetHurt(888);
        //}
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    SetDodge();
        //}
        Vector3 ScreenPos = Camera.main.WorldToScreenPoint(rootTrans.position);
        rect.anchoredPosition = ScreenPos * scaleRate;

        UpdateMixBlend();
        imgGray.fillAmount = currentPrg;
    }

    public void InitItem(int hp, Transform trans)
    {
        rect = GetComponent<RectTransform>();
        rootTrans = trans;
        hpValue = hp;
        imgGray.fillAmount = 1;
        imgRed.fillAmount = 1;
    }

    public void SetCritical(int critical)
    {
        aniCritical.Stop();
        txtCritical.text = "暴击 " + critical;
        aniCritical.Play();
    }

    public void SetDodge()
    {
        aniDodge.Stop();
        txtDodge.text = "闪避";
        aniDodge.Play();
    }

    public void SetHurt(int hurt)
    {
        aniHp.Stop();
        txtHp.text = "-" + hurt;
        aniHp.Play();
    }


    private float currentPrg;
    private float targetPrg;
    public void SetHpVal(int oldVal, int newVal)
    {
        currentPrg = oldVal * 1.0f / hpValue;
        targetPrg = newVal * 1.0f / hpValue;
        imgRed.fillAmount = targetPrg;
    }

    private void UpdateMixBlend()
    {
        if (Mathf.Abs(currentPrg - targetPrg) < Constants.AccelerateHpSpeed*Time.deltaTime)
        {
            currentPrg = targetPrg;
        }
        else if (currentPrg > targetPrg)
        {
            currentPrg -= Constants.AccelerateHpSpeed * Time.deltaTime;
        }
        else
        {
            currentPrg += Constants.AccelerateHpSpeed * Time.deltaTime;
        }
    }
}