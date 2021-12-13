/****************************************************
    文件：WindowRoot.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/25 20:0:25
	功能：UI窗口基类
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class WindowRoot : MonoBehaviour 
{
    protected ResSvc resSvc;
    protected AudioSvc audioSvc;
    protected NetSvc netSvc;
    protected TimerSvc timerSvc;

    public void SetWndowState(bool state = true)
    {
        if (gameObject.activeSelf != state)
        {
            SetActive(gameObject, state);
        }
        if (state)
        {
            InitWnd();
        }
        else
        {
            ClearWnd();
        }
    }

    protected virtual void InitWnd()
    {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
        timerSvc = TimerSvc.Instance;
    }

    protected virtual void ClearWnd()
    {
        resSvc = null;
        audioSvc = null;
        netSvc = null;
        timerSvc = null;
    }

    public bool GetWndState()
    {
        return gameObject.activeSelf;
    }

    #region Tool Functions

    protected void SetText(Text text,string context = "")
    {
        text.text = context;
    }

    protected void SetText(Transform trans, int num = 0)
    {
        SetText(trans.GetComponent<Text>(), num);
    }
    protected void SetText(Transform trans, string context = "")
    {
        SetText(trans.GetComponent<Text>(), context);
    }
    protected void SetText(Text txt, int num = 0)
    {
        SetText(txt, num.ToString());
    }

    protected void SetActive(GameObject go,bool state = true)
    {
        go.SetActive(state);
    }

    protected void SetActive(Transform trans, bool state = true)
    {
        trans.gameObject.SetActive(state);
    }
    protected void SetActive(RectTransform rectTrans, bool state = true)
    {
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Image img, bool state = true)
    {
        img.transform.gameObject.SetActive(state);
    }
    protected void SetActive(Text txt, bool state = true)
    {
        txt.transform.gameObject.SetActive(state);
    }
    protected T GetOrAddComponent<T>(GameObject go)
        where T:Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

    protected void SetSprite(Image image,string path)
    {
        Sprite sprite = resSvc.LoadSprite(path, true);
        image.sprite = sprite;
    }

    protected Transform GetTrans(Transform trans,string name)
    {
        if (trans != null)
        {
            return trans.Find(name);
        }
        else
        {
            return transform.Find(name);
        }
    }

    #endregion

    #region Click Evts

    protected void OnClick(GameObject go, Action<object> cb,object args)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClick = cb;
        listener.args = args;
    }

    protected void OnClickDown(GameObject go,Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onPointerDown = cb;
    }
    protected void OnClickUp(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onPointerUp = cb;
    }
    protected void OnDrag(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onDrag = cb;
    }
    #endregion
}