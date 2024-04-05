using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Shape { 
    Spade,
    Diamond,
    Heart,
    Clover
}

public class Card
{
    public Shape shape;
    public int number; //Ace is 14

    public Card(Shape shape, int number) {
        this.shape = shape;
        if (number < 2 || 14 > number) Debug.LogError("Invalid Number!");
        this.number = number;
    }
}
