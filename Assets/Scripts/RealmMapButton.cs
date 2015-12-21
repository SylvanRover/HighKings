using UnityEngine;
using System.Collections;

public class RealmMapButton : MonoBehaviour {
	// Use this for initialization
	TerritoryData dataHolder;
	public Transform scaleTrans;
	public UnityEngine.UI.Text text;
	public UnityEngine.UI.Text allianceText;

	void Start () {
		dataHolder = FindObjectOfType<TerritoryData> ();

		int index = Random.Range (0, dataHolder.territoryData.Count);
		text.text = dataHolder.territoryData[index].Name;
		allianceText.text = string.IsNullOrEmpty(dataHolder.territoryData[index].AllianceName) ? "Contested Realm" : ("Realm of " + dataHolder.territoryData[index].AllianceName);
		RectTransform trans = scaleTrans as RectTransform;
		if (text.text.Length >= allianceText.text.Length) {
			trans.sizeDelta = new Vector2 (Mathf.Max (33 * text.text.Length, 300), trans.sizeDelta.y);
		} else {
			trans.sizeDelta = new Vector2 (Mathf.Max (25 * allianceText.text.Length, 300), trans.sizeDelta.y);
		}
	}
}
