using UnityEngine;
using System.Collections;

public class RoundManager : MonoBehaviour {

    public int currentPlayer = 0;
    public int activePlayers = 1;

    public void NextPlayerMove() {
        if (currentPlayer < activePlayers) {
            currentPlayer++;
        } else {
            currentPlayer = 0;
        }
    }

}
