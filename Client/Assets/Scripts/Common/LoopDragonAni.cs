/****************************************************
    文件：LoopDragonAni.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/25 21:53:32
	功能：重复播放龙飞行动画
*****************************************************/

using UnityEngine;

public class LoopDragonAni : MonoBehaviour 
{
    private Animation animDragon;

    private void Awake()
    {
        animDragon = GetComponent<Animation>();
    }
    private void Start()
    {
        if (animDragon != null)
        {
            InvokeRepeating("PlayDragonAni", 0, 20);
        }
    }
    private void PlayDragonAni()
    {
        if (animDragon != null)
        {
            animDragon.Play();
        }
    }
}