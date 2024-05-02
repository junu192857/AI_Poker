using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelOneAI : Player
{
    public override void Bet(int blind)
    {
        base.Bet(blind);
        switch (PokerManager.state)
        {
            case PokerState.PreFlop:
                if (blind == 1 && blind == 2) Raise(1);//SB: 0에서 1로 베팅, BB: 1에서 2로 베팅
                throw new NotImplementedException();
        }
        betDone = true;
        nextPlayer.Bet(blind + 1);
        betDone = false;
        return;
    }
}
