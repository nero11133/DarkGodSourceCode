/****************************************************
    文件：TaskWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/11 15:51:14
	功能：任务奖励窗口
*****************************************************/

using UnityEngine;
using PEProtocol;
using System.Collections.Generic;
using UnityEngine.UI;

public class TaskWnd : WindowRoot 
{
    private PlayerData playerData;
    private List<TaskRewardData> trdList = new List<TaskRewardData>();
    public Transform scrollTrans;

    protected override void InitWnd()
    {
        base.InitWnd();
        playerData = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    public void RefreshUI()
    {
        trdList.Clear();

        List<TaskRewardData> todoList = new List<TaskRewardData>();
        List<TaskRewardData> doneList = new List<TaskRewardData>();
        //1|0|0
        for (int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskInfo = playerData.taskArr[i].Split('|');
            TaskRewardData trd = new TaskRewardData
            {
                id = int.Parse(taskInfo[0]),
                prg = int.Parse(taskInfo[1]),
                taked = taskInfo[2].Equals("1")
            };
            if (trd.taked)
            {
                doneList.Add(trd);
            }
            else
            {
                todoList.Add(trd);
            }
        }
        trdList.AddRange(todoList);
        trdList.AddRange(doneList);
        for (int i = 0; i < scrollTrans.childCount; i++)
        {
            Destroy(scrollTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < trdList.Count; i++)
        {
            GameObject go = resSvc.LoadPrefab(PathDefine.TaskItemPrefab,true);
            go.transform.SetParent(scrollTrans);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.name = "taskItem_" + i;

            TaskRewardData trd = trdList[i];
            TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trd.id);
            SetText(GetTrans(go.transform, "txtName"), trc.taskName);
            SetText(GetTrans(go.transform, "txtPrg"), trd.prg+"/"+trc.count);
            SetText(GetTrans(go.transform, "txtExp"), "奖励：      经验"+trc.exp);
            SetText(GetTrans(go.transform, "txtCoin"), "金币"+trc.coin);
            Image imgPrg = GetTrans(go.transform, "prgBar/prgTask").GetComponent<Image>();
            float prgVal = trd.prg * 1.0f / trc.count;
            imgPrg.fillAmount = prgVal;

            Button btnTake = GetTrans(go.transform, "btnTake").GetComponent<Button>();
            btnTake.onClick.AddListener(() =>
            {
                ClickTakeBtn(go.name);
            });
            Transform transComp = GetTrans(go.transform, "imgComp");
            if (trd.taked)
            {
                SetActive(transComp, true);
                btnTake.interactable = false;
            }
            else
            {
                SetActive(transComp, false);
                if (trd.prg == trc.count)
                {
                    btnTake.interactable = true;
                }
                else
                {
                    btnTake.interactable = false;
                }
            }
        }
    }

    private void ClickTakeBtn(string name)
    {
        string[] nameArr = name.Split('_');
        int index = int.Parse(nameArr[1]);
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqTakeTask,
            reqTakeTask = new ReqTakeTask
            {
                taskID = trdList[index].id
            }
        };
        netSvc.SendMsg(msg);
        TaskRewardData trd = trdList[index];
        TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trd.id);
        int exp = trc.exp;
        int coin = trc.coin;
        GameRoot.AddTips(Constants.Color("获得奖励：", TxtColor.Blue) + Constants.Color(" 经验" + exp + " 金币" + coin, TxtColor.Green));
    }

    public void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        SetWndowState(false);
    }
}