/****************************************************
	文件：StateMove.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/17 19:58   	
	功能：移动状态
*****************************************************/


public class StateMove : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentState = AniState.Move;
        PECommon.Log("Enter StateMove");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        PECommon.Log("Exit StateMove");
    }

    public void Process(EntityBase entity, params object[] args)
    {
        PECommon.Log("Process StateMove");
        entity.SetBlend(Constants.BlendMove);
    }
}

