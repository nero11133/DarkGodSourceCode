/****************************************************
    文件：PlayerController.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/5/13 16:38:24
	功能：主角表现实体角色控制器
*****************************************************/

using UnityEngine;

public class PlayerController : Controller 
{
    public GameObject skill1Fx;
    public GameObject skill2Fx;
    public GameObject skill3Fx;

    public GameObject atk1Fx;
    public GameObject atk2Fx;
    public GameObject atk3Fx;
    public GameObject atk4Fx;
    public GameObject atk5Fx;

    
    private Vector3 camOffSet;

    private float targetBlend;
    private float currentBlend;

    public override void Init()
    {
        base.Init();
        camTrans = Camera.main.transform;
        camOffSet = transform.position - camTrans.position;
        if (skill1Fx != null)
        {
            fxDict.Add(skill1Fx.name, skill1Fx);
        }
        if (skill2Fx != null)
        {
            fxDict.Add(skill2Fx.name, skill2Fx);
        }
        if (skill3Fx != null)
        {
            fxDict.Add(skill3Fx.name, skill3Fx);
        }

        if (atk1Fx != null)
        {
            fxDict.Add(atk1Fx.name, atk1Fx);
        }
        if (atk2Fx != null)
        {
            fxDict.Add(atk2Fx.name, atk2Fx);
        }
        if (atk3Fx != null)
        {
            fxDict.Add(atk3Fx.name, atk3Fx);
        }
        if (atk4Fx != null)
        {
            fxDict.Add(atk4Fx.name, atk4Fx);
        }
        if (atk5Fx != null)
        {
            fxDict.Add(atk5Fx.name, atk5Fx);
        }

    }
    private void Update()
    {
        #region Input
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");
        //Vector2 _dir = new Vector2(h, v).normalized;
        //if (_dir == Vector2.zero)
        //{
        //    Dir = Vector2.zero;
        //    SetBlend(Constants.BlendIdle);
        //}
        //else
        //{
        //    Dir = _dir;
        //    SetBlend(Constants.BlendRun);
        //}
        #endregion

        if (currentBlend != targetBlend)
        {
            UpdateMixBlend();
        }

        if (isMove)
        {
            //设置方向
            SetDir();
            //产生移动
            SetMove();
            //相机跟随
            SetCam();
        }

        if (skillMove)
        {
            SkillMove();
            SetCam();
        }
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0,1))+camTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }
    private void SetMove()
    {
        ctrl.Move(transform.forward * Constants.PlayerMoveSpeed * Time.deltaTime);
    }

    private void SkillMove()
    {
        ctrl.Move(transform.forward * skillMoveSpeed * Time.deltaTime);
    }

    public void SetCam()
    {
        if (camTrans != null)
        {
            camTrans.position = transform.position - camOffSet;
        }
    }
    public override void SetBlend(float blend)
    {
        targetBlend = blend;
    }

    public override void SetFx(string name, float time)
    {
        GameObject go;
        if(fxDict.TryGetValue(name,out go))
        {
            go.SetActive(true);
            timerSvc.AddTimeTask((int tid) =>
            {
                go.SetActive(false);
            }, time,PETimeUnit.Millisecond);
        }

    }

    private void UpdateMixBlend()
    {
        if (Mathf.Abs(targetBlend - currentBlend) < Constants.AccelerateSpeed * Time.deltaTime)
        {
            currentBlend = targetBlend;
        }else if (currentBlend > targetBlend)
        {
            currentBlend -= Constants.AccelerateSpeed * Time.deltaTime;
        }
        else
        {
            currentBlend += Constants.AccelerateSpeed * Time.deltaTime;
        }
        animator.SetFloat("Blend", currentBlend);
    }
}