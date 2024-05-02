using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Shape { 
    Spade = 3,
    Diamond = 2,
    Heart = 1,
    Clover = 0
}

public class Card
{
    public Shape shape;
    public int number; //Ace is 14

    public Card(Shape shape, int number) {
        this.shape = shape;
        if (number < 2 || 14 < number) Debug.LogError("Invalid Number!");
        this.number = number;
    }

    public string GetNumber() => number switch
        {
            14 => "A",
            13 => "K",
            12 => "Q",
            11 => "J",
            _ => number.ToString()
        };
}
