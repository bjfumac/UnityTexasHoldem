using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UI;
using UnityEngine;

public class CardTableScene : MonoBehaviour
{
    private UI_CardTableScene _view;

    private CardTableLogic _logic;

    private void Awake()
    {
        UI.UIBinder.BindAll();
    }

    // Start is called before the first frame update
    void Start()
    {
        // UI层初始化
        UIPanel panel = gameObject.GetComponent<UIPanel>();
        _view = panel.ui as UI_CardTableScene;
        _view.m_playerZone.itemRenderer = PlayerCardItemRenderer;
        _view.m_aiZone.itemRenderer = AICardItemRenderer;
        _view.m_shareCardZone.itemRenderer = ShareCardItemRenderer;
        // 逻辑层初始化
        _logic = new CardTableLogic();
        _logic.Init();
        // 添加各种关联事件
        AddEvents();
        // 默认开始一局
        _logic.StartRound();
    }

    // 渲染玩家牌，默认显示牌面
    private void PlayerCardItemRenderer(int index, GObject item)
    {
        var data = (_view.m_playerZone.data as List<string>)![index];
        var itemObj = item as UI_Card;
        itemObj.m_numTxt.text =data[0] == 'T'?"10": data[0].ToString();
        switch (data[1])
        {
            case 's':
                itemObj.m_cardTypeCtl.selectedIndex = 3;
                break;
            case 'h':
                itemObj.m_cardTypeCtl.selectedIndex = 2;
                break;
            case 'd':
                itemObj.m_cardTypeCtl.selectedIndex = 1;
                break;
            case 'c':
                itemObj.m_cardTypeCtl.selectedIndex = 0;
                break;
        }

        itemObj.m_isShowCtl.selectedIndex = 0;
    }
    
    // 渲染电脑牌，默认显示牌背
    private void AICardItemRenderer(int index, GObject item)
    {
        var data = (_view.m_aiZone.data as List<string>)![index];
        var itemObj = item as UI_Card;
        itemObj.m_numTxt.text =data[0] == 'T'?"10": data[0].ToString();
        switch (data[1])
        {
            case 's':
                itemObj.m_cardTypeCtl.selectedIndex = 3;
                break;
            case 'h':
                itemObj.m_cardTypeCtl.selectedIndex = 2;
                break;
            case 'd':
                itemObj.m_cardTypeCtl.selectedIndex = 1;
                break;
            case 'c':
                itemObj.m_cardTypeCtl.selectedIndex = 0;
                break;
        }

        itemObj.m_isShowCtl.selectedIndex = 1;
    }
    
    // 渲染公共牌，默认显示牌面
    private void ShareCardItemRenderer(int index, GObject item)
    {
        var data = (_view.m_shareCardZone.data as List<string>)![index];
        var itemObj = item as UI_Card;
        itemObj.m_numTxt.text =data[0] == 'T'?"10": data[0].ToString();
        switch (data[1])
        {
            case 's':
                itemObj.m_cardTypeCtl.selectedIndex = 3;
                break;
            case 'h':
                itemObj.m_cardTypeCtl.selectedIndex = 2;
                break;
            case 'd':
                itemObj.m_cardTypeCtl.selectedIndex = 1;
                break;
            case 'c':
                itemObj.m_cardTypeCtl.selectedIndex = 0;
                break;
        }

        itemObj.m_isShowCtl.selectedIndex = 0;
    }

    // 各种事件监听
    private void AddEvents()
    {
        _logic.OnInitDeal += OnInitDeal;
        _logic.OnDealShareCard += OnDealShareCard;
        _view.m_dealBtn.onClick.Add(OnClickDealBtn);
        _view.m_compareBtn.onClick.Add(OnClickCompareBtn);
        _view.m_giveUpBtn.onClick.Add(OnClickGiveUpBtn);
    }

    // 发一张新的共享牌后触发
    private void OnDealShareCard()
    {
        _view.m_shareCardZone.data = _logic.ShareCards;
        _view.m_shareCardZone.numItems = _logic.ShareCards.Count;
    }

    // 发放初始牌局后触发（玩家2张 电脑2张 共享3张）
    private void OnInitDeal()
    {
        _view.m_playerZone.data = _logic.PlayerCards;
        _view.m_playerZone.numItems = _logic.PlayerCards.Count;
        _view.m_aiZone.data = _logic.AICards;
        _view.m_aiZone.numItems = _logic.AICards.Count;
        _view.m_shareCardZone.data = _logic.ShareCards;
        _view.m_shareCardZone.numItems = _logic.ShareCards.Count;
    }

    // 点击放弃按钮，重开一局
    private void OnClickGiveUpBtn()
    {
        _view.m_resultTxt.text = "";
        _logic.StartRound();
    }

    // 点击比较按钮，比较双方牌面，获得对局结果
    private void OnClickCompareBtn()
    {
        // 共享区域的牌少于五张不能比较
        if (_logic.ShareCards.Count < 5) return;
        // 显示ai手牌牌面
        for (int i = 0; i < _view.m_aiZone.numItems; i++)
        {
            var item = _view.m_aiZone.GetChildAt(i) as UI_Card;
            item.m_isShowCtl.selectedIndex = 0;
        }
        // 比较双方牌面
        Compare();
    }

    // 比较双方牌面，获得对局结果
    private void Compare()
    {
         int ret = _logic.Compare();
         switch (ret)
         {
             case 0:
                 _view.m_resultTxt.text = "平局";
                 break;
             case 1:
                 _view.m_resultTxt.text = "玩家胜利";
                 break;
             case 2:
                 _view.m_resultTxt.text = "电脑胜利";
                 break;
         }
    }

    // 点击发牌按钮，发一张牌到共享区域
    private void OnClickDealBtn()
    {
        _logic.DealOneToShareZone();
    }
    
}
