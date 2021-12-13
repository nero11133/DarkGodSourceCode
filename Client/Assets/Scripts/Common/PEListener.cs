/****************************************************
    文件：PEListener.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/5/8 18:52:5
	功能：UI事件监听插件
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PEListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,IPointerClickHandler
{
    public Action<PointerEventData> onDrag;
    public Action<PointerEventData> onPointerDown;
    public Action<PointerEventData> onPointerUp;
    public Action<object> onClick;

    public object args;

    public void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
        {
            onDrag(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(args);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null)
        {
            onPointerDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null)
        {
            onPointerUp(eventData);
        }
    }
}