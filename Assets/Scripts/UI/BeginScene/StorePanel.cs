using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StorePanel : BasePanel<StorePanel>
{
    //关闭按钮
    public UIButton btnClose;
    //金钱总额
    public UILabel labMoney;
    //购买按钮
    public UIButton btnBuy;
    //购物车的总价值
    public UILabel labValue;
    //所有的左侧按钮
    public List<UIButton> btnLeft;
    //所有的右侧按钮
    public List<UIButton> btnRight;
    //所有显示商品选择的数量lab
    public List<UILabel> labNum;

    public override void Init()
    {
        //关闭按钮相关
        btnClose.onClick.Add(new EventDelegate(() =>
        {
            HideMe();
            ChoosePanel.Instance.ShowMe();
        }));

        //购买相关
        btnBuy.onClick.Add(new EventDelegate(() =>
        {
            if (int.Parse(labValue.text) > GameDataMgr.Instance.StoreData.money)
            {
                TipPanel.Instance.ChangeTip("钱不够");
                TipPanel.Instance.ShowMe();
                return;
            }

            if (int.Parse(labValue.text) > 0)
            {
                TipPanel.Instance.ChangeTip("购买成功");
                TipPanel.Instance.ShowMe();
                //更新减少货币显示
                GameDataMgr.Instance.StoreData.money -= int.Parse(labValue.text);
                changeMoney(GameDataMgr.Instance.StoreData.money);
                //保存购买数量
                for (int i = 0; i < labNum.Count; i++)
                {
                    GameDataMgr.Instance.StoreData.storeList[i].num += int.Parse(labNum[i].text);
                }
                GameDataMgr.Instance.SaveStoreData();
            }
        }));

        //左右按钮相关
        for (int i = 0; i < btnLeft.Count; i++)
        {
            int index = i;
            btnLeft[i].onClick.Add(new EventDelegate(() =>
            {
                int num = int.Parse(labNum[index].text);
                if (num == 0)
                {
                    labNum[index].text = 0.ToString();
                    return;
                }
                num--;
                labNum[index].text = num.ToString();
                UpdateAllCal();
            }));

            btnRight[i].onClick.Add(new EventDelegate(() =>
            {
                int num = int.Parse(labNum[index].text);
                num++;
                if (num > 10)
                {
                    return;
                }
                labNum[index].text = num.ToString();
                UpdateAllCal();
            }));
        }
        //一进来隐藏自己
        HideMe();
    }

    public override void ShowMe()
    {
        base.ShowMe();
        changeMoney(GameDataMgr.Instance.StoreData.money);
        for (int i = 0; i < btnLeft.Count; i++)
        {
            labNum[i].text = "0";
        }
    }

    //算总账的方法
    private void UpdateAllCal()
    {
        int sum = 0;
        for (int i = 0; i < labNum.Count; i++)
        {
            sum += int.Parse(labNum[i].text) * GameDataMgr.Instance.StoreData.storeList[i].price;
        }

        labValue.text = sum.ToString();
    }

    //更新金额的方法
    private void changeMoney(int money)
    {
        labMoney.text = money.ToString();
    }
}
