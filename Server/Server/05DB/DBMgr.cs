/****************************************************
	文件：DBMgr.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/04/30 17:48   	
	功能：数据库管理类
*****************************************************/

using MySql.Data.MySqlClient;
using PEProtocol;
using System;

public class DBMgr
{
    private static DBMgr instance;
    public static DBMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBMgr();
            }
            return instance;
        }
    }
    private MySqlConnection sqlConnection = null;

    public void Init()
    {
        sqlConnection = new MySqlConnection("server=localhost;User Id=root;password=;Database=darkgod;Charset=utf8");
        sqlConnection.Open();
        PECommon.Log("DBMgr Init Done");
        //QueryPlayerData("xxx", "aaa");
    }

    public PlayerData QueryPlayerData(string acct, string pass)
    {
        PlayerData playerData = null;
        MySqlCommand sqlCommand = new MySqlCommand("select * from account where acct=@acct", sqlConnection);
        sqlCommand.Parameters.AddWithValue("acct", acct);
        MySqlDataReader sqlDataReader = null;
        bool isNew = true;
        try
        {
            sqlDataReader = sqlCommand.ExecuteReader();
            if (sqlDataReader.Read())
            {
                isNew = false;
                string _pass = sqlDataReader.GetString("pass");
                if (_pass.Equals(pass))
                {
                    //密码正确，返回玩家数据
                    playerData = new PlayerData
                    {
                        id = sqlDataReader.GetInt32("id"),
                        name = sqlDataReader.GetString("name"),
                        lv = sqlDataReader.GetInt32("level"),
                        exp = sqlDataReader.GetInt32("exp"),
                        power = sqlDataReader.GetInt32("power"),
                        coin = sqlDataReader.GetInt32("coin"),
                        diamond = sqlDataReader.GetInt32("diamond"),
                        crystal = sqlDataReader.GetInt32("crystal"),

                        hp = sqlDataReader.GetInt32("hp"),
                        ad = sqlDataReader.GetInt32("ad"),
                        ap = sqlDataReader.GetInt32("ap"),
                        addef = sqlDataReader.GetInt32("addef"),
                        apdef = sqlDataReader.GetInt32("apdef"),
                        dodge = sqlDataReader.GetInt32("dodge"),
                        pierce = sqlDataReader.GetInt32("pierce"),
                        critical = sqlDataReader.GetInt32("critical"),
                        guideId = sqlDataReader.GetInt32("guideId"),
                        time = sqlDataReader.GetInt64("time"),
                        fuben= sqlDataReader.GetInt32("fuben"),
                        //toadd
                    };
                    #region strongArr
                    //数据实例 1#2#3#4#5#6#
                    string[] strongStrArr = sqlDataReader.GetString("strong").Split('#');
                    int[] strongIntArr = new int[6];
                    for (int i = 0; i < strongStrArr.Length; i++)
                    {
                        if (strongStrArr[i] == "")
                        {
                            continue;
                        }
                        if (int.TryParse(strongStrArr[i], out int starlv))
                        {
                            strongIntArr[i] = starlv;
                        }
                        else
                        {
                            PECommon.Log("Parse Strong Data Error", LogType.Error);
                        }
                    }
                    playerData.strongArr = strongIntArr;
                    #endregion

                    #region taskArr
                    //数据示例 1|0|0#2|0|0#3|0|0#4|0|0#5|0|0#6|0|0#
                    string[] taskStrArr = sqlDataReader.GetString("task").Split('#');
                    playerData.taskArr = new string[6];
                    for (int i = 0; i < taskStrArr.Length; i++)
                    {
                        if (taskStrArr[i] == "")
                        {
                            continue;
                        }
                        else if (taskStrArr[i].Length >= 5)
                        {
                            playerData.taskArr[i] = taskStrArr[i];
                        }
                        else
                        {
                            PECommon.Log("TaskData Error", LogType.Error);
                        }
                    }
                    #endregion
                }

            }
        }
        catch (Exception e)
        {

            PECommon.Log("Query PlayerData By Acct&Pass Error:" + e, LogType.Error);
        }
        finally
        {
            if (sqlDataReader != null)
            {
                sqlDataReader.Close();
            }
            if (isNew)
            {
                playerData = new PlayerData
                {
                    id = -1,
                    name = "",
                    lv = 1,
                    exp = 0,
                    power = 150,
                    coin = 5000,
                    diamond = 500,
                    crystal = 500,

                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,
                    guideId = 1001,
                    strongArr = new int[6],
                    taskArr = new string[6],
                    time = TimerSvc.Instance.GetNowTime(),
                    fuben=10001,
                };
                //数据示例 1|0|0#2|0|0#3|0|0#4|0|0#5|0|0#6|0|0#
                //初始化任务奖励数据
                for (int i = 0; i < playerData.taskArr.Length; i++)
                {
                    playerData.taskArr[i] = (i + 1) + "|0|0";
                }
                playerData.id = InsertNewAcctData(acct, pass, playerData);
            }
        }

        return playerData;
    }

    private int InsertNewAcctData(string acct, string pass, PlayerData pd)
    {
        int id = -1;
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand
            ("insert into account set acct=@acct,pass =@pass,name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
            "hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical, guideId=@guideId,strong=@strong,time=@time,task=@task,fuben=@fuben", sqlConnection);
            sqlCommand.Parameters.AddWithValue("acct", acct);
            sqlCommand.Parameters.AddWithValue("pass", pass);
            sqlCommand.Parameters.AddWithValue("name", pd.name);
            sqlCommand.Parameters.AddWithValue("level", pd.lv);
            sqlCommand.Parameters.AddWithValue("exp", pd.exp);
            sqlCommand.Parameters.AddWithValue("power", pd.power);
            sqlCommand.Parameters.AddWithValue("coin", pd.coin);
            sqlCommand.Parameters.AddWithValue("diamond", pd.diamond);
            sqlCommand.Parameters.AddWithValue("crystal", pd.crystal);


            sqlCommand.Parameters.AddWithValue("hp", pd.hp);
            sqlCommand.Parameters.AddWithValue("ad", pd.ad);
            sqlCommand.Parameters.AddWithValue("ap", pd.ap);
            sqlCommand.Parameters.AddWithValue("addef", pd.addef);
            sqlCommand.Parameters.AddWithValue("apdef", pd.apdef);
            sqlCommand.Parameters.AddWithValue("dodge", pd.dodge);
            sqlCommand.Parameters.AddWithValue("pierce", pd.pierce);
            sqlCommand.Parameters.AddWithValue("critical", pd.critical);
            sqlCommand.Parameters.AddWithValue("guideId", pd.guideId);
            string strongInfo = "";
            for (int i = 0; i < pd.strongArr.Length; i++)
            {
                strongInfo += pd.strongArr[i];
                strongInfo += "#";
            }
            sqlCommand.Parameters.AddWithValue("strong", strongInfo);
            sqlCommand.Parameters.AddWithValue("time", pd.time);

            //数据示例 1|0|0#2|0|0#3|0|0#4|0|0#5|0|0#6|0|0#
            string taskInfo = "";
            for (int i = 0; i < pd.taskArr.Length; i++)
            {
                taskInfo += pd.taskArr[i];
                taskInfo += "#";
            }
            sqlCommand.Parameters.AddWithValue("task", taskInfo);
            sqlCommand.Parameters.AddWithValue("fuben", pd.fuben);

            sqlCommand.ExecuteNonQuery();
            id = (int)sqlCommand.LastInsertedId;
        }
        catch (Exception e)
        {

            PECommon.Log("Insert PlayerData Error:" + e, LogType.Error);
        }
        return id;

    }
    public bool QueryNameData(string name)
    {
        bool isExist = false;
        MySqlCommand sqlCommand = new MySqlCommand("select * from account where name=@name", sqlConnection);
        sqlCommand.Parameters.AddWithValue("name", name);
        MySqlDataReader sqlDataReader = null;
        try
        {
            sqlDataReader = sqlCommand.ExecuteReader();
            if (sqlDataReader.Read())
            {
                isExist = true;
            }
        }
        catch (Exception e)
        {

            PECommon.Log("QueryNameData Error:" + e, LogType.Error);
        }
        finally
        {
            if (sqlDataReader != null)
            {
                sqlDataReader.Close();
            }
        }
        return isExist;
    }

    public bool UpdatePlayerData(int id, PlayerData playerData)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand
                ("update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
                "hp = @hp, ad = @ad, ap = @ap, addef = @addef, apdef = @apdef, dodge = @dodge, pierce = @pierce, critical = @critical, guideId=@guideId,strong=@strong,time=@time,task=@task,fuben=@fuben where id =@id", sqlConnection);
            sqlCommand.Parameters.AddWithValue("id", id);
            sqlCommand.Parameters.AddWithValue("name", playerData.name);
            sqlCommand.Parameters.AddWithValue("level", playerData.lv);
            sqlCommand.Parameters.AddWithValue("exp", playerData.exp);
            sqlCommand.Parameters.AddWithValue("power", playerData.power);
            sqlCommand.Parameters.AddWithValue("coin", playerData.coin);
            sqlCommand.Parameters.AddWithValue("diamond", playerData.diamond);
            sqlCommand.Parameters.AddWithValue("crystal", playerData.crystal);


            sqlCommand.Parameters.AddWithValue("hp", playerData.hp);
            sqlCommand.Parameters.AddWithValue("ad", playerData.ad);
            sqlCommand.Parameters.AddWithValue("ap", playerData.ap);
            sqlCommand.Parameters.AddWithValue("addef", playerData.addef);
            sqlCommand.Parameters.AddWithValue("apdef", playerData.apdef);
            sqlCommand.Parameters.AddWithValue("dodge", playerData.dodge);
            sqlCommand.Parameters.AddWithValue("pierce", playerData.pierce);
            sqlCommand.Parameters.AddWithValue("critical", playerData.critical);
            sqlCommand.Parameters.AddWithValue("guideId", playerData.guideId);
            string strongInfo = "";
            for (int i = 0; i < playerData.strongArr.Length; i++)
            {
                strongInfo += playerData.strongArr[i];
                strongInfo += "#";
            }
            sqlCommand.Parameters.AddWithValue("strong", strongInfo);
            sqlCommand.Parameters.AddWithValue("time", playerData.time);
            //数据示例 1|0|0#2|0|0#3|0|0#4|0|0#5|0|0#6|0|0#
            string taskInfo = "";
            for (int i = 0; i < playerData.taskArr.Length; i++)
            {
                taskInfo += playerData.taskArr[i];
                taskInfo += "#";
            }
            sqlCommand.Parameters.AddWithValue("task", taskInfo);
            sqlCommand.Parameters.AddWithValue("fuben", playerData.fuben);

            sqlCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {

            PECommon.Log("UpdatePlayerData Error:" + e, LogType.Error);
            return false;
        }
        return true;
    }
}

