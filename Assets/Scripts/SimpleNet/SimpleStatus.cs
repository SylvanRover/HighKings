using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Text;


public class SimpleStatus : MonoBehaviour {

	public bool useJson = true;

	private Dropzone[] _dropZones;
	public SimpleNet _simpleNet;

	void Start(){
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("MapDropZone");
		_dropZones = new Dropzone[objs.Length];
		for(int i=0; i<objs.Length; i++){
			_dropZones [i] = objs [i].GetComponent<Dropzone> ();
		}
		_simpleNet.dataListener += ReceiveData;
	}

	void OnDestroy(){
		_simpleNet.dataListener -= ReceiveData;
	}

	[Serializable]
	public struct PlayUnitState {
		public int uniqueID;
		public Unit.UnitType unitType;
		public int hexPositionU;
		public int hexPositionV;
		public float health;
		public int playerOwner;
	}

	[Serializable]
	public struct DropZoneState {
		public bool isCapturePoint;
		public int playerOccupier;
		public bool owned;
		//public PlayUnit occupierUnit;
	}

	[Serializable]
	public class NetworkData {
		public DropZoneState[] dropzones;
		public PlayUnitState[] units;
	}

	public PlayUnitState[] GetUnits(){
		// Super inefficient but for now will work
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Unit");
		PlayUnitState[] units = new PlayUnitState[objs.Length];
		for(int i=0; i<objs.Length; i++){
			units[i]=objs [i].GetComponent<Unit>().GetState();
		}
		return units;
	}

	public DropZoneState[] GetStates(){
		DropZoneState[] states = new DropZoneState[_dropZones.Length];
		for(int i=0; i<_dropZones.Length; i++){
			states[i]=_dropZones [i].GetState();
		}
		return states;
	}

	void UpdateAllDropZones (DropZoneState[] states) {
		for(int i=0; i<states.Length; i++){
			_dropZones [i].UpdateState (states[i]);
		}
	}

	void UpdateAllUnits(PlayUnitState[] states){
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Unit"); // We could use the hex grid.units
		for(int i=0; i<states.Length; i++){
			int id = states [i].uniqueID;
			bool found = false;
			for (int j = 0; j < objs.Length; j++) {
				Unit unit = objs [j].GetComponent<Unit> ();
				if (unit.GetState ().uniqueID == id) {
					unit.UpdateState (states[i]);
					found = true;
					break;
				}
			}
			if (!found) {

				Debug.LogError ("Unit must be new");
				// If can't find then is a new unit, so spawn one and then update
				//unit.UpdateState (states[i]);
			}
		}
	}

	public void ReceiveData(int connectionID, byte[] recBuffer, int bufferSize, int dataSize){
		//Debug.LogError ("GOT DATA HERE");
		NetworkData holder;
		if (useJson) {
			//byte[] smallerBuffer = new byte[dataSize];
			//Array.Copy (recBuffer, smallerBuffer, dataSize);
			string json = System.Text.Encoding.ASCII.GetString (recBuffer, 0, dataSize);
			/*
			Char[] test = json.ToCharArray ();
			for (int i = 0; i < test.Length; i++) {
				Debug.LogError (test[i]);
			}
			int splitLengthTest = json.Length / 4;
			for (int j = 0; j < 3; j++) {
				Debug.LogError (json.Substring(j*splitLengthTest,splitLengthTest));
			}
			*/
			holder = JsonUtility.FromJson<NetworkData>(json);
		} else {
			MemoryStream memoryStream = new MemoryStream (recBuffer);
			BinaryFormatter binaryFormatter = new BinaryFormatter ();
			holder = binaryFormatter.Deserialize (memoryStream) as NetworkData;
		}

		UpdateAllDropZones (holder.dropzones);
		UpdateAllUnits (holder.units);
	}

	public void EndTurn(){
		// This is inefficient but will work in prototyping
		NetworkData statesHolder = new NetworkData ();
		statesHolder.dropzones = GetStates ();
		statesHolder.units = GetUnits ();
		if (useJson) {
			string json = JsonUtility.ToJson (statesHolder);
			byte[] byteData = Encoding.ASCII.GetBytes (json);
			_simpleNet.SendReliableData (byteData, byteData.Length);
		} else {
			int bufferSize = 1024;
			byte[] buffer = new byte[bufferSize];
			Stream stream = new MemoryStream(buffer);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, statesHolder);
			//TODO: send
			_simpleNet.SendReliableData (buffer, buffer.Length);//Might be too long?
		}
	}
}
