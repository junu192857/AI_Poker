using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = System.Random;

public class PokerManager : MonoBehaviour
{
    private List<Card> deck;

    private void Start()
    {
        Initialize();
    }

    // ======== Initializing ========
    private void Initialize()
    {
        ResetDeck();
    }

    // ResetDeck: Initialize 52 cards for deck and shuffle it
    private void ResetDeck() {
        var shapes = Enum.GetValues(typeof(Shape)).Cast<Shape>();
        foreach (var shape in shapes) {
            for (int i = 2; i <= 14; i++) {
                deck.Add(new Card(shape, i));
            }
        }

        Random rand = new Random();

        for (int j = deck.Count - 1; j >= 0; j--) {
            int k = rand.Next(j + 1);
            Card c = deck[k];
            deck[k] = deck[j];
            deck[j] = c;
        }
    }
}
