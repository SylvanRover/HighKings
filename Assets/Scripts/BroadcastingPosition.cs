using UnityEngine;
using System.Collections;
using Map.Events;

public class BroadcastingPosition : MonoBehaviour {

	private bool myAttack = true;

	public void BroadCastMyPosition () {
		InteractionEvents.BroadcastPosition (this.transform.position, this.tag);
	}
	public void BroadCastMySendAttack () {
		InteractionEvents.BroadcastSendAttack (this.transform.position, myAttack);
	}
}

namespace Map.Events {
	partial class InteractionEvents {
		
		public delegate void SimpleVectorEvent(Vector3 pos, string targetType);
		private static SimpleVectorEvent OnBroadcastPositionDelegate;
		public static event SimpleVectorEvent OnBroadcastPosition {
			add {
				OnBroadcastPositionDelegate -= value;
				OnBroadcastPositionDelegate += value;
			} remove {
				OnBroadcastPositionDelegate -= value;
			}
		}
		public static void BroadcastPosition(Vector3 pos, string targetType) {
			if(OnBroadcastPositionDelegate != null) {
				OnBroadcastPositionDelegate(pos, targetType);
			}
		}

		public delegate void SimpleAttackEvent(Vector3 pos,bool attack);
		private static SimpleAttackEvent OnBroadcastSendAttackDelegate;
		public static event SimpleAttackEvent OnBroadcastSendAttack {
			add {
				OnBroadcastSendAttackDelegate -= value;
				OnBroadcastSendAttackDelegate += value;
			} remove {
				OnBroadcastSendAttackDelegate -= value;
			}
		}
		public static void BroadcastSendAttack(Vector3 pos,bool attack) {
			if(OnBroadcastSendAttackDelegate != null) {
				OnBroadcastSendAttackDelegate(pos, attack);
			}
		}
	}
}