/****************************************************
    文件：MonsterController.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/6/25 13:33:47
	功能：怪物表现实体角色控制器
*****************************************************/

using UnityEngine;

public class MonsterController : Controller 
{
    private void Update()
    {
        //AI逻辑表现
        if (isMove)
        {
            SetDir();
            SetMove();
        }
    }


    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }
    private void SetMove()
    {
        ctrl.Move(transform.forward * Constants.MonsterMoveSpeed * Time.deltaTime);
        ctrl.Move(Vector3.down * Constants.MonsterMoveSpeed * Time.deltaTime);

    }
}