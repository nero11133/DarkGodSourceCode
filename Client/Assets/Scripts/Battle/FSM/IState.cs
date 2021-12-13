/****************************************************
	文件：IState.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/17 19:51   	
	功能：状态接口
*****************************************************/
public interface IState
{
    void Enter(EntityBase entity, params object[] args);
    void Process(EntityBase entity, params object[] args);
    void Exit(EntityBase entity, params object[] args);
}

public enum AniState
{
    None,
    Born,
    Idle,
    Move,
    Attack,
    Hit,
    Die
}

