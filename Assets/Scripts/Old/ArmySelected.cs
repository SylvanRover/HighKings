using UnityEngine;
using System.Collections;
using Map.Events;

public class ArmySelected : MonoBehaviour {

	public ArmyStats stats;

	public void BroadcastSelected () {
		InteractionEvents.BroadcastSelected (stats.commanderID, stats.commanderButtonSprite, stats.transform, stats);
	}

	public void BroadcastSwitch () {
		InteractionEvents.BroadcastSwitch (stats.commanderID);
	}
}

namespace Map.Events {
	partial class InteractionEvents {

		// Commander Broadcast
		public delegate void SimpleBoolEvent(int commanderID, Sprite commanderSprite, Transform commanderPos, ArmyStats commanderStats);
		private static SimpleBoolEvent OnBroadcastSelectedDelegate;
		public static event SimpleBoolEvent OnBroadcastSelected {
			add {
				OnBroadcastSelectedDelegate -= value;
				OnBroadcastSelectedDelegate += value;
			} remove {
				OnBroadcastSelectedDelegate -= value;
			}
		}
		public static void BroadcastSelected(int commanderID, Sprite commanderSprite, Transform commanderPos, ArmyStats commanderStats) {
			if(OnBroadcastSelectedDelegate != null) {
				OnBroadcastSelectedDelegate(commanderID, commanderSprite, commanderPos, commanderStats);
			}
		}

		// MGMT Broadcast
		public delegate void SimpleSwitchEvent(int commanderID);
		private static SimpleSwitchEvent OnBroadcastSwitchDelegate;
		public static event SimpleSwitchEvent OnBroadcastSwitch {
			add {
				OnBroadcastSwitchDelegate -= value;
				OnBroadcastSwitchDelegate += value;
			} remove {
				OnBroadcastSwitchDelegate -= value;
			}
		}
		public static void BroadcastSwitch(int commanderID) {
			if(OnBroadcastSwitchDelegate != null) {
				OnBroadcastSwitchDelegate(commanderID);
			}
		}
	}
}