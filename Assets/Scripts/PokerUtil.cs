using System;


public static class PokerUtil
{
    // 统计二进制中1的个数（最大有效位数为16位）
    public static UInt64 CountOne(UInt64 a) {
        // 这里用了分治思想：先将相邻两个比特位１的个数相加，再将相邻四各比特位值相加...
        a = ((a & 0xAAAA) >> 1) + (a & 0x5555); // 1010101010101010  0101010101010101
        a = ((a & 0xCCCC) >> 2) + (a & 0x3333); // 1100110011001100  0011001100110011
        a = ((a & 0xF0F0) >> 4) + (a & 0x0F0F); // 1111000011110000  0000111100001111
        a = ((a & 0xFF00) >> 8) + (a & 0x00FF); // 1111111100000000  0000000011111111
        return a;
    }
    
    // 将数值左移后累加 func(100,2) 100 -> 100,100  func(100,3) 100 -> 100,100,100
    public static UInt64 LeftMoveAndAdd(UInt64 data, int moveCount)
    {
        UInt64 result = 0;
        for (int i = 0; i < moveCount; i++)
        {
            result |= data << (i * 13);
        }
        return result;
    }
    
    // 删除整形转二进制后最后n个1,并返回删除后的值 func(1011, 2)  10,11 -> 10,00
    public static UInt64 DeleteLastOne(UInt64 data, int deleteOneNum) {
        if (deleteOneNum <= 0)
        {
            return data;
        } else
        {
            deleteOneNum--;
            // 把最后一位1变成0
            return DeleteLastOne(data & (data - 1), deleteOneNum);
        }
    }
    
    // 获取整形转二进制后最高位1的值 func(1011) -> 1000
    public static UInt64 GetFirstOne(UInt64 data) {
        UInt64 result = 0;
        while (data > 0)
        {
            result = data;
            // 把最后一位1变成0
            data = data & (data - 1);
        }
        return result;
    }
    
    // 查找序列中可能存在的顺子，并返回牌面最大的一个
    // 从最大顺子"AKQJT"开始依次与牌面做匹配
    // 假设牌面cardface是	0000011011111    0000011011111    		 0000011011111
    // 用模板cardMold匹配 1111100000000 -> 0111110000000 -> ... -> 0000000011111
    public static UInt64 FindStraight(UInt64 data)
    {
        UInt64 cardNum;

        var cardMold = (UInt64)CardEnum.AKQJT;
        // 31 == 0000000011111，即顺子65432
        while(cardMold >= 31)
        {
            cardNum = CountOne(data & cardMold);
            if (cardNum >= 5) {
                return cardMold;
            }
            cardMold >>= 1;
        }

        // 最后判断"A2345"这一特殊情况
        cardMold = (UInt64)CardEnum.A2345;
        cardNum = CountOne(data & cardMold);
        if (cardNum >= 5)
        {
            return cardMold;
        }

        return 0;
    }

}
