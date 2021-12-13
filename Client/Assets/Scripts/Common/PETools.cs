/****************************************************
    文件：PETools.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/26 18:46:8
	功能：工具类
*****************************************************/

using System;

public class PETools  
{
    public static int GetRDInt(int min,int max,Random random=null)
    {
        if (random == null)
        {
            random = new Random();
        }
        int value = random.Next(min, max + 1);
        return value;
    }
}