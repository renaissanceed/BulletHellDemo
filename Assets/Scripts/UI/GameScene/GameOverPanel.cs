using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : BasePanel<GameOverPanel>
{
    public UIButton btnSure;
    public UIInput inputName;
    public UILabel labTime;

    private int endTime;
    public override void Init()
    {
        btnSure.onClick.Add(new EventDelegate(() =>
        {
            if (inputName.value == "请输入你的名字" || inputName.value == "请重新输入！")
            {
                inputName.value = "请重新输入！";
                return;
            }
            GameDataMgr.Instance.AddRankData(inputName.value, endTime);
            Time.timeScale = 1f;
            SceneManager.LoadScene("BeginScene");
        }));
        HideMe();
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //储存秒数
        endTime = (int)GamePanel.Instance.nowTime;
        //从游戏界面 得到显示当前的时间
        labTime.text = GamePanel.Instance.labTime.text;
    }
}
