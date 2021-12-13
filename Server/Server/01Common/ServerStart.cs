/****************************************************
	文件：ServerStart.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/04/27 16:20   	
	功能：服务器入口
*****************************************************/
using System.Threading;

public class ServerStart
{
    static void Main(string[] args)
    {
        ServerRoot.Instance.Init();
        while (true)
        {
            ServerRoot.Instance.Update();
            Thread.Sleep(20);
        }
    }
}

