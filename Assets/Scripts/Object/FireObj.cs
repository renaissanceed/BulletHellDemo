using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示发射位置的类型
/// </summary>
public enum E_Pos_Type
{
    TopLeft, 
    TopRight, 
    Top,

    Left,
    Right,

    Buttom,
    ButtomLeft,
    ButtomRight,
}
public class FireObj : MonoBehaviour
{
    public E_Pos_Type type;

    //表示屏幕上的点
    private Vector3 screenPos;

    //初始的发射子弹的方向 主要用于作为散弹的 初始方向
    private Vector3 initDir;

    //当前开火点的数据信息
    private FireInfo fireInfo;
    private int nowNum;
    private float nowCd;
    private float nowDelay;

    //当前组开火点 使用的子弹信息
    private BulletInfo nowBulletInfo;

    //散弹时 每颗子弹的间隔角度
    private float changeAngle;

    //用于发射散弹时 记录上一次方向
    private Vector3 nowDir;

    void Update()
    {
        //测试玩家转屏幕坐标后 横截面的Z轴值
        //print(Camera.main.WorldToScreenPoint(PlayerObj.Instance.transform.position));
        //更新位置
        UpdatePos();

        //重置炮台发射数据 每一次发射都检测 是否需要重置开火点 数据
        RestFireInfo();

        //发射子弹
        UpdateFire();
    }

    /// <summary>
    /// 根据点的类型 来更新位置
    /// </summary>
    private void UpdatePos()
    {
        //这里设置和玩家位置转屏幕z一样 目的是 让点和玩家 所在的横截面一致的
        screenPos.z = 300;
        switch (type)
        {
            case E_Pos_Type.TopLeft:
                screenPos.x = 0;
                screenPos.y = Screen.height;

                initDir = Vector3.right;
                break;
            case E_Pos_Type.TopRight:
                screenPos.x = Screen.width;
                screenPos.y = Screen.height;

                initDir = Vector3.left;
                break;
            case E_Pos_Type.Top:
                screenPos.x = Screen.width / 2;
                screenPos.y = Screen.height;

                initDir = Vector3.right;
                break;
            case E_Pos_Type.Left:
                screenPos.x = 0;
                screenPos.y = Screen.height /  2;

                initDir = Vector3.right;
                break;
            case E_Pos_Type.Right:
                screenPos.x = Screen.width;
                screenPos.y = Screen.height / 2;

                initDir = Vector3.left;
                break;
            case E_Pos_Type.Buttom:
                screenPos.x = Screen.width / 2;
                screenPos.y = 0;

                initDir = Vector3.right;
                break;
            case E_Pos_Type.ButtomLeft:
                screenPos.x = 0;
                screenPos.y = 0;

                initDir = Vector3.right;
                break;
            case E_Pos_Type.ButtomRight: 
                screenPos.x = Screen.width;
                screenPos.y = 0;

                initDir = Vector3.left;
                break;
        }
        //再把屏幕点 转成世界 坐标点 得到的就我们想要的坐标
        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
    }

    //重置当前要发射的炮台数据
    private void RestFireInfo()
    {
        //只有当CD和数量 都为0时 才认为需要重新创建我们的 发射点数据
        if (nowCd != 0 && nowNum != 0)
        {
            return;
        }
        //组间休息时间 判断
        if (fireInfo != null)
        {
            nowDelay -= Time.deltaTime;
            //还在组间休息
            if (nowDelay > 0)
            {
                return;
            }
        }

        //从数据中 随机取一条数据 来按照规则 发射子弹
        List<FireInfo> list = GameDataMgr.Instance.FireData.fireInfoList;
        fireInfo = list[Random.Range(0, list.Count)];
        //我们不能直接修改数据当中的内容 我们应该拿一个变量 临时存储下来 就不会影响数据本身
        nowNum = fireInfo.num;
        nowCd = fireInfo.cd;
        nowDelay = fireInfo.delay;

        //通过发火点数据 取出 当前要使用的子弹数据信息
        //得到开始ID 和 结束ID 用于随机取子弹的信息
        string[] strs = fireInfo.ids.Split(',');
        int beginID = int.Parse(strs[0]);
        int endID = int.Parse(strs[1]);
        int randomBulletID = Random.Range(beginID , endID + 1);
        nowBulletInfo = GameDataMgr.Instance.BulletData.bulletInfoList[randomBulletID - 1];

        //等于2的时候 代表发射散弹
        if (fireInfo.type == 2)
        {
            switch (type)
            {
                case E_Pos_Type.TopLeft:
                case E_Pos_Type.TopRight:
                case E_Pos_Type.ButtomLeft:
                case E_Pos_Type.ButtomRight:
                    changeAngle = 90f / (nowNum + 1);
                    break;
                case E_Pos_Type.Top:
                case E_Pos_Type.Left:
                case E_Pos_Type.Right:
                case E_Pos_Type.Buttom:
                    changeAngle = 180f / (nowNum + 1);
                    break;
            }
        }
    }

    //检测开火
    private void UpdateFire()
    {
        //当前状态不需要发射子弹
        if (nowCd == 0 && nowNum == 0)
        {
            return;
        }

        //Cd更新
        nowCd -= Time.deltaTime;
        if (nowCd > 0)
        {
            return;
        }
        GameObject bullet;
        BulletObj obj;
        switch (fireInfo.type )
        {
            //一个一个发射子弹
            case 1:
                //动态创建子弹对象
                bullet = Instantiate(Resources.Load<GameObject>(nowBulletInfo.resName));
                //动态添加子弹脚本
                obj = bullet.AddComponent<BulletObj>();
                //把当前的子弹数据传入子弹脚本 进行初始化
                obj.InitInfo(nowBulletInfo);

                //设置子弹位置 和 朝向
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.LookRotation(PlayerObj.Instance.transform.position - transform.position);
                
                //表示发射了一颗子弹
                nowNum --;
                nowCd = nowNum == 0? 0 : nowCd = fireInfo.cd;

                break;
            //发射散弹
            case 2:
                //无 cd 一瞬间发射所有的子弹
                if (nowCd == 0)
                {
                    for (int i = 0; i < nowNum; i++)
                    {
                        //动态创建子弹对象
                        bullet = Instantiate(Resources.Load<GameObject>(nowBulletInfo.resName));
                        //动态添加子弹脚本
                        obj = bullet.AddComponent<BulletObj>();
                        //把当前的子弹数据传入子弹脚本 进行初始化
                        obj.InitInfo(nowBulletInfo);

                        //设置子弹位置 和 朝向
                        bullet.transform.position = transform.position;
                        //每次都会 旋转一个角度 得到一个新的方向
                        nowDir = Quaternion.AngleAxis(changeAngle * (i + 1), Vector3.up) * initDir;
                        bullet.transform.rotation = Quaternion.LookRotation(nowDir);
                    }

                    //重置数据
                    nowCd = nowNum = 0;
                }
                else
                {
                    //动态创建子弹对象
                    bullet = Instantiate(Resources.Load<GameObject>(nowBulletInfo.resName));
                    //动态添加子弹脚本
                    obj = bullet.AddComponent<BulletObj>();
                    //把当前的子弹数据传入子弹脚本 进行初始化
                    obj.InitInfo(nowBulletInfo);

                    //设置子弹位置 和 朝向
                    bullet.transform.position = transform.position;
                    //每次都会 旋转一个角度 得到一个新的方向
                    nowDir = Quaternion.AngleAxis(changeAngle * ((fireInfo.num - nowNum) + 1), Vector3.up) * initDir;
                    bullet.transform.rotation = Quaternion.LookRotation(nowDir);

                    //表示发射了一颗子弹
                    nowNum--;
                    nowCd = nowNum == 0 ? 0 : nowCd = fireInfo.cd;
                }
                break;
        }
    }

}
