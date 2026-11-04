using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePanel : BasePanel<GamePanel>
{
    public UIButton btnBack;
    public UILabel labTime;

    public List<GameObject> hpObjs;

    public List<UILabel> labBuff;

    //当前游戏运行的时间
    public float nowTime;
    public override void Init()
    {
        btnBack.onClick.Add(new EventDelegate(() =>
        {
            QuitPanel.Instance.ShowMe();
            Time.timeScale = 0;
        }));
    }

    /// <summary>
    /// 提供给外部改变血量的方法
    /// </summary>
    public void ChangeHP(int hp)
    {
        for (int i = 0; i < hpObjs.Count; i++)
        {
            hpObjs[i].SetActive(i< hp);
        }
    }

    void Update()
    {                                              
        nowTime += Time.deltaTime;
        labTime.text ="";

        labTime.text += (int)nowTime % 60 + "s";
        if ((int)nowTime / 60 > 0)
        {
            labTime.text += (int)nowTime % 3600 / 60 + "m";
            if ((int)nowTime / 3600 > 0)
            {
                labTime.text += (int)nowTime / 3600 + "h";  
            }
        }

        //更新数量
        for (int i = 0; i < labBuff.Count; i++)
        {
            labBuff[i].text = GameDataMgr.Instance.StoreData.storeList[i].num.ToString();
        }
    }
}
