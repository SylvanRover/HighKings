using UnityEngine;
using System.Collections;

public class TroopStats : MonoBehaviour {

	public float healthMax = 100;
	public float healthCurrent = 100;
	public float attackPoints = 10;
	public float attackRange = 20;
	public float armourPoints = 0;
	public float LoS = 25;
	public float speed = 5;
	public float size =1;
	public string troopType = "infantry";
	public string combatType = "melee";
	public string target = "any";
	public float cost = 20;
	public float carry = 10;
	public string uniqueTrait = "None";

	// Use this for initialization
	void Start () {
		healthCurrent = healthMax;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
