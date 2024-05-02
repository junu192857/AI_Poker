using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class ZokboTest : MonoBehaviour
{
    [SerializeField] private Text zokboText;

    private Card[] cards = new Card[7];

    [SerializeField] private Text[] cardText = new Text[7]; // Pre-set array of length 7.


    public void Test() {
        if (CheckEmptySlot() >= 0) {
            Debug.Log("Fill the seven cards");
        }
        else
        {
            List<Card> myList = cards.ToList();
            PokerHands hand = Player.CalculatePriorityForTest(myList);
            Debug.Log(hand);
        }
    }

    public void putCard(int cardNum)
    {
        if (CheckEmptySlot() < 0) { 
            Debug.LogError("Cards are already full");
            return;
        }

        //cardNum == 15 * shape + number(2~14)
        if (cardNum / 15 < 0 || cardNum / 15 >= 4) { 
            Debug.LogError($"CardNum {cardNum} is invalid: invalid shape");
            return;
        }

        if (cardNum % 15 < 2) {
            Debug.LogError($"CardNum {cardNum} is invalid: invalid number. Remind that Ace is 14");
            return;
        }



        Card myCard = new Card((Shape)(cardNum / 15), cardNum % 15);



        foreach (Card card in cards)
        {
            if (card == null) continue;
            if (card.shape == myCard.shape && card.number == myCard.number)
            {
                Debug.Log("This card already Exists");
                return;
            }
        }

        int empty = CheckEmptySlot();
        if (empty == -1)
        {
            Debug.Log("Slot is Full");
            return;
        }
        else {
            cards[empty] = myCard;
            string cardinfo = myCard.shape.ToString()[0].ToString() + myCard.GetNumber();
            cardText[empty].text = cardinfo;
        }


        
    
    }

    //선택한 카드 삭제하기.
    public void removeCard(int cardIndex) {
        cardText[cardIndex].text = "";
        cards[cardIndex] = null;
    }

    private int CheckEmptySlot() {
        for (int i = 0; i < 7; i++)
        {
            if (cards[i] == null) return i;
        }
        return -1;
    }
       
}
