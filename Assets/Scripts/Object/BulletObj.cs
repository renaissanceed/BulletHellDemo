using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObj : MonoBehaviour
{
    //子弹使用的数据
    private BulletInfo info;

    //用于曲线运动的计时  变量
    private float time;

    //用于初始化 子弹数据的方法
    public void InitInfo(BulletInfo info)
    {
        this.info = info;
        //根据生命周期函数 决定自己什么时候 延迟移除
        //通过延迟函数 保证就算对象被提前删除了 也不会报错
        //因为延迟函数 会在对象移除后 不执行 
        Invoke("DelayDestroy",info.lifeTime);
    }
    private void DelayDestroy()
    {
        //直接执行死亡 会播放爆炸特效
        Dead();
    }

    //销毁子弹
    public void Dead()
    {
        //创建死亡特效
        GameObject effObj = Instantiate(Resources.Load<GameObject>(info.deadEff));
        //设置特效位置 创建在当前点击的位置
        effObj.transform.position = transform.position;
        //1秒后 延迟移除
        Destroy(effObj, 1f);

        //销毁子弹对象
        Destroy(this.gameObject);
    }

    //和对象 碰撞时
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //得到玩家脚本
            PlayerObj obj = other.GetComponent<PlayerObj>();
            //玩家受伤 减血
            obj.Wound();

            //销毁自己
            Dead();
        }

        if (other.gameObject.CompareTag("Eff"))
        {
            //销毁自己
            Dead();
            Destroy(other.gameObject);
            PlayerObj.Instance.isDefC = true;
            //创建死亡特效
            GameObject effObj = Instantiate(Resources.Load<GameObject>("DeadEff/Buff/DefEffDead"));
            //设置特效位置 创建在当前点击的位置
            effObj.transform.position = PlayerObj.Instance.transform.position;
            //1秒后 延迟移除
            Destroy(effObj, 1f);
        }

        if (other.gameObject.CompareTag("EffRevive"))
        {
            //销毁自己
            Dead();
        }
    }
    void Update()
    {
        //所有移动都是朝向 自己的面朝向 
        transform.Translate(Vector3.forward * info.forwardSpeed * Time.deltaTime);
        //处理其他的方式
        //1 直线运动
        //2 曲线运动
        //3 右抛物线
        //4 左抛物线
        //5 跟踪
        switch (info.type)
        {
            case 2:
                time += Time.deltaTime;
                //sin里面的值  变化的快慢 决定了左右变化的频率 
                //乘以速度 变化的大小 决定了 左右位移的多少
                //roundSpeed 主要用于控制变化的频率
                transform.Translate(Vector3.right * Time.deltaTime * Mathf.Sin(time * info.roundSpeed) * info.rightSpeed);
                break;
            case 3:
                //右抛物线 无非就是去改变 我们的旋转角度 
                transform.rotation *= Quaternion.AngleAxis(info.roundSpeed * Time.deltaTime,Vector3.up);
                break;
            case 4:
                //左抛物线 无非就是去改变 我们的旋转角度 
                transform.rotation *= Quaternion.AngleAxis(-info.roundSpeed * Time.deltaTime, Vector3.up);
                break;
            case 5:
                //跟踪移动 不停计算 玩家和子弹之间的方向向量 然后得到 四元数 不停地改变自己的角度 变化为 目标四元数
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                Quaternion.LookRotation(PlayerObj.Instance.transform.position - transform.position),
                                                info.roundSpeed * Time.deltaTime);
                break;
        }
    }
}
