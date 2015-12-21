using UnityEngine;
using System.Collections;

public class AttackBannerInstantiator : MonoBehaviour {

	public GameObject AttackBanner;
	
	// Use this for initialization
	public void SpawnAttackBanner () {
		GameObject go = GameObject.Instantiate (AttackBanner);
		go.transform.parent = this.transform;
		go.transform.localPosition = Vector3.zero;
		//go.transform.localScale = Vector3.one;
		go.transform.localRotation = Quaternion.identity;
	}
	
}
