/****************************************************
    文件：Constants.cs
	作者：Yangjierchang-裂开公子
    邮箱: 2972357040@qq.com
    日期：2021/4/23 20:53:15
	功能：常量配置
*****************************************************/

using UnityEngine;

public enum TxtColor
{
    Red,
    Green,
    Blue,
    Yellow,
}
public enum DamageType
{
    None,
    AD=1,
    AP=2
}
public enum EntityType
{
    None,
    Player,
    Monster
}
public enum EntityState
{
    None,
    BatiState,//霸体状态：不可控制，可以受到伤害
    //toadd
}
public enum MonsterType
{
    None,
    Normal=1,
    Boss=2,
}

public class Constants : MonoBehaviour 
{
    private const string ColorRed = "<color=#FF0000FF>";
    private const string ColorGreen = "<color=#00FF00FF>";
    private const string ColorBlue = "<color=#00B4FFFF>";
    private const string ColorYellow = "<color=#FFFF00FF>";
    private const string ColorEnd = "</color>";

    public static string Color(string str,TxtColor txtColor)
    {
        string result = "";
        switch (txtColor)
        {
            case TxtColor.Red:
                result = ColorRed + str + ColorEnd;
                break;
            case TxtColor.Green:
                result = ColorGreen + str + ColorEnd;
                break;
            case TxtColor.Blue:
                result = ColorBlue + str + ColorEnd;
                break;
            case TxtColor.Yellow:
                result = ColorYellow + str + ColorEnd;
                break;
            default:
                break;
        }
        return result;
    }

    //AutoGuideNpcId
    public const int NPCWiseMan = 0;
    public const int NPCGeneral = 1;
    public const int NPCArtisan = 2;
    public const int NPCTrader = 3;

    //场景名称/Id
    public const string SceneLogin = "SceneLogin";
    public const int SceneMainCityId = 10000;
    //public const string SceneMainCity = "SceneMainCity";

    //音效名称
    public const string BGLogin = "bgLogin";
    public const string BGMainCity = "bgMainCity";
    public const string BGHuangYe = "bgHuangYe";
    public const string AssassinHit = "assassin_Hit";

    //登录按钮音效
    public const string AudioLoginBtn = "uiLoginBtn";

    //UI点击音效
    public const string AudioUIClick = "uiClickBtn";
    public const string AudioUIExtenBtn = "uiExtenBtn";
    public const string AudioUIOpenPage = "uiOpenPage";
    public const string AudioFBItemEnter = "fbitem";

    public const string FBLose = "fblose";
    public const string FBWin = "fbwin";

    //屏幕目标分辨率
    public const int ScreenStandardWidth = 1334;
    public const int ScreenStandardHeight = 750;
    //摇杆点标准距离
    public const int ScreenOPDis = 90;

    //玩家移动速度
    public const int PlayerMoveSpeed = 8;
    //怪物移动速度
    public const int MonsterMoveSpeed = 3;

    //平滑加速度
    public const float AccelerateSpeed = 5;
    public const float AccelerateHpSpeed = 0.3f;

    //混合参数
    public const float BlendIdle = 0;
    public const float BlendMove = 1;

    //Action触发参数
    public const int ActionDefault = -1;
    public const int ActionBorn = 0;
    public const int ActionDie = 100;
    public const int ActionHit = 101;

    public const int DieAniTime = 5;
    //普攻连招有效间隔
    public const int ComboSpace = 500;//毫秒
}