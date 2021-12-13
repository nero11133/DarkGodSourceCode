/****************************************************
    文件：GameQuitWnd.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/7/29 16:12:52
	功能：游戏退出窗口
*****************************************************/

using UnityEngine;

public class GameQuitWnd : WindowRoot 
{
    public void GameQuit()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        Application.Quit();
    }

    public void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.AudioUIClick);
        SetWndowState(false);
    }
}