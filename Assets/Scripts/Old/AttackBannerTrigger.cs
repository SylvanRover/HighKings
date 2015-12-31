using UnityEngine;
using System.Collections;

public class AttackBannerTrigger : MonoBehaviour {

	public MakePrefabAppear spawnFX;

	public void SpawnFXatPoint () {
		spawnFX.Appear();
	}

}
