/****************************************************
    文件：ResSvc.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/23 18:23:44
	功能：资源加载服务
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml;

public class ResSvc : MonoBehaviour 
{
    public static ResSvc Instance;

    private Action prgAction = null;
    private Dictionary<string, AudioClip> adDict = new Dictionary<string, AudioClip>();

    public void InitSvc()
    {
        Instance = this;
        InitStrongCfg(PathDefine.StrongCfg);
        InitRDNameCfg(PathDefine.RDNameCfg);
        InitMonsterCfg(PathDefine.MonsterCfg);
        InitMapCfg(PathDefine.MapCfg);
        InitAutoGuideCfg(PathDefine.AutoGuideCfg);
        InitTaskRewardCfg(PathDefine.TaskRewardCfg);

        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);
        InitSkillActionCfg(PathDefine.SkillActionCfg);
        PECommon.Log("Init ResSvc");
    }

    public void AsyncLoadScene(string sceneName,Action loaded)
    {
        GameRoot.Instance.loadingWnd.SetWndowState();
        AsyncOperation asyncOperation= SceneManager.LoadSceneAsync(sceneName);
        prgAction = () =>
        {
            float prg = asyncOperation.progress;
            GameRoot.Instance.loadingWnd.SetPrg(prg);
            if (prg == 1)
            {
                if (loaded != null)
                {
                    loaded();
                }
                prgAction = null;
                asyncOperation = null;
                GameRoot.Instance.loadingWnd.SetWndowState(false);
                
            }
        };
    }

    public AudioClip LoadAudio(string path,bool cache = false)
    {
        AudioClip audioClip = null;
        if(!adDict.TryGetValue(path,out audioClip))
        {
            audioClip = Resources.Load<AudioClip>(path);
            if (cache)
            {
                adDict.Add(path, audioClip);
            }
        }
        return audioClip;
    }

    private Dictionary<string, GameObject> goDict = new Dictionary<string, GameObject>();

    public GameObject LoadPrefab(string path,bool cache=false)
    {
        GameObject prefab;
        GameObject go = null;
        if (!goDict.TryGetValue(path,out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if (cache)
            {
                goDict.Add(path, prefab);
            }
        }
        if (prefab != null)
        {
            go = Instantiate(prefab);
        }
        return go;
    }

    private Dictionary<string, Sprite> spDict = new Dictionary<string, Sprite>();
    public Sprite LoadSprite(string path,bool cache = false)
    {
        Sprite sprite = null;
        if(!spDict.TryGetValue(path,out sprite))
        {
            sprite = Resources.Load<Sprite>(path);
            if (cache)
            {
                spDict.Add(path, sprite);
            }
        }
        return sprite;
    }

    #region InitCfgs
    #region Rdname
    private List<string> surnameLst = new List<string>();
    private List<string> manLst = new List<string>();
    private List<string> womanlst = new List<string>();

    private void InitRDNameCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.text);
            XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement xmlElement = nodeLst[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                //int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                foreach (XmlElement e in nodeLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "surname":
                            surnameLst.Add(e.InnerText);
                            break;
                        case "man":
                            manLst.Add(e.InnerText);
                            break;
                        case "woman":
                            womanlst.Add(e.InnerText);
                            break;
                        default:
                            break;
                    }
                }
            }
        }


    }
    public string GetRDName(bool man = true)
    {
        System.Random random = new System.Random();
        string rdName = surnameLst[PETools.GetRDInt(0, surnameLst.Count - 1, random)];
        if (man)
        {
            rdName += manLst[PETools.GetRDInt(0, manLst.Count - 1, random)];
        }
        else
        {
            rdName += womanlst[PETools.GetRDInt(0, womanlst.Count - 1, random)];
        }
        return rdName;
    }
    #endregion

    #region Map

    private Dictionary<int, MapCfg> mapCfgDataDict = new Dictionary<int, MapCfg>();

    private void InitMapCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.text);
            XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement xmlElement = nodeLst[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                MapCfg mapCfg = new MapCfg()
                {
                    id = ID,
                    monsterLst = new List<MonsterData>(),
                };
                foreach (XmlElement e in nodeLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "mapName":
                            mapCfg.mapName = e.InnerText;
                            break;
                        case "sceneName":
                            mapCfg.sceneName = e.InnerText;
                            break;
                        case "mainCamPos":
                            {
                                string[] valueArr = e.InnerText.Split(',');
                                mapCfg.mainCamPos = new Vector3(float.Parse(valueArr[0]), float.Parse(valueArr[1]), float.Parse(valueArr[2]));
                            }
                            break;
                        case "mainCamRote":
                            {
                                string[] valueArr = e.InnerText.Split(',');
                                mapCfg.mainCamRote = new Vector3(float.Parse(valueArr[0]), float.Parse(valueArr[1]), float.Parse(valueArr[2]));
                            }
                            break;
                        case "playerBornPos":
                            {
                                string[] valueArr = e.InnerText.Split(',');
                                mapCfg.playerBornPos = new Vector3(float.Parse(valueArr[0]), float.Parse(valueArr[1]), float.Parse(valueArr[2]));
                            }
                            break;
                        case "playerBornRote":
                            {
                                string[] valueArr = e.InnerText.Split(',');
                                mapCfg.playerBornRote = new Vector3(float.Parse(valueArr[0]), float.Parse(valueArr[1]), float.Parse(valueArr[2]));
                            }
                            break;
                        case "power":
                            mapCfg.power = int.Parse(e.InnerText);
                            break;
                        case "monsterLst":
                            {
                                string[] valueArr = e.InnerText.Split('#');
                                for (int waveIndex = 0; waveIndex < valueArr.Length; waveIndex++)
                                {
                                    if (waveIndex == 0)
                                    {
                                        continue;
                                    }
                                    string[] tempArr = valueArr[waveIndex].Split('|');
                                    for (int j = 0; j < tempArr.Length; j++)
                                    {
                                        if (j == 0)
                                        {
                                            continue;
                                        }
                                        string[] arr = tempArr[j].Split(',');
                                        MonsterData md = new MonsterData
                                        {
                                            id = int.Parse(arr[0]),
                                            mWave = waveIndex,
                                            mIndex = j,
                                            mCfg = GetMonsterCfg(int.Parse(arr[0])),
                                            mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                            mBornRote = new Vector3(0, float.Parse(arr[4]), 0),
                                            mLevel = int.Parse(arr[5])
                                        };
                                        mapCfg.monsterLst.Add(md);
                                    }
                                }
                            }
                            break;
                        case "coin":
                            mapCfg.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mapCfg.exp = int.Parse(e.InnerText);
                            break;
                        case "crystal":
                            mapCfg.crystal = int.Parse(e.InnerText);
                            break;

                    }
                }
                mapCfgDataDict.Add(ID, mapCfg);
            }
        }
    }
    public MapCfg GetMapCfg(int id)
    {
        MapCfg mapCfg = null;
        if(mapCfgDataDict.TryGetValue(id,out mapCfg))
        {
            return mapCfg;
        }
        return null;
    }
    #endregion


    #region 自动引导配置
    private Dictionary<int, AutoGuideCfg> guideTaskDict = new Dictionary<int, AutoGuideCfg>();

    private void InitAutoGuideCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.text);
            XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement xmlElement = nodeLst[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                AutoGuideCfg agCfg = new AutoGuideCfg()
                {
                    id = ID
                };
                foreach (XmlElement e in nodeLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "npcID":
                            agCfg.npcId = int.Parse(e.InnerText);
                            break;
                        case "dilogArr":
                            agCfg.dilogArr = e.InnerText;
                            break;
                        case "actID":
                            agCfg.actId = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            agCfg.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            agCfg.exp = int.Parse(e.InnerText);
                            break;
                    }
                }
                guideTaskDict.Add(ID, agCfg);

            }
        }
    }

    public AutoGuideCfg GetGuideTaskCfg(int id)
    {
        AutoGuideCfg guideCfg = null;
        if (guideTaskDict.TryGetValue(id, out guideCfg))
        {
            return guideCfg;
        }
        return null;
    }
    #endregion

    #region 强化升级配置
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDict = new Dictionary<int, Dictionary<int, StrongCfg>>();

    private void InitStrongCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.text);
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
                if(strongDict.TryGetValue(sd.pos,out dict))
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
        }
    }

    public StrongCfg GetStrongData(int pos,int starlv)
    {
        StrongCfg sd = null;
        Dictionary<int, StrongCfg> dict = null;
        if(strongDict.TryGetValue(pos,out dict))
        {
            dict.TryGetValue(starlv, out sd);
        }
        return sd;
    }

    public int GetPropAddValPreLv(int pos,int starlv,int type)
    {
        int val = 0;
        Dictionary<int, StrongCfg> posDict = null;
        if(strongDict.TryGetValue(pos,out posDict))
        {
            for (int i = 0; i < starlv; i++)
            {
                StrongCfg strongCfg = null;
                if(posDict.TryGetValue(i,out strongCfg))
                {
                    switch (type)
                    {
                        case 1://生命
                            val += strongCfg.addhp;
                            break;
                        case 2://伤害
                            val += strongCfg.addhurt;
                            break;
                        case 3://防御
                            val += strongCfg.adddef;
                            break;
                    }
                }
            }
        }
        return val;
    }
    #endregion

    #region 任务奖励配置
    private Dictionary<int, TaskRewardCfg> taskRewardDict = new Dictionary<int, TaskRewardCfg>();

    private void InitTaskRewardCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.text);
            XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement xmlElement = nodeLst[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                TaskRewardCfg trc = new TaskRewardCfg()
                {
                    id = ID
                };
                foreach (XmlElement e in nodeLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "taskName":
                            trc.taskName = e.InnerText;
                            break;
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
        }
    }

    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg crc = null;
        if (taskRewardDict.TryGetValue(id, out crc))
        {
            return crc;
        }
        return null;
    }
    #endregion

    #region 技能配置
    private Dictionary<int, SkillCfg> skillDict = new Dictionary<int, SkillCfg>();

    private void InitSkillCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.text);
            XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement xmlElement = nodeLst[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                SkillCfg sc = new SkillCfg()
                {
                    id = ID,
                    skillMoveList = new List<int>(),
                    skillActionList = new List<int>(),
                    skillDamageList = new List<int>()
                };
                foreach (XmlElement e in nodeLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "skillName":
                            sc.skillName = e.InnerText;
                            break;
                        case "cdTime":
                            sc.cdTime = int.Parse(e.InnerText);
                            break;
                        case "skillTime":
                            sc.skillTime = int.Parse(e.InnerText);
                            break;
                        case "aniAction":
                            sc.aniAction = int.Parse(e.InnerText);
                            break;
                        case "fx":
                            sc.fx = e.InnerText;
                            break;
                        case "isCombo":
                            sc.isCombo = e.InnerText.Equals("1");
                            break;
                        case "isCollide":
                            sc.isCollide = e.InnerText.Equals("1");
                            break;
                        case "isBreak":
                            sc.isBreak = e.InnerText.Equals("1");
                            break;
                        case "dmgType":
                            if (e.InnerText.Equals("1"))
                            {
                                sc.dmgType = DamageType.AD;
                            }
                            else if (e.InnerText.Equals("2"))
                            {
                                sc.dmgType = DamageType.AP;
                            }
                            else
                            {
                                PECommon.Log("DmgType Error");
                            }
                            break;
                        case "skillMoveLst":
                            string[] skillMoveLst = e.InnerText.Split('|');
                            for (int j = 0; j < skillMoveLst.Length; j++)
                            {
                                if (skillMoveLst[j] != "")
                                {
                                    sc.skillMoveList.Add(int.Parse(skillMoveLst[j]));
                                }
                            }
                            break;
                        case "skillActionLst":
                            string[] skillActionLst = e.InnerText.Split('|');
                            for (int j = 0; j < skillActionLst.Length; j++)
                            {
                                if (skillActionLst[j] != "")
                                {
                                    sc.skillActionList.Add(int.Parse(skillActionLst[j]));
                                }
                            }
                            break;
                        case "skillDamageLst":
                            string[] skillDamageLst = e.InnerText.Split('|');
                            for (int j = 0; j < skillDamageLst.Length; j++)
                            {
                                if (skillDamageLst[j] != "")
                                {
                                    sc.skillDamageList.Add(int.Parse(skillDamageLst[j]));
                                }
                            }
                            break;
                    }
                }
                skillDict.Add(ID, sc);

            }
        }
    }

    public SkillCfg GetSkillCfg(int id)
    {
        SkillCfg sc = null;
        if (skillDict.TryGetValue(id, out sc))
        {
            return sc;
        }
        return null;
    }
    #endregion

    #region 技能移动配置
    private Dictionary<int, SkillMoveCfg> skillMoveDict = new Dictionary<int, SkillMoveCfg>();

    private void InitSkillMoveCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.text);
            XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement xmlElement = nodeLst[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                SkillMoveCfg smc = new SkillMoveCfg()
                {
                    id = ID
                };
                foreach (XmlElement e in nodeLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "moveTime":
                            smc.moveTime = int.Parse(e.InnerText);
                            break;
                        case "moveDis":
                            smc.moveDis = float.Parse(e.InnerText);
                            break;
                        case "delayTime":
                            smc.delayTime = int.Parse(e.InnerText);
                            break;
                    
                    }
                }
                skillMoveDict.Add(ID, smc);

            }
        }
    }

    public SkillMoveCfg GetSkillMoveCfg(int id)
    {
        SkillMoveCfg sc = null;
        if (skillMoveDict.TryGetValue(id, out sc))
        {
            return sc;
        }
        return null;
    }
    #endregion

    #region 技能伤害配置
    private Dictionary<int, SkillActionCfg> skillActionDict = new Dictionary<int, SkillActionCfg>();

    private void InitSkillActionCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.text);
            XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement xmlElement = nodeLst[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                SkillActionCfg sac = new SkillActionCfg()
                {
                    id = ID
                };
                foreach (XmlElement e in nodeLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "delayTime":
                            sac.delayTime = int.Parse(e.InnerText);
                            break;
                        case "radius":
                            sac.radius = float.Parse(e.InnerText);
                            break;
                        case "angle":
                            sac.angle = int.Parse(e.InnerText);
                            break;

                    }
                }
                skillActionDict.Add(ID, sac);

            }
        }
    }

    public SkillActionCfg GetSkillActionCfg(int id)
    {
        SkillActionCfg sac = null;
        if (skillActionDict.TryGetValue(id, out sac))
        {
            return sac;
        }
        return null;
    }
    #endregion

    #region 怪物配置
    private Dictionary<int, MonsterCfg> monsterCfgDict = new Dictionary<int, MonsterCfg>();

    private void InitMonsterCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.text);
            XmlNodeList nodeLst = xmlDocument.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement xmlElement = nodeLst[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                MonsterCfg mc = new MonsterCfg()
                {
                    id = ID,
                    bps=new BattleProps()
                };
                foreach (XmlElement e in nodeLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "mName":
                            mc.mName = e.InnerText;
                            break;
                        case "isStop":
                            mc.isStop = e.InnerText.Equals("1");
                            break;
                        case "mType":
                            if (e.InnerText.Equals("1"))
                            {
                                mc.monsterType = MonsterType.Normal;
                            }
                            else if (e.InnerText.Equals("2"))
                            {
                                mc.monsterType = MonsterType.Boss;
                            }
                            break;
                        case "resPath":
                            mc.resPath = e.InnerText;
                            break;
                        case "hp":
                            mc.bps.hp = int.Parse(e.InnerText);
                            break;
                        case "skillID":
                            mc.skillID = int.Parse(e.InnerText);
                            break;
                        case "atkDis":
                            mc.atkDis = float.Parse(e.InnerText);
                            break;
                        case "ad":
                            mc.bps.ad = int.Parse(e.InnerText);
                            break;
                        case "ap":
                            mc.bps.ap = int.Parse(e.InnerText);
                            break;
                        case "addef":
                            mc.bps.addef = int.Parse(e.InnerText);
                            break;
                        case "apdef":
                            mc.bps.apdef = int.Parse(e.InnerText);
                            break;
                        case "dodge":
                            mc.bps.dodge = int.Parse(e.InnerText);
                            break;
                        case "pierce":
                            mc.bps.pierce = int.Parse(e.InnerText);
                            break;
                        case "critical":
                            mc.bps.critical = int.Parse(e.InnerText);
                            break;

                    }
                }
                monsterCfgDict.Add(ID, mc);

            }
        }
    }

    public MonsterCfg GetMonsterCfg(int id)
    {
        MonsterCfg mc = null;
        if (monsterCfgDict.TryGetValue(id, out mc))
        {
            return mc;
        }
        return null;
    }
    #endregion

    #endregion

    private void Update()
    {
        if (prgAction != null)
        {
            prgAction();
        }
    }
}