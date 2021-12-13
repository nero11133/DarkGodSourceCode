/****************************************************
	文件：CfgSvc.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/05/24 21:32   	
	功能：配置文件服务
*****************************************************/
using System;
using System.Collections.Generic;
using System.Xml;

public class CfgSvc
{
    private static CfgSvc instance;
    public static CfgSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CfgSvc();
            }
            return instance;
        }
    }
   

    public void Init()
    {
        PECommon.Log("CfgSvc Init Done");
        InitAutoGuideCfg();
        InitStrongCfg();
        InitTaskRewardCfg();
        InitMapCfg();
    }

    #region 自动引导配置
    private Dictionary<int, GuideCfg> guideTaskDict = new Dictionary<int, GuideCfg>();

    private void InitAutoGuideCfg()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(@"G:\Unity2017Project\DrakGod\Client\Assets\Resources\ResCfgs\guide.xml");
        XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement xmlElement = nodeLst[i] as XmlElement;
            if (xmlElement.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
            GuideCfg cfg = new GuideCfg
            {
                id = ID
            };
            foreach (XmlElement e in nodeLst[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "coin":
                        cfg.coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        cfg.exp = int.Parse(e.InnerText);
                        break;
                }
            }
            guideTaskDict.Add(ID, cfg);

        }
        PECommon.Log("GuideCfg Init Done");
    }

    public GuideCfg GetGuideTaskCfg(int id)
    {
        GuideCfg guideCfg = null;
        if (guideTaskDict.TryGetValue(id, out guideCfg))
        {
            return guideCfg;
        }
        return null;
    }
    #endregion

    #region 强化配置
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDict = new Dictionary<int, Dictionary<int, StrongCfg>>();

    private void InitStrongCfg()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(@"G:\Unity2017Project\DrakGod\Client\Assets\Resources\ResCfgs\strong.xml");
        XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement xmlElement = nodeLst[i] as XmlElement;
            if (xmlElement.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
            StrongCfg sd = new StrongCfg()
            {
                id = ID
            };
            foreach (XmlElement e in nodeLst[i].ChildNodes)
            {
                int val = int.Parse(e.InnerText);
                switch (e.Name)
                {
                    case "pos":
                        sd.pos = val;
                        break;
                    case "starlv":
                        sd.starlv = val;
                        break;
                    case "addhp":
                        sd.addhp = val;
                        break;
                    case "addhurt":
                        sd.addhurt = val;
                        break;
                    case "adddef":
                        sd.adddef = val;
                        break;
                    case "coin":
                        sd.coin = val;
                        break;
                    case "ctystal":
                        sd.crystal = val;
                        break;
                    case "minlv":
                        sd.minlv = val;
                        break;
                    case "crystal":
                        sd.crystal = val;
                        break;

                }
            }
            Dictionary<int, StrongCfg> dict = null;
            if (strongDict.TryGetValue(sd.pos, out dict))
            {
                dict.Add(sd.starlv, sd);
            }
            else
            {
                dict = new Dictionary<int, StrongCfg>();
                dict.Add(sd.starlv, sd);
                strongDict.Add(sd.pos, dict);
            }
        }
        PECommon.Log("StrongCfg Init Done");
    }

    public StrongCfg GetStrongData(int pos, int starlv)
    {
        StrongCfg sd = null;
        Dictionary<int, StrongCfg> dict = null;
        if (strongDict.TryGetValue(pos, out dict))
        {
            dict.TryGetValue(starlv, out sd);
        }
        return sd;
    }
    #endregion

    #region 任务奖励配置
    private Dictionary<int, TaskRewardCfg> taskRewardDict = new Dictionary<int, TaskRewardCfg>();

    private void InitTaskRewardCfg()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(@"G:\Unity2017Project\DrakGod\Client\Assets\Resources\ResCfgs\taskreward.xml");
        XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement xmlElement = nodeLst[i] as XmlElement;
            if (xmlElement.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
            TaskRewardCfg trc = new TaskRewardCfg
            {
                id = ID
            };
            foreach (XmlElement e in nodeLst[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "coin":
                        trc.coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        trc.exp = int.Parse(e.InnerText);
                        break;
                    case "count":
                        trc.count= int.Parse(e.InnerText);
                        break;
                }
            }
            taskRewardDict.Add(ID, trc);

        }
        PECommon.Log("TaskRewardCfg Init Done");
    }

    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg trc = null;
        if (taskRewardDict.TryGetValue(id, out trc))
        {
            return trc;
        }
        return null;
    }
    #endregion

    #region 地图配置
    private Dictionary<int, MapCfg> mapDict = new Dictionary<int, MapCfg>();

    private void InitMapCfg()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(@"G:\Unity2017Project\DrakGod\Client\Assets\Resources\ResCfgs\map.xml");
        XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement xmlElement = nodeLst[i] as XmlElement;
            if (xmlElement.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
            MapCfg mc = new MapCfg
            {
                id = ID
            };
            foreach (XmlElement e in nodeLst[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "power":
                        mc.power = int.Parse(e.InnerText);
                        break;
                    case "coin":
                        mc.coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        mc.exp = int.Parse(e.InnerText);
                        break;
                    case "crystal":
                        mc.crystal = int.Parse(e.InnerText);
                        break;

                }
            }
            mapDict.Add(ID, mc);

        }
        PECommon.Log("MapCfg Init Done");
    }

    public MapCfg GetMapCfg(int id)
    {
        MapCfg mc = null;
        if (mapDict.TryGetValue(id, out mc))
        {
            return mc;
        }
        return null;
    }
    #endregion

}
public class BaseData
{
    public int id;
}

public class GuideCfg : BaseData
{
    public int npcId;
    public string dilogArr;
    public int actId;
    public int coin;
    public int exp;
}
public class StrongCfg : BaseData
{
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}
public class TaskRewardCfg : BaseData
{
    public int count;
    public int exp;
    public int coin;
}
public class TaskRewardData : BaseData
{
    public int prg;
    public bool taked;
}
public class MapCfg : BaseData
{
    public int power;
    public int coin;
    public int exp;
    public int crystal;
}

