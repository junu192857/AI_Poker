using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum PokerHands { 
    StraightFlush,
    FourCards,
    FullHouse,
    Flush,
    Straight,
    Triple,
    TwoPair,
    OnePair,
    HighCard
}

public class Player : MonoBehaviour
{
    //player's Hand-held cards
    public Card firstCard;
    public Card secondCard;

    public Player nextPlayer;

    public int money;
    public int betMoney;

    public bool isDead;
    public bool isAllIn;

    public bool betDone; // 베팅 완료 표시. betDone == true 이고 PokerManager.currentBetMoney == betMoney인 사람의 차례가 오면 한 번의 베팅이 끝나게 된다.


    public Player() {
        money = 100;
        betMoney = 0;
        firstCard = null;
        secondCard = null;
        isDead = false;
        isAllIn = false;
        betDone = false;
    }

    public void Check() {
        if (betMoney != PokerManager.currentBetMoney)
        {
            Debug.Log("Cannot Check. Please Call, Raise or Fold");
        }
        else betDone = true;
    }
    public void Call() {
        int added = PokerManager.currentBetMoney - betMoney;
        Pay(added);
    }
    public void Raise(int extraMoney) {
        Call();
        Pay(extraMoney);
    }
    public void Fold() {
        isDead = true;
    }

    public void Pay(int money) {
        if (this.money >= money)
        {
            this.money -= money;
            betMoney += money;
        }
        else { //All-in
            betMoney += this.money;
            this.money = 0;
            isAllIn = true;
        }
        PokerManager.currentBetMoney = betMoney;
    }

    public virtual void Bet(int blind) {
        if (isDead || isAllIn)
        {
            nextPlayer.Bet(blind + 1);
            betDone = false;
            return;
        }
        if (betDone && PokerManager.currentBetMoney == betMoney) {
            betDone = false;
            return;
        }
    }
    // 족보 -> 같은 족보 시 더 상위 숫자(priority) -> 키커 카드로 순위 결정.
    protected (PokerHands, int, int) CalculatePriority(List<Card> boardCards) {
        int priority, kicker;

        List<Card> allCards = boardCards;
        
        allCards.Add(firstCard);
        allCards.Add(secondCard);

        if (CheckStraightFlush(allCards, out priority, out kicker)) return (PokerHands.StraightFlush, priority, kicker);
        else if (CheckFourCard(allCards, out priority, out kicker)) return (PokerHands.FourCards, priority, kicker);
        else if (CheckFullHouse(allCards, out priority, out kicker)) return (PokerHands.FullHouse, priority, kicker);
        else if (CheckFlush(allCards, out priority, out kicker)) return (PokerHands.Flush, priority, kicker);
        else if (CheckStraight(allCards, out priority, out kicker)) return (PokerHands.Straight, priority, kicker);
        else if (CheckTriple(allCards, out priority, out kicker)) return (PokerHands.Triple, priority, kicker);
        else if (CheckTwoPair(allCards, out priority, out kicker)) return (PokerHands.TwoPair, priority, kicker);
        else if (CheckOnePair(allCards, out priority, out kicker)) return (PokerHands.OnePair, priority, kicker);
        else return CalculateHighCard(allCards);
    }

    public static PokerHands CalculatePriorityForTest(List<Card> allCards) {
        int priority;
        int kicker;

        if (CheckStraightFlush(allCards, out priority, out kicker)) return PokerHands.StraightFlush;
        else if (CheckFourCard(allCards, out priority, out kicker)) return PokerHands.FourCards;
        else if (CheckFullHouse(allCards, out priority, out kicker)) return PokerHands.FullHouse;
        else if (CheckFlush(allCards, out priority, out kicker)) return PokerHands.Flush;
        else if (CheckStraight(allCards, out priority, out kicker)) return PokerHands.Straight;
        else if (CheckTriple(allCards, out priority, out kicker)) return PokerHands.Triple;
        else if (CheckTwoPair(allCards, out priority, out kicker)) return PokerHands.TwoPair;
        else if (CheckOnePair(allCards, out priority, out kicker)) return PokerHands.OnePair;
        else return PokerHands.HighCard;
    }

    //스트레이트 플러시의 가장 높은 숫자를 반환. 키커는 0
    protected static bool CheckStraightFlush(List<Card> allCards, out int priority, out int kicker) {
        List<Card> temp = allCards.OrderBy(c => c.number).ToList();

        int sfMarker = 1;
        Shape sfShape = temp[0].shape;
        int sfFirst = temp[0].number;

        priority = 0;
        kicker = 0;

        for (int k = 1; k < temp.Count; k++)
        {
            if (temp[k].number == sfMarker + sfFirst && temp[k].shape == sfShape) sfMarker++;
            else
            {
                sfMarker = 1;
                sfFirst = temp[k].number;
                sfShape = temp[k].shape;
            }

            if (sfMarker == 4 && temp[k].number == 5 && temp.FindAll(c => c.number == 14 && c.shape == sfShape).Count > 0) priority = temp[k].number;
            if (sfMarker >= 5) priority = temp[k].number;
        }
        return priority > 0;
    }

    //포카드에 해당하는 숫자 및 키커 1개의 숫자를 반환
    protected static bool CheckFourCard(List<Card> allCards, out int priority, out int kicker) {
        int[] count = new int[15];

        priority = 0;
        kicker = 0;

        foreach (Card c in allCards) { 
            count[c.number]++;
        }

        for (int k = 14; k >= 2; k--) { 
            if (count[k] == 4 && priority == 0)
            {
                priority = k;
            }
            else if (count[k] > 0 && kicker == 0) kicker = k;
        }
        return priority > 0;
    }

    protected static bool CheckFullHouse(List<Card> allCards, out int priority, out int kicker) {
        kicker = 0;

        if (CheckTriple(allCards, out int triple, out int dummy)) {
            List<Card> temp = allCards.FindAll(c => c.number != triple).ToList();
            if (CheckOnePair(temp, out int pair, out int dummy2)) {
                priority = 100 * triple + pair;
                return true;
            }
        }

        throw new NotImplementedException();
    }

    protected static bool CheckFlush(List<Card> allCards, out int priority, out int kicker) {
        List<Card> temp = allCards.OrderBy(c => c.number).ToList();
        int[] shapeCount = new int[4];
        priority = 0;
        kicker = 0;

        foreach (Card card in allCards) {
            shapeCount[(int)card.shape]++;
        }

        for (int i = 0; i < shapeCount.Length; i++) {
            if (shapeCount[i] >= 5) {
                priority = temp.FindLast(card => (int)card.shape == i).number;
                break;
            }
        }
        return priority > 0;
    }

    //트리플의 가장 높은 숫자와 함께 키커 2개의 숫자를 (가장 높은 키커 * 100 + 두 번째 키커)의 식으로 반환.
    protected static bool CheckTriple(List<Card> allCards, out int priority, out int kicker) {
        int[] count = new int[15];
        int kicker1 = 0;
        int kicker2 = 0;

        priority = 0;
        kicker = 0;

        foreach (Card c in allCards)
        {
            count[c.number]++;
        }

        for (int k = 14; k >= 2; k--)
        {
            if (count[k] >= 3 && priority == 0)
            {
                priority = k;
                count[k] -= 3;
            }
        }
        for (int k = 14; k >= 2; k--)
        {
            while (count[k] > 0)
            {
                count[k]--;
                if (kicker1 == 0) kicker1 = k;
                else if (kicker2 == 0) kicker2 = k;
                else break;
            }
        }
        kicker = 100 * kicker1 + kicker2;

        return priority > 0;
    }

    //두 개의 페어를 priority로 반환, 하나의 키커를 kicker로 반환
    protected static bool CheckTwoPair(List<Card> allCards, out int priority, out int kicker) {
        int[] count = new int[15];

        int pair1 = 0;
        int pair2 = 0;

        priority = 0;
        kicker = 0;

        foreach (Card c in allCards)
        {
            count[c.number]++;
        }

        for (int k = 14; k >= 2; k--)
        {
            if (count[k] >= 2)
            {
                if (pair1 == 0) {
                    pair1 = k;
                    count[k] -= 2;
                }
                else if (pair2 == 0)
                {
                    pair2 = k;
                    count[k] -= 2;
                    break;
                }
            }
        }
        for (int k = 14; k >= 2; k--) {
            if (count[k] > 0 && kicker == 0) {
                kicker = k;
                break;
            }
        }

        priority = 100 * pair1 + pair2;

        return priority % 100 != 0;
    }

    //하나의 페어와 3개의 키커를 반환.
    protected static bool CheckOnePair(List<Card> allCards, out int priority, out int kicker) {
        int[] count = new int[15];

        priority = 0;
        kicker = 0;
        int[] kickers = new int[3];

        foreach (Card c in allCards) {
            count[c.number]++;
        }

        for (int k = 14; k >= 2; k--) {
            if (count[k] >= 2)
            {
                if (priority == 0) { 
                    priority = k;
                    count[k] -= 2;
                    break;
                }
            }
        }

        for (int k = 14; k >= 2; k--)
        {
            while (count[k] > 0)
            {
                count[k]--;
                if (kickers[0] == 0) kickers[0] = k;
                else if (kickers[1] == 0) kickers[1] = k;
                else if (kickers[2] == 0) kickers[2] = k;
                else break;
            }
        }

        kicker = 10000 * kickers[0] + 100 * kickers[1] + kickers[2];
        return priority > 0;
    }

    //스트레이트의 가장 높은 숫자를 반환. 키커는 0
    protected static bool CheckStraight(List<Card> allCards, out int priority, out int kicker) {
        List<Card> temp = allCards.OrderBy(c => c.number).ToList();

        int straightMarker = 1;
        int straightFirst = temp[0].number == 14 ? 1 : temp[0].number;

        priority = 0;
        kicker = 0;

        for (int k = 1; k < temp.Count; k++) {
            if (temp[k].number == straightMarker + straightFirst) straightMarker++;
            else {
                straightMarker = 1;
                straightFirst = temp[k].number;
            }

            if (straightMarker == 4 && temp[k].number == 5 && temp.FindAll(c => c.number == 14).Count > 0) priority = temp[k].number;
            if (straightMarker >= 5) priority = temp[k].number;

        }
        return priority > 0;
    }

    protected static (PokerHands, int, int) CalculateHighCard(List<Card> allCards) {
        List<Card> temp = allCards.OrderBy(c => c.number).ToList();

        int priority = 0;
        for (int i = 3; i <= 7; i++) {
            priority += (int)Mathf.Pow(15, i - 3) * temp[i].number;
        }
        return (PokerHands.HighCard, priority, 0);
    }
}
