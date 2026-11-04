using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipPanel : BasePanel<TipPanel>
{
    //关闭按钮
    public UIButton btnClose;
    //提示信息
    public UILabel labTip;
    public override void Init()
    {
        //关闭逻辑
        btnClose.onClick.Add(new EventDelegate(() =>
        {
            HideMe();
            StorePanel.Instance.ShowMe();
        }));
        HideMe();
    }

    public void ChangeTip(string tip = "钱不够")
    {
        //提供给外部的修改提示信息的方法
        labTip.text = tip;
    }

    public override void HideMe()
    {
        base.HideMe();
        StorePanel.Instance.labValue.text = 0.ToString();
    }
}
