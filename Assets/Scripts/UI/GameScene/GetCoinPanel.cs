using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetCoinPanel : BasePanel<GetCoinPanel>
{
    public UIButton btnSure;
    public UILabel labCoin;
    public override void Init()
    {
        btnSure.onClick.Add(new EventDelegate(()=>{
            HideMe();
            GameOverPanel.Instance.ShowMe();
            GameDataMgr.Instance.StoreData.money += int.Parse(labCoin.text);
            GameDataMgr.Instance.SaveStoreData();
        }));
        HideMe();
    }

    public override void ShowMe()
    {
        base.ShowMe();
        Time.timeScale = 0f;
        labCoin.text = ((int)(GamePanel.Instance.nowTime * 0.5f)).ToString();
    }
}
