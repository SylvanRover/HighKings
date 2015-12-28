using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour {

    [System.Serializable]
    public class UnitList {
        public string unitName;
        public int unitID;
        public float MAX_HP;
        public float hp;
        public float STRENGTH;
        public float VARIATION;
        public int SPEED;
        public int RANGE;
        public GameObject unitMesh;
    }

    public List<UnitList> commanderData;

}
