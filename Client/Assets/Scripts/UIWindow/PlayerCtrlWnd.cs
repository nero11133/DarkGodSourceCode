/****************************************************
    文件：PlayerCtrlWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/17 17:36:11
	功能：玩家控制界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCtrlWnd : WindowRoot
{
    #region UIDefine
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    public Image imgSelfHp;

    public Text txtLv;
    public Text txtName;
    public Text txtExpPrg;
    public Text txtSelfHp;

    private int hpSum;


    public Transform expPrgTrans;

    #endregion

    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos = Vector2.zero;
    private float pointDis;

    public Vector2 currentDir = Vector2.zero;

    protected override void InitWnd()
    {
        base.InitWnd();
        defaultPos = imgDirBg.transform.position;
        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        SetActive(imgDirPoint, false);
        RefreshUI(GameRoot.Instance.PlayerData);
        RegisterTouchEvts();
        hpSum = GameRoot.Instance.PlayerData.hp;
        SetText(txtSelfHp, hpSum + "/" + hpSum);
        imgSelfHp.fillAmount = 1;
        SetBossHpBarState(false);
        InitSkillCd();
    }

    public void RefreshUI(PlayerData playerData)
    {
        SetText(txtLv, playerData.lv);
        SetText(txtName, playerData.name);

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
            currentDir = Vector2.zero;
            BattleSys.Instance.SetMoveDir(currentDir);
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
            currentDir = dir.normalized;
            BattleSys.Instance.SetMoveDir(currentDir);
        });
    }

    public Vector2 GetDir()
    {
        return currentDir;
    }

    public void ClickNormalAtk()
    {
        BattleSys.Instance.ReqReleaseSkill(0);
    }

    private void InitSkillCd()
    {
        sk1CDTime = resSvc.GetSkillCfg(101).cdTime / 1000f;
        sk2CDTime = resSvc.GetSkillCfg(102).cdTime / 1000f;
        sk3CDTime = resSvc.GetSkillCfg(103).cdTime / 1000f;

        sk1FillAmount = 0;
        sk1NumConut = 0;
        isSk1CD = false;
        SetActive(imgSk1CD, false);

        sk2FillAmount = 0;
        sk2NumConut = 0;
        isSk2CD = false;
        SetActive(imgSk2CD, false);

        sk3FillAmount = 0;
        sk3NumConut = 0;
        isSk3CD = false;
        SetActive(imgSk3CD, false);
    }

    #region sk1
    public Image imgSk1CD;
    public Text txtSk1CD;

    private float sk1CDTime;
    private int sk1Num;
    private float sk1NumConut;
    private float sk1FillAmount;

    private bool isSk1CD = false;
    #endregion

    public void ClickSkill1()
    {
        if (isSk1CD == false && GetCanRlsSkill())
        {
            BattleSys.Instance.ReqReleaseSkill(1);
            isSk1CD = true;
            sk1Num = (int)sk1CDTime;
            SetActive(imgSk1CD);
            imgSk1CD.fillAmount = 1;
            SetText(txtSk1CD, sk1Num);
        }

    }

    #region sk2
    public Image imgSk2CD;
    public Text txtSk2CD;

    private float sk2CDTime;
    private int sk2Num;
    private float sk2NumConut;
    private float sk2FillAmount;

    private bool isSk2CD = false;
    #endregion

    public void ClickSkill2()
    {
        if (isSk2CD == false && GetCanRlsSkill())
        {
            BattleSys.Instance.ReqReleaseSkill(2);
            isSk2CD = true;
            sk2Num = (int)sk2CDTime;
            SetActive(imgSk2CD);
            imgSk2CD.fillAmount = 1;
            SetText(txtSk2CD, sk2Num);
        }
    }

    #region sk3
    public Image imgSk3CD;
    public Text txtSk3CD;

    private float sk3CDTime;
    private int sk3Num;
    private float sk3NumConut;
    private float sk3FillAmount;

    private bool isSk3CD = false;
    #endregion

    public void ClickSkill3()
    {
        if (isSk3CD == false && GetCanRlsSkill())
        {
            BattleSys.Instance.ReqReleaseSkill(3);
            isSk3CD = true;
            sk3Num = (int)sk3CDTime;
            SetActive(imgSk3CD);
            imgSk3CD.fillAmount = 1;
            SetText(txtSk3CD, sk3Num);
        }
    }

    public void ClickHeadBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        BattleSys.Instance.battleMgr.isGamePause = true;
        BattleSys.Instance.SetBattleEndWndState(FBEndType.Pause);
        //Time.timeScale = 0;
    }


    public void SetSelfHpBarVal(int val)
    {
        SetText(txtSelfHp, val + "/" + hpSum);
        imgSelfHp.fillAmount = val * 1.0f / hpSum;
    }

    public bool GetCanRlsSkill()
    {
        return BattleSys.Instance.battleMgr.GetCanRlsSkill();
    }

    public Transform transBossHpBar;
    public Image imgYellow;
    public Image imgRed;
    private float currentPrg = 1;
    private float targetPrg = 1;

    public void SetBossHpBarState(bool state, float prg = 1)
    {
        SetActive(transBossHpBar, state);
        imgRed.fillAmount = prg;
        imgYellow.fillAmount = prg;

    }

    public void SetBossHpVal(int oldVal, int newVal, int sumVal)
    {
        currentPrg = oldVal * 1.0f / sumVal;
        targetPrg = newVal * 1.0f / sumVal;
        imgRed.fillAmount = targetPrg;
    }

    private void BlendBossHp()
    {
        if (Mathf.Abs(currentPrg - targetPrg) < Constants.AccelerateHpSpeed * Time.deltaTime)
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

    private void Update()
    {
        
        #region Skill CD
        float delta = Time.deltaTime;
        if (isSk1CD)
        {
            sk1FillAmount += delta;
            if (sk1FillAmount >= sk1CDTime)
            {
                isSk1CD = false;
                SetActive(imgSk1CD, false);
                sk1FillAmount = 0;
                sk1NumConut = 0;
            }
            else
            {
                imgSk1CD.fillAmount = 1 - sk1FillAmount / sk1CDTime;
            }
            sk1NumConut += delta;
            if (sk1NumConut >= 1)
            {
                sk1NumConut -= 1;
                sk1Num -= 1;
                SetText(txtSk1CD, sk1Num);
            }
        }

        if (isSk2CD)
        {
            sk2FillAmount += delta;
            if (sk2FillAmount >= sk2CDTime)
            {
                isSk2CD = false;
                SetActive(imgSk2CD, false);
                sk2FillAmount = 0;
                sk2NumConut = 0;
            }
            else
            {
                imgSk2CD.fillAmount = 1 - sk2FillAmount / sk2CDTime;
            }
            sk2NumConut += delta;
            if (sk2NumConut >= 1)
            {
                sk2NumConut -= 1;
                sk2Num -= 1;
                SetText(txtSk2CD, sk2Num);
            }
        }

        if (isSk3CD)
        {
            sk3FillAmount += delta;
            if (sk3FillAmount >= sk3CDTime)
            {
                isSk3CD = false;
                SetActive(imgSk3CD, false);
                sk3FillAmount = 0;
                sk3NumConut = 0;
            }
            else
            {
                imgSk3CD.fillAmount = 1 - sk3FillAmount / sk3CDTime;
            }
            sk3NumConut += delta;
            if (sk3NumConut >= 1)
            {
                sk3NumConut -= 1;
                sk3Num -= 1;
                SetText(txtSk3CD, sk3Num);
            }
        }
        #endregion

        if (transBossHpBar.gameObject.activeSelf)
        {
            BlendBossHp();
            imgYellow.fillAmount = currentPrg;
        }
        //if (BattleSys.Instance.battleMgr.isGamePause) return;
        //#region player ctrl

        //if (Input.GetMouseButtonDown(0))
        //{
        //    ClickNormalAtk();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    ClickSkill1();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    ClickSkill2();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    ClickSkill3();
        //}
        //float x = Input.GetAxisRaw("Horizontal");
        //float y = Input.GetAxisRaw("Vertical");
        //if (x != 0 || y != 0)
        //{
        //    BattleSys.Instance.SetMoveDir(new Vector2(x, y));
        //}
        //else
        //{
        //    BattleSys.Instance.SetMoveDir(Vector2.zero);
        //}
        //#endregion
    }
}