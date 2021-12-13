/****************************************************
    文件：TriggerData.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/7/22 14:54:54
	功能：地图触发数据
*****************************************************/

using UnityEngine;

public class TriggerData : MonoBehaviour 
{
    public int triggerWave;
    public MapMgr mapMgr;

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (mapMgr != null)
            {
                mapMgr.TriggerMonsterBorn(this, triggerWave);
            }
        }
    }
}