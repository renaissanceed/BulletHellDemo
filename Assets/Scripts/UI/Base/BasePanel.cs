using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 面板基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BasePanel<T> : MonoBehaviour where T:class 
{
    private static T instance;
    public static T Instance => instance;

    protected virtual void Awake()
    {
        instance = this as T;
    }
    private void Start()
    {
        //父类中会去强行调用 初始化方法
        //因为是一个抽象函数 子类必须去实现
        Init();
    }
    //主要用于 初始化 控件事件的 监听等等逻辑
    public abstract void Init(); 

    public virtual void ShowMe()
    {
        gameObject.SetActive(true);
    }

    public virtual void HideMe()
    {
        gameObject.SetActive(false);
    }
}
