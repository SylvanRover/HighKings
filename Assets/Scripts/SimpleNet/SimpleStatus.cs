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
	public enum UnitType{
		Infantry,
		Ranged,
		Mounted
	}

	[Serializable]
	public struct PlayUnit {
		public UnitType type;
		public int health;
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
		public DropZoneState[] states;
		public PlayUnit[] units;
	}

	/*public DropZoneState[] GetStates(){
		DropZoneState[] states = new DropZoneState[_dropZones.Length];
		for(int i=0; i<_dropZones.Length; i++){
			states[i]=_dropZones [i].GetState();
		}
		return states;
	}*/

	/*void UpdateAllDropZones (DropZoneState[] states) {
		Debug.LogError ("DropZoneState GOT DATA HERE");
		for(int i=0; i<states.Length; i++){
			_dropZones [i].UpdateState (states[i]);
		}
	}*/

	void UpdateAllUnits(PlayUnit[] units){
		Debug.LogError ("PlayUnit GOT DATA HERE");
		//TODO: something with units
	}

	public void ReceiveData(int connectionID, byte[] recBuffer, int bufferSize, int dataSize){
		Debug.LogError ("GOT DATA HERE");
		NetworkData holder;
		if (useJson) {
			byte[] smallerBuffer = new byte[bufferSize];
			Array.Copy (recBuffer, smallerBuffer, bufferSize);
			string json = System.Text.Encoding.UTF8.GetString (smallerBuffer);
			holder = (JsonUtility.FromJson<NetworkData> (json));
		} else {
			MemoryStream memoryStream = new MemoryStream (recBuffer);
			BinaryFormatter binaryFormatter = new BinaryFormatter ();
			holder = binaryFormatter.Deserialize (memoryStream) as NetworkData;
		}
		//UpdateAllDropZones (holder.states);
		UpdateAllUnits (holder.units);
	}

	public void EndTurn(){
		// This is inefficient but will work in prototyping
		NetworkData statesHolder = new NetworkData ();
		//statesHolder.states = GetStates ();

		if (useJson) {
			string json = JsonUtility.ToJson (statesHolder);
			byte[] byteData = Encoding.UTF8.GetBytes (json);
			_simpleNet.SendReliableData (byteData, byteData.Length);
		} else {
			int bufferSize = 1024;
			byte[] buffer = new byte[bufferSize];
			Stream stream = new MemoryStream(buffer);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, statesHolder);
		}
	}
}
