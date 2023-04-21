using System;
using System.Collections.Generic;
using UnityEngine;

public class PokerLogic
{
    // 牌面数字（2-A）采用bit进行标记
    public readonly Dictionary<char, UInt64>  Faces;
    // 牌面花色（黑桃、红桃、方块、梅花）采用枚举进行标记
    public readonly Dictionary<char, UInt64>  Suits;
    public PokerLogic()
    {
        Faces = new Dictionary<char, UInt64>
        {
            ['A'] = 1 << 12,
            ['K'] = 1 << 11,
            ['Q'] = 1 << 10,
            ['J'] = 1 << 9,
            ['T'] = 1 << 8,
            ['9'] = 1 << 7,
            ['8'] = 1 << 6,
            ['7'] = 1 << 5,
            ['6'] = 1 << 4,
            ['5'] = 1 << 3,
            ['4'] = 1 << 2,
            ['3'] = 1 << 1,
            ['2'] = 1 << 0
        };
        
        Suits = new Dictionary<char, UInt64>
        {
            ['s'] = 3,  //黑桃spades
            ['h'] = 2,  //红桃hearts
            ['d'] = 1,  //方块diamonds
            ['c'] = 0,  //梅花clubs
        };
    }
    
    // 将手牌的字符串表示二进制化
    // 如手牌AsAhKsKhKc 为黑桃A红桃A 黑桃K红桃K梅花K
    // 经过分析，转换成hand.Faces[1]=1000000000000 hand.Faces[2]=0100000000000 
    private Hand AnalyzeHandStr(string handStr)
    {
        var hand = new Hand() {HandStr = handStr, Faces = new UInt64[4], Suits = new UInt64[4]};
        UInt64 faceValue = 0;
        for (int i = 0; i < handStr.Length; i++)
        {
            // 记录牌面点数（如AK）
            if (i%2 == 0)
            {
                faceValue = Faces[handStr[i]];
                // 出现四次的相同面值的牌,更新对应bit位为1
                hand.Faces[3] |= hand.Faces[2] & faceValue;
                // 出现三次的相同面值的牌,更新对应bit位为1
                hand.Faces[2] |= hand.Faces[1] & faceValue;
                // 出现两次的相同面值的牌,更新对应bit位为1
                hand.Faces[1] |= hand.Faces[0] & faceValue;
                // 出现一次的相同面值的牌,更新对应bit位为1
                hand.Faces[0] |= faceValue;
            } 
            // 记录牌面花色（如shc）
            else {
                hand.Suits[Suits[handStr[i]]] |= faceValue;
            }
        }

        return hand;
    }
    
    // 获取获胜者编号
    private int GetWinner(UInt64 a, UInt64 b) {
        // 平
        if (a == b)
        {
            return 0;
        }

        // A胜利
        if (a > b)
        {
            return 1;
        }

        // B胜利
        if (a < b)
        {
            return 2;
        }

        return 0;
    }
    
    // 比较两张手牌、支持任意数量手牌及任意数量赖子
    public int Compare(string strA, string strB)
    {
        var playerA = AnalyzeHandStr(strA).GetMaxHands();
        var playerB = AnalyzeHandStr(strB).GetMaxHands();

        // 比较最大牌型
        var winner = GetWinner((UInt64)playerA.MaxCase, (UInt64)playerB.MaxCase);
        Debug.Log($"玩家牌型:{playerA.MaxCase.ToString()}，电脑牌型：{playerB.MaxCase.ToString()}");
        // 如果牌型不同，直接返回结果
        if  (winner != 0)
        {
            return winner;
        }
        // 如果牌型相同，则需要比较点数
        // 如果牌型是顺子或者同花顺，会产生特例A2345，它是这类牌型的最小值，手动标记为0
        var scoreA = playerA.MaxHandData == (UInt64) CardEnum.A2345 ? 0 : playerA.MaxHandData;
        var scoreB = playerB.MaxHandData == (UInt64) CardEnum.A2345 ? 0 : playerB.MaxHandData;
        return GetWinner(scoreA, scoreB);
    }

}


public class Hand
{
    public string HandStr;   // 记录原始手牌字符串
    public UInt64[] Suits;   // 记录手牌中各花色对应的牌，数组索引0-3表示四种花色；每个花色由13个有效bit组成。
    public UInt64[] Faces;   // 记录手牌中某个点数的出现次数，数组索引0-3表示某个点数出现1-4次；每个出现频率由13个有效bit组成。
    
    // 获取最大手牌
    public MaxHand GetMaxHands()
    {
        var maxHand = new MaxHand { };
        if (maxHand.IsStraightFlush(this)) {
        } else if (maxHand.IsFourOfAKind(this)) {
        } else if (maxHand.IsFullHouse(this)) {
        } else if (maxHand.IsFlush(this)) {
        } else if (maxHand.IsStraight(this)) {
        } else if (maxHand.IsThreeOfAKind(this)) {
        } else if (maxHand.IsTwoPair(this)) {
        } else if (maxHand.IsOnePair(this)) {
        } else if (maxHand.IsHighCard(this)) {
        }

        return maxHand;
    }
}

public class MaxHand
{
    public CaseEnum MaxCase;      // 记录最大牌型
    public UInt64 MaxHandData;  // 记录最大五张牌和得分
    public bool FlushFlag;      // 记录是否存在5张同花牌型
    public int FlushSuit;       // 5张同花的花色编号
    
    
    // 筛选同花顺
    public bool IsStraightFlush(Hand hand)
    {
        // 依次遍历花色
        for (int i = 0; i < hand.Suits.Length; i++)
        {
            // 筛选相同花色牌个数，如果大于（5）则标记为同花
            UInt64 cardNum = PokerUtil.CountOne(hand.Suits[i]);
            if (cardNum >= 5)
            {
                this.FlushFlag = true;
                this.FlushSuit = i;
                // 再检查是否有顺子，若有则标记为同花顺
                UInt64 tempValue = PokerUtil.FindStraight(hand.Suits[i]); 
                if (tempValue > 0) {
                    if (this.MaxHandData == 0)
                    {
                        this.MaxHandData = tempValue;
                    } else {
                        // 可能7张同花顺，取最大的一组出来，需要特别注意A2345这个组合。
                        this.MaxHandData = tempValue > this.MaxHandData && tempValue != (uint)CardEnum.A2345? tempValue: this.MaxHandData;
                    }

                    this.MaxCase = CaseEnum.StraightFlush;
                }
            }
        }

        return this.MaxCase == CaseEnum.StraightFlush;
    }
    
    // 筛选四条
    public bool IsFourOfAKind(Hand hand) 
    {
        if (hand.Faces[3] > 0)
        {
            this.MaxCase = CaseEnum.FourOfAKind;
            // 取四条和单张牌中的最大牌
            this.MaxHandData = PokerUtil.LeftMoveAndAdd(hand.Faces[3], 4) | PokerUtil.GetFirstOne(hand.Faces[3] ^ hand.Faces[0]);
            return true;
        }

        return false;
    }
    
    // 筛选葫芦（三条+两条）
    public bool IsFullHouse(Hand hand) 
    {
        // 筛选一组三条和两组二条（三条必然占一组二条）
        if (hand.Faces[2] > 0 && PokerUtil.CountOne(hand.Faces[1]) >= 2)
        {
            this.MaxCase = CaseEnum.FullHouse;
            // 找出三条
            var firstOne = hand.Faces[2];
            // 找出二条，点数不能和三条一样
            var secondOne = PokerUtil.GetFirstOne(hand.Faces[2] ^ hand.Faces[1]);
            this.MaxHandData = PokerUtil.LeftMoveAndAdd(firstOne, 3) | PokerUtil.LeftMoveAndAdd(secondOne, 2);
            return true;
        }

        return false;
    }
    
    // 筛选同花（五张）
    public bool IsFlush(Hand hand) 
    {
        // 在IsStraightFlush中已经检测过是否存在5张同花了
        if (this.FlushFlag)
        {
            this.MaxCase = CaseEnum.Flush;
            // 直接把该花色的牌取出来即可
            this.MaxHandData = hand.Suits[this.FlushSuit];
            return true;
        }

        return false;
    }
    
    // 筛选顺子（五张）
    public bool IsStraight(Hand hand)
    {
        // 顺子都是单张，所以从Faces[0]取即可
        this.MaxHandData = PokerUtil.FindStraight(hand.Faces[0]);
        if (this.MaxHandData != 0) 
        {
            this.MaxCase = CaseEnum.Straight;
            return true;
        }
        return false;
    }
    
    // 筛选三条
   public bool IsThreeOfAKind(Hand hand) {
        if(hand.Faces[2] > 0)
        {
            this.MaxCase = CaseEnum.ThreeOfAKind; 
            // 找出三条
            var firstOne = PokerUtil.GetFirstOne(hand.Faces[2]);
            // 剔除掉两张最小的单牌
            this.MaxHandData = PokerUtil.LeftMoveAndAdd(firstOne, 3) |
                               PokerUtil.DeleteLastOne(hand.Faces[0] ^ firstOne, 2);
            return true;
        }

        return false;
   }
   
   // 筛选两对
   public bool IsTwoPair(Hand hand) 
   {
       // 找出有几对
       var countOne = PokerUtil.CountOne(hand.Faces[1]);
       // 如果多于两对，把较小的几对去掉
       if (countOne >= 2)
       {
           this.MaxCase = CaseEnum.TwoPair;
           var tempValue = PokerUtil.DeleteLastOne(hand.Faces[1], (int)(countOne - 2));
           this.MaxHandData = PokerUtil.LeftMoveAndAdd(tempValue, 2) |
                              PokerUtil.DeleteLastOne(hand.Faces[0] ^ tempValue, (int) (4 - countOne));
           return true;
       }

       return false;
   }
   
   // 筛选一对
   public bool IsOnePair(Hand hand) 
   {
       if (hand.Faces[1] > 0)
       {
           this.MaxCase = CaseEnum.OnePair;
           // 去掉最小的两张牌（不能是这个对子），保证总共五张牌
           this.MaxHandData = PokerUtil.LeftMoveAndAdd(hand.Faces[1], 2) | PokerUtil.DeleteLastOne(hand.Faces[0] ^ hand.Faces[1], 2);
           return true;
       }

       return false;
   }
    
    // 筛选高牌（2张底牌加上5张公共牌连一个对子都组合不了），直接去掉两张最小牌即可
    public bool IsHighCard(Hand hand)
    {
        this.MaxCase = CaseEnum.HighCard;
        this.MaxHandData = PokerUtil.DeleteLastOne(hand.Faces[0], 2);
        return true;
    }
}

public enum CaseEnum
{
    StraightFlush = 8, // 皇家同花顺&同花顺
    FourOfAKind   = 7, // 四条
    FullHouse     = 6, // 葫芦
    Flush         = 5, // 同花
    Straight      = 4, // 顺子
    ThreeOfAKind  = 3, // 三条
    TwoPair       = 2, // 两对
    OnePair       = 1, // 一对
    HighCard      = 0 // 散牌
}

public enum CardEnum
{
    A2345 = 4111, // 1000000001111
    AKQJT = 7936, // 1111100000000
    A     = 4096, // 1000000000000
}
    