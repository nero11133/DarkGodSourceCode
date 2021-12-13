/****************************************************
	文件：PECommon.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/04/27 19:52   	
	功能：服务端客户端通用工具类
*****************************************************/

using PENet;
using PEProtocol;

public enum LogType
{
    Log=0,
    Warning=1,
    Error=2,
    Info=3
}

public class PECommon
{
    public static void Log(string msg="",LogType logType = LogType.Log)
    {
        LogLevel logLevel = (LogLevel)logType;
        PETool.LogMsg(msg, logLevel);
    }

    public static int GetFightByProps(PlayerData playerData)
    {
        return playerData.lv * 100 + playerData.ad + playerData.ap + playerData.addef + playerData.apdef;
    }

    public static int GetPowerLimit(int lv)
    {
        return (lv - 1) / 10 * 150 + 150;
    }

    public static int GetExpUpValueBylv(int lv)
    {
        return 100 * lv * lv;
    }

    public static void CalcExp(PlayerData pd, int addExp)
    {
        int curtLv = pd.lv;
        int curtExp = pd.exp;
        int addRestExp = addExp;
        while (true)
        {
            int upNeedExp = GetExpUpValueBylv(curtLv) - curtExp;
            if (addRestExp >= upNeedExp)
            {
                curtLv += 1;
                curtExp = 0;
                addRestExp -= upNeedExp;
            }
            else
            {
                pd.lv = curtLv;
                pd.exp = curtExp + addRestExp;
                break;
            }
        }
    }

    public const int PowerAddCount = 2;
    public const int PowerAddSpace = 5;//分钟
}

