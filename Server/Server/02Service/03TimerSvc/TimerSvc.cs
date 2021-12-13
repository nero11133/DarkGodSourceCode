/****************************************************
	文件：TimerSvc.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/09 15:06   	
	功能：定时服务
*****************************************************/
using System;
using System.Collections.Generic;

public class TimerSvc
{
    private class TaskPack
    {
        public int tid;
        public Action<int> cb;
        public TaskPack(int tid, Action<int> cb)
        {
            this.tid = tid;
            this.cb = cb;
        }
    }

    private static TimerSvc instance;
    public static TimerSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimerSvc();
            }
            return instance;
        }
    }

    private PETimer pt;
    private Queue<TaskPack> tpQue = new Queue<TaskPack>();
    private static readonly string tpQueLock = "tpQueLock";

    public void Init()
    {
        pt = new PETimer(100);
        //设置日志输出
        pt.SetLog((string info) =>
        {
            PECommon.Log(info);
        });

        pt.SetHandle((Action<int> cb, int tid) =>
        {
            if (cb != null)
            {
                lock (tpQueLock)
                {
                    tpQue.Enqueue(new TaskPack(tid, cb));
                }
            }
        });

        PECommon.Log("Init TimerSvc");
    }

    public void Update()
    {
        while (tpQue.Count>0)
        {
            TaskPack tp = null;
            lock (tpQueLock)
            {
                tp = tpQue.Dequeue();
            }
            if (tp != null)
            {
                tp.cb(tp.tid);
            }
        }
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Second, int count = 1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public long GetNowTime()
    {
        return (long)pt.GetMillisecondsTime();
    }
}

