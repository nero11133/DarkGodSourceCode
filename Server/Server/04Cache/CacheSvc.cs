/****************************************************
	文件：CacheSvc.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/04/29 19:14   	
	功能：缓存层
*****************************************************/
using System.Collections.Generic;
using PEProtocol;

public class CacheSvc
{
    private static CacheSvc instance;
    public static CacheSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSvc();
            }
            return instance;
        }
    }
    DBMgr dbMgr = null;

    public void Init()
    {
        dbMgr = DBMgr.Instance;
        PECommon.Log("CacheSvc Init Done");
    }

    private Dictionary<string, ServerSession> onlineAcctDict = new Dictionary<string, ServerSession>();
    private Dictionary<ServerSession, PlayerData> onlineSessionDict = new Dictionary<ServerSession, PlayerData>();

    public bool IsAcctOnline(string acct)
    {
        return onlineAcctDict.ContainsKey(acct);
    }

    /// <summary>
    /// 根据账号密码返回对应账号玩家数据，账号不存在就默认创建新账号
    /// </summary>
    public PlayerData GetPlayerData(string acct,string pass)
    {
        //todo
        //根据数据库查找
        return dbMgr.QueryPlayerData(acct, pass);
    }

    /// <summary>
    /// 账号上线，缓存数据
    /// </summary>
    public void AcctOnLine(string acct,PlayerData playerData,ServerSession serverSession)
    {
        onlineAcctDict.Add(acct, serverSession);
        onlineSessionDict.Add(serverSession, playerData);
    }

    public List<ServerSession> GetOnLineSessions()
    {
        List<ServerSession> list = new List<ServerSession>();
        foreach(var item in onlineSessionDict)
        {
            list.Add(item.Key);
        }
        return list;
    }

    public ServerSession GetOnLineSession(int id)
    {
        ServerSession session = null;
        foreach (var item in onlineSessionDict)
        {
            if (item.Value.id == id)
            {
                session = item.Key;
                break;
            }
        }
        return session;
    }

    public Dictionary<ServerSession,PlayerData> GetOnLineCache()
    {
        return onlineSessionDict;
    }

    public PlayerData GetPlayerDataBySession(ServerSession session)
    {
        if(onlineSessionDict.TryGetValue(session,out PlayerData playerData))
        {
            return playerData;
        }
        else
        {
            return null;
        }
    }

    public bool IsNameExist(string name)
    {
        return dbMgr.QueryNameData(name);
    }

    public bool UpdatePlayerData(int id,PlayerData playerData)
    {
        return dbMgr.UpdatePlayerData(id, playerData);
        
    }

    public void AcctOffLine(ServerSession session)
    {
        foreach (var item in onlineAcctDict)
        {
            if (item.Value == session)
            {
                onlineAcctDict.Remove(item.Key);
                break;
            }
        }
        bool success = onlineSessionDict.Remove(session);
        PECommon.Log("OffLine Result: SessionId:" + session.sessionId + "  " + success);
    }
}

