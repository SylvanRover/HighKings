using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerritoryData : MonoBehaviour {
	[System.Serializable]
	public class Territory {
		public string RealmNumber;
		public string Name;
		public string AllianceName;
	}

	public List<Territory> territoryData;

	public void ViewedTerritory(GameObject realm) {
		
	}

	static Territory ViewingRealm;
}
