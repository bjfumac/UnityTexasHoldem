using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class CardTableLogic
{
    public string[] AllCards;
    public List<string> PlayerCards;
    public List<string> AICards;
    public List<string> ShareCards;

    public delegate void InitDealDelegate();
    public event InitDealDelegate OnInitDeal;
    public delegate void DealShareCardDelegate();
    public event DealShareCardDelegate OnDealShareCard;

    private PokerLogic _pokerLogic;

    public void Init()
    {
        _pokerLogic = new PokerLogic();
        InitAllCards();
        PlayerCards = new List<string>();
        AICards = new List<string>();
        ShareCards = new List<string>();
    }

    // 清空牌桌上的牌
    private void ClearTable()
    {
        PlayerCards.Clear();
        AICards.Clear();
        ShareCards.Clear();
    }

    // 初始化牌组
    private void InitAllCards()
    {
        AllCards = new string[]
        {
            // 黑桃
            "As", "Ks", "Qs", "Js", "Ts","9s","8s","7s","6s","5s","4s","3s","2s",
            // 红桃
            "Ah", "Kh", "Qh", "Jh", "Th","9h","8h","7h","6h","5h","4h","3h","2h",
            // 方块
            "Ad", "Kd", "Qd", "Jd", "Td","9d","8d","7d","6d","5d","4d","3d","2d",
            // 梅花
            "Ac", "Kc", "Qc", "Jc", "Tc","9c","8c","7c","6c","5c","4c","3c","2c"
        };
    }

    // 洗牌
    private void Shuffle()
    {
        DoShuffle(AllCards, 52);
    }
    
    // 洗牌算法 Knuth-Durstenfeld Shuffle  时间复杂度为O(n),空间复杂度为O(1)
    public void DoShuffle(string []card, int n)
    {
        Random rand = new Random();
        for (int i = 0; i < n; i++)
        {
            int r = i + rand.Next(52 - i);
            (card[r], card[i]) = (card[i], card[r]);
        }
    }
    
    // 发牌 预先生成好，简化逻辑
    private void DealCards()
    {
        PlayerCards.Add(AllCards[0]);
        AICards.Add(AllCards[1]);
        PlayerCards.Add(AllCards[2]);
        AICards.Add(AllCards[3]);
        ShareCards.Add(AllCards[4]);
        ShareCards.Add(AllCards[5]);
        ShareCards.Add(AllCards[6]);
        
        OnInitDeal?.Invoke();
    }

    // 点击Deal发额外的共享牌
    public void DealOneToShareZone()
    {
        if (ShareCards.Count == 3)
        {
            ShareCards.Add(AllCards[7]);
            OnDealShareCard?.Invoke();
        }
        else if (ShareCards.Count == 4)
        {
            ShareCards.Add(AllCards[8]);
            OnDealShareCard?.Invoke();
        }
    }

    // 开始一局
    public void StartRound()
    {
        ClearTable();
        Shuffle();
        DealCards();
    }

    // 比较双方牌面，获得对局结果
    public int Compare()
    {
        return _pokerLogic.Compare(string.Join("", PlayerCards.Concat(ShareCards)), string.Join("", AICards.Concat(ShareCards)));
    }
}
