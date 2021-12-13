/****************************************************
	文件：Controller.cs
	作者：Yangjierchang-裂开公子
	邮箱:  2972357040@qq.com
	日期：2021/06/18 19:39   	
	功能：表现实体控制器抽象基类
*****************************************************/
using UnityEngine;
using System.Collections.Generic;

public abstract class Controller : MonoBehaviour
{
    public Animator animator;
    public CharacterController ctrl;
    public Transform hpRoot;

    protected bool isMove = false;
    protected Dictionary<string, GameObject> fxDict = new Dictionary<string, GameObject>();
    protected TimerSvc timerSvc;
    protected Transform camTrans;

    private Vector2 dir = Vector2.zero;

    public Vector2 Dir
    {
        get
        {
            return dir;
        }

        set
        {
            if (value == Vector2.zero)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }
            dir = value;
        }
    }

    protected bool skillMove = false;
    protected float skillMoveSpeed = 0;

    public virtual void Init()
    {
        timerSvc = TimerSvc.Instance;
    }

    public virtual void SetBlend(float blend)
    {
        animator.SetFloat("Blend", blend);
    }

    public virtual void SetAction(int act)
    {
        animator.SetInteger("Action", act);
    }

    public virtual void SetFx(string name,float time)
    {
        
    }

    public virtual void SetAtkDirCam(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(dir, new Vector2(0, 1)) + camTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    public virtual void SetAtkDirLocal(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(dir, new Vector2(0, 1));
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    public void SetSkillMove(bool skillMove,float skillSpeed = 0)
    {
        this.skillMove = skillMove;
        skillMoveSpeed = skillSpeed;
    }
}

