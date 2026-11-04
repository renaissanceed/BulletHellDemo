using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        //根据开始场景选择的英雄 来动态创建 飞机
        RoleInfo info = GameDataMgr.Instance.GetNowSelHeroInfo();

        //根据数据中的资源 路径 进行动态创建
        GameObject obj = Instantiate(Resources.Load<GameObject>(info.resName));
        PlayerObj playerObj = obj.AddComponent<PlayerObj>();
        playerObj.speed = info.speed * 20;
        playerObj.maxHP = 10;
        playerObj.nowHP = info.hp;
        playerObj.roundSpeed = 30;

        //更新显示的血量
        GamePanel.Instance.ChangeHP(info.hp);
    }
}
