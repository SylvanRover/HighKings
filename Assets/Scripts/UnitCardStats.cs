using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitCardStats : MonoBehaviour {

    public int unitID;
    public int ownership = 1;
    public string unitName;
    public int unitCost = 1;
    public Text unitNameText;
    public Text unitCostText;

    void Start() {
        unitNameText.text = unitName;
        unitCostText.text = unitCost.ToString();
    }

}
