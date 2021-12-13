/****************************************************
    文件：TimerSvc.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/8 19:54:13
	功能：计时服务
*****************************************************/

using UnityEngine;
using System;

public class TimerSvc : MonoBehaviour 
{
    public static TimerSvc Instance;

    private PETimer pt;

    public void InitSvc()
    {
        Instance = this;
        pt = new PETimer();
        //设置日志输出
        pt.SetLog((string info) =>
        {
            PECommon.Log(info);
        });
        PECommon.Log("Init TimerSvc");
    }

    private void Update()
    {
        //if (BattleSys.Instance != null && BattleSys.Instance.battleMgr != null && BattleSys.Instance.battleMgr.isGamePause) return;
        pt.Update();
    }

    public int AddTimeTask(Action<int> callback,double delay,PETimeUnit timeUnit=PETimeUnit.Second,int count = 1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public void DeleteTimeTask(int tid)
    {
        pt.DeleteTimeTask(tid);
    }

    public double GetNowTime()
    {
        return pt.GetMillisecondsTime();
    }

  
}