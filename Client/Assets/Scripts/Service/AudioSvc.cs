/****************************************************
    文件：AudioSvc.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/25 20:44:30
	功能：声音播放服务
*****************************************************/

using UnityEngine;

public class AudioSvc : MonoBehaviour 
{
    public static AudioSvc Instance;

    public AudioSource bgAudio;
    public AudioSource uiAudio;

    public void InitSvc()
    {
        Instance = this;
        PECommon.Log("AudioSvc Init");
    }

    public void PlayBGM(string name,bool isLoop = true)
    {
        AudioClip audioClip = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        if (bgAudio.clip == null || bgAudio.clip.name != audioClip.name)
        {
            bgAudio.clip = audioClip;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    public void StopBGM()
    {
        if (bgAudio != null)
        {
            bgAudio.Stop();
        }
    }

    public void PlayUIAudio(string name)
    {
        AudioClip audioClip = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        if (audioClip != null)
        {
            uiAudio.clip = audioClip;
            uiAudio.Play();
        }
    }

    public void PlayCharAudio(string name,AudioSource audioSource)
    {
        AudioClip audioClip = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}