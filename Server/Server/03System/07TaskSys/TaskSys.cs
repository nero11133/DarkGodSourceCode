/****************************************************
	文件：TaskSys.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/11 20:30   	
	功能：任务奖励系统
*****************************************************/
using PEProtocol;

public class TaskSys
{
    private static TaskSys instance;
    public static TaskSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TaskSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("TaskSys Init Done");
    }

    public void ReqTakeTask(MsgPack pack)
    {
        ReqTakeTask data = pack.msg.reqTakeTask;
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspTakeTask
        };
        TaskRewardCfg trc = CfgSvc.Instance.GetTaskRewardCfg(data.taskID);
        TaskRewardData trd = CalcTaskRewardData(pd, data.taskID);
        if (trd.prg == trc.count && trd.taked == false)
        {
            pd.coin += trc.coin;
            PECommon.CalcExp(pd, trc.exp);
            trd.taked = true;
            CalcTaskArr(pd, trd);
            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspTakeTask rspTakeTask = new RspTakeTask
                {
                    coin = pd.coin,
                    exp = pd.exp,
                    lv = pd.lv,
                    taskArr = pd.taskArr
                };
                msg.rspTakeTask = rspTakeTask;
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        pack.session.SendMsg(msg);
    }

    private TaskRewardData CalcTaskRewardData(PlayerData pd,int taskID)
    {
        TaskRewardData trd = null;
        for (int i = 0; i < pd.taskArr.Length; i++)
        {
            //1|0|0
            string[] taskInfo = pd.taskArr[i].Split('|');
            if (int.Parse(taskInfo[0]) == taskID)
            {
                trd = new TaskRewardData
                {
                    id = int.Parse(taskInfo[0]),
                    prg = int.Parse(taskInfo[1]),
                    taked = taskInfo[2].Equals("1")
                };
                break;
            }
        }
        return trd;
    }

    private void CalcTaskArr(PlayerData pd,TaskRewardData trd)
    {
        string result = trd.id + "|" + trd.prg + "|" + (trd.taked ? 1 : 0);
        int index = -1;
        for (int i = 0; i < pd.taskArr.Length; i++)
        {
            string[] taskInfo = pd.taskArr[i].Split('|');
            if (int.Parse(taskInfo[0]) == trd.id)
            {
                index = i;
                break;
            }
        }
        pd.taskArr[index] = result;
    }

    public void CalcTaskPrg(PlayerData pd,int taskID)
    {
        TaskRewardData trd = CalcTaskRewardData(pd, taskID);
        TaskRewardCfg trc = CfgSvc.Instance.GetTaskRewardCfg(taskID);
        if (trd.prg < trc.count)
        {
            //更新任务进度
            trd.prg += 1;
            CalcTaskArr(pd, trd);
            ServerSession session = cacheSvc.GetOnLineSession(pd.id);
            if (session != null)
            {
                session.SendMsg(new GameMsg
                {
                    cmd = (int)CMD.PshTaskPrg,
                    pshTaskPrg = new PshTaskPrg
                    {
                        taskArr = pd.taskArr
                    }
                });
            }
        }
    }

    public PshTaskPrg GetTaskPrg(PlayerData pd, int taskID)
    {
        TaskRewardData trd = CalcTaskRewardData(pd, taskID);
        TaskRewardCfg trc = CfgSvc.Instance.GetTaskRewardCfg(taskID);
        if (trd.prg < trc.count)
        {
            //更新任务进度
            trd.prg += 1;
            CalcTaskArr(pd, trd);
            return new PshTaskPrg
            {
                taskArr = pd.taskArr
            };
        }
        else
        {
            return null;
        }
    }
}

