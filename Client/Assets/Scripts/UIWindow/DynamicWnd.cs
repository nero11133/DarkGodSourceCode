/****************************************************
    文件：DynamicWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/26 15:51:32
	功能：动态弹窗提示窗口
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class DynamicWnd : WindowRoot
{
    public Animation tipsAni;
    public Animation selfDodgeAni;
    public Text tipsText;
    public Transform hpItemRoot;

    private bool isTipsShow = false;
    private Queue<string> tipsQue = new Queue<string>();
    private Dictionary<string, ItemEntityHp> itemHpDict = new Dictionary<string, ItemEntityHp>();

    protected override void InitWnd()
    {
        base.InitWnd();
        SetActive(tipsText, false);
    }

    private void Update()
    {
        if (tipsQue.Count > 0 && isTipsShow == false)
        {
            lock (tipsQue)
            {
                string tips = tipsQue.Dequeue();
                isTipsShow = true;
                SetTips(tips);
            }
        }
    }

    #region Tips相关
    public void AddTips(string tips)
    {
        lock (tipsQue)
        {
            tipsQue.Enqueue(tips);
        }
    }

    private void SetTips(string tips)
    {
        SetActive(tipsText, true);
        SetText(tipsText, tips);
        AnimationClip animationClip = tipsAni.GetClip("TipsTextAnim");
        tipsAni.Play();
        StartCoroutine(AniPlayDone(animationClip.length, () => { SetActive(tipsText, false); isTipsShow = false; }));
    }

    IEnumerator AniPlayDone(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        if (action != null)
        {
            action();
        }
    }
    #endregion

    public void AddHpItem(string mName,int hp,Transform trans)
    {
        ItemEntityHp item = null;
        if(itemHpDict.TryGetValue(mName,out item))
        {
            return;
        }
        else
        {
            GameObject go = resSvc.LoadPrefab(PathDefine.HpItemPrefab, true);
            go.transform.SetParent(hpItemRoot);
            go.transform.localPosition = new Vector3(-1000, 0, 0);
            item = go.GetComponent<ItemEntityHp>();
            item.InitItem(hp,trans);
            itemHpDict.Add(mName, item);
        }
    }

    public void RmvHpItem(string mName)
    {
        ItemEntityHp item = null;
        if (itemHpDict.TryGetValue(mName, out item))
        {
            itemHpDict.Remove(mName);
            Destroy(item.gameObject);
        }
    }

    public void RmvAllHpItem()
    {
        foreach (var item in itemHpDict)
        {
            Destroy(item.Value.gameObject);
        }
        itemHpDict.Clear();
    }

    public void SetCritical(string mName,int critical)
    {
        ItemEntityHp item = null;
        if(itemHpDict.TryGetValue(mName,out item))
        {
            item.SetCritical(critical);
        }
    }

    public void SetDodge(string mName)
    {
        ItemEntityHp item = null;
        if (itemHpDict.TryGetValue(mName, out item))
        {
            item.SetDodge();
        }
    }

    public void SetHurt(string mName, int hurt)
    {
        ItemEntityHp item = null;
        if (itemHpDict.TryGetValue(mName, out item))
        {
            item.SetHurt(hurt);
        }
    }

    public void SetHpVal(string mName, int oldVal,int newVal)
    {
        ItemEntityHp item = null;
        if (itemHpDict.TryGetValue(mName, out item))
        {
            item.SetHpVal(oldVal,newVal);
        }
    }

    public void SetSelfDodge()
    {
        selfDodgeAni.Stop();
        selfDodgeAni.Play();
    }
}