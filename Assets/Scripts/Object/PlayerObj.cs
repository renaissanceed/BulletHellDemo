using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerObj : MonoBehaviour
{
    //单例模式 方便获取玩家信息
    private static PlayerObj instance;
    public static PlayerObj Instance => instance;
    private void Awake()
    {
        instance = this;
    }
    //血量
    public int nowHP = 10, maxHP = 10;

    //速度
    public int speed = 100;
    private int nowSpeed;
    //旋转速度
    public int roundSpeed = 10;
    //目标四元数角度
    private Quaternion targetQ;

    //死亡状态
    public bool isDead;

    //当前世界坐标转屏幕上的点
    private Vector3 nowPos;
    //上一次玩家的位置  就是在位移前  玩家的位置
    private Vector3 frontPos;

    //无敌特效消失的bool
    public bool isWuDi;
    public void Dead()
    {
        isDead = true;  
        //显示结束面板
        GetCoinPanel.Instance.ShowMe();
    }

    public bool isDefC = true, isRevive = true;

    public void Wound()
    {
        if (isDead)
        {
            return;
        }
        //减血
        nowHP -= 1;
        //更新面板血量
        GamePanel.Instance.ChangeHP(nowHP);
        //是否死亡
        if (nowHP <= 0)
        {
             Dead();
        }
    }

    private float hValue;
    private float vValue;
    void Update()
    {
        //如果死亡了 就没必要移动了
        if (isDead)
        {
            return;
        }

        //旋转逻辑
        hValue = Input.GetAxisRaw("Horizontal");
        vValue = Input.GetAxisRaw("Vertical"); 
        //如果没有按 我们的 AD键 那么 目标角度就是 000度
        if (hValue == 0)
        {
            targetQ = Quaternion.identity;
        }
        //如果按 AD键 就是0020，或00-20
        else
        {
            targetQ = hValue < 0 ? Quaternion.AngleAxis(30, Vector3.forward) : Quaternion.AngleAxis(-30, Vector3.forward);
        }
        //让飞机 朝着 目标四元数 旋转
        transform.rotation =  Quaternion.Slerp(transform.rotation, targetQ,Time.deltaTime * roundSpeed);

        //记录之前的位置
        frontPos = transform.position;

        //按Shift加速
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            nowSpeed = speed;
            speed += (int)(speed * 0.1);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed -= (int)(speed * 0.1);
            speed = nowSpeed;
        }
        //移动逻辑
        transform.Translate(Vector3.forward * vValue * speed * Time.deltaTime);
        transform.Translate(Vector3.right * hValue * speed * Time.deltaTime,Space.World);

        //进行极限 判断
        nowPos = Camera.main.WorldToScreenPoint(transform.position);
        //溢出判断
        if(nowPos.x <= 0 || nowPos.x >= Screen.width)
        {
            transform.position = new Vector3(frontPos.x,transform.position.y, transform.position.z);
        }
        if (nowPos.y <= 0 || nowPos.y >= Screen.height)
        { 
            transform.position = new Vector3(transform.position.x, transform.position.y, frontPos.z);
        }

        //射线检测 用于销毁子弹
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hifInfo;
            //这里只检测 子弹层
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hifInfo,
                1000, 1 << LayerMask.NameToLayer("Bullet")))
            {
                BulletObj obj = hifInfo.transform.GetComponent<BulletObj>();
                //直接让被点中的子弹 被销毁
                obj.Dead();
            }
        }

        //检测是否按下buff激活键位
        BuffActive();
    }

    //Buff激活
    public void BuffActive()
    {
        //激活速度
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (GameDataMgr.Instance.StoreData.storeList[3].num > 0)
            {
                speed += GameDataMgr.Instance.StoreData.storeList[3].attr;
                BuffEff();
                GameDataMgr.Instance.StoreData.storeList[3].num--;
            }
        }
        //激活加血
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (GameDataMgr.Instance.StoreData.storeList[1].num > 0)
            {
                nowHP = GameDataMgr.Instance.GetNowSelHeroInfo().hp;
                GamePanel.Instance.ChangeHP(nowHP);
                BuffEff();
                GameDataMgr.Instance.StoreData.storeList[1].num--;
            }
            
        }
        //激活防御
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (GameDataMgr.Instance.StoreData.storeList[2].num > 0 && isDefC)
            {
                Def();
                isDefC = false;
                BuffEff();
                GameDataMgr.Instance.StoreData.storeList[2].num--;
            }     
        }
        //激活无敌
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (isRevive && GameDataMgr.Instance.StoreData.storeList[0].num > 0)
            {
                ReviveEff();
                BuffEff();
                GameDataMgr.Instance.StoreData.storeList[0].num--;
            }
        }

    }

    //创建无敌的特效
    public void ReviveEff()
    {
        GameObject effObj = Instantiate(Resources.Load<GameObject>("DeadEff/Buff/ReviveEff"));
        effObj.transform.SetParent(transform);
        effObj.transform.position = transform.position;
        isRevive = false;
        Destroy(effObj, 3f);
        //延迟3秒后执行消失特效的函数
        Invoke("DeadReviveEndEff", 3f);
    }

    //创建无敌的特效 的消失特效
    public void DeadReviveEndEff()
    {
        //创建死亡特效
        GameObject effObj = Instantiate(Resources.Load<GameObject>("DeadEff/Buff/ReviveEffDead"));
        //设置特效位置 创建在当前点击的位置
        effObj.transform.position = transform.position;
        //1秒后 延迟移除
        Destroy(effObj, 1f);
        isRevive = true;
    }

    //获得物体 特效生成
    public void BuffEff()
    {
        //创建获得特效
        GameObject effObj = Instantiate(Resources.Load<GameObject>("DeadEff/Buff/Player"));
        //设置特效位置 创建在当前的位置
        effObj.transform.position = transform.position;
        //1秒后 延迟移除
        Destroy(effObj, 1f);
    }

    //防御特效生成
    public void Def()
    {
        GameObject effObj = Instantiate(Resources.Load<GameObject>("DeadEff/Buff/DefEff"));
        effObj.transform.SetParent(transform);
        effObj.transform.position = transform.position;
    }
}
