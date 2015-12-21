using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public int playerID;
    public int popCap;
    public int goldCurrent;
    public int goldMax;
    public int goldPerTurn;
    public int techAge = 0;

    [System.Serializable]
    public class Unit {
        public string Name;
        public int ID;
        public int Level;
    }

    public List<Unit> unitData;

}
