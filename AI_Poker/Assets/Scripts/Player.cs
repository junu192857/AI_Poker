using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

enum PokerHands { 
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
    private Card firstCard;
    private Card secondCard;

    //true if the player is human.
    public bool isHuman;

    private int CalculatePriority(List<Card> boardCards) {
        int priority;

        List<Card> allCards = boardCards;
        
        allCards.Add(firstCard);
        allCards.Add(secondCard);

        if (CheckStraightFlush(allCards, out priority)) return priority;
        throw new NotImplementedException();
    }

    private static bool CheckStraightFlush(List<Card> allCards, out int priority) {
        List<Card> temp = allCards.OrderBy(c => c.number).ToList();

        int sfMarker = 1;
        Shape sfShape = temp[0].shape;
        int sfFirst = temp[0].number == 14 ? 1 : temp[0].number;

        priority = 0;

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

    private static bool CheckStraight(List<Card> allCards, out int priority) {
        List<Card> temp = allCards.OrderBy(c => c.number).ToList();

        int straightMarker = 1;
        int straightFirst = temp[0].number == 14 ? 1 : temp[0].number;

        priority = 0;

        for (int k = 1; k < temp.Count; k++) {
            if (temp[k].number == straightMarker + straightFirst) straightMarker++;
            else {
                straightMarker = 1;
                straightFirst = temp[k].number;
            }

            if (straightMarker >= 5) priority = temp[k].number;

        }
        return priority > 0;
    }
}
