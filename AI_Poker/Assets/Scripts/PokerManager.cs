using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = System.Random;

public enum PokerState
{
    PreFlop,
    Flop,
    Turn,
    River
}
public class PokerManager : MonoBehaviour
{
    private List<Card> deck;

    private int sbIndex;

    public static PokerState state;
    public static List<Card> boardCards;
    public static int currentBetMoney; // 진행을 위해 베팅해야 할 최소 금액.

    [SerializeField] private List<Player> players;

    [SerializeField] private Players preInfo;

    private void Start()
    {
        players = new List<Player>();
        sbIndex = 0;
        Initialize();
    }

    // ======== Initializing ========
    private void Initialize()
    {
        currentBetMoney = 0;
        state = PokerState.PreFlop;
        ResetDeck();
        PreparePlayers(); // First player will be BB, second player will be SB

        DoPreFlop();
        DoFlop();
        DoTurn();
        DoRiver();

        //TODO: 라운드 종료 처리하기 (River까지 정상적으로 끝난 케이스 / 중간에 혼자 살아남은 케이스)
        //TODO: 라운드 종료 후 정산 + SB, BB 바꾸기
        //TODO: 새 라운드 시작: PokerManager, Player의 필드 초기화
    }

    private void DoPreFlop() {
        players[sbIndex].Bet(1);
    }

    private void DoFlop() {
        boardCards.Add(Pop());
        state = PokerState.Flop;
        players[sbIndex + 1 % players.Count].Bet(2);
    }
    private void DoTurn() {
        boardCards.Add(Pop());
        state = PokerState.Turn;
        players[sbIndex + 1 % players.Count].Bet(2);
    }
    private void DoRiver() {
        boardCards.Add(Pop());
        state = PokerState.River;
        players[sbIndex + 1 % players.Count].Bet(2);
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

    

    private void PreparePlayers() {
        foreach (int i in preInfo.players)
        {
            switch (i)
            {
                case 0:
                    break;
                case 1:
                    players.Add(new LevelOneAI());
                    break;
                case 2:
                    players.Add(new LevelTwoAI());
                    break;
                case 3:
                    players.Add(new LevelThreeAI());
                    break;
                case 4:
                    players.Add(new LevelFourAI());
                    break;
                case 5:
                    players.Add(new LevelFiveAI());
                    break;
                case 6:
                    players.Add(new Human());
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < players.Count; i++) { 
            players[i].nextPlayer = players[(i+1) % players.Count];
            players[i].firstCard = Pop();
            players[i].secondCard = Pop();
        }
    }

    private Card Pop() {
        Card c = deck[0];
        deck.RemoveAt(0);
        return c;
    }
}
