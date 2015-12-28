using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
using System.Text;

public class SimpleNet : MonoBehaviour {

	public Text _debug;
	public Toggle _server;
	//Client
	public Text _socketIdText;
	public Text _socketPortText;

	private int _socketId;
	private int _socketPort;
	private int _reiliableChannelId;  
	private int _unreliableChannelId;

	//Server
	public Text _serverIPAddressText;
	private string _serverIPAdddress;

	public Text _serverSocketPortText;
	private int _serverSocketPort;

	public Text _serverSocketIdText;
	private int _serverSocketId;

	public bool useDebug = false;
	public int debugSocketID = 0;
	public int debugConnectionID = 0;
	public int debugChannelID = 0;

	//private int _webHostId;
	private int _maxConnections = 2;
	private int _connectionId;
	private int _otherPlayerConnectionId; //Testing
	private int _otherPlayerHostId; //Testing
	private int _otherPlayerChannel; //Testing
	private int maxBufferSize = 8192;

	public static int PlayerID = 0;

	public delegate void DataListener( int connectionID, byte[] recBuffer, int bufferSize, int dataSize);
	public DataListener dataListener;

	void Start () {
		// An example of initializing the Transport Layer with custom settings
		GlobalConfig gConfig = new GlobalConfig();
		gConfig.MaxPacketSize = 8192; //Needs to be large for json
		NetworkTransport.Init(gConfig);
	}

	public void CreateSocket(){
		if (_server.isOn) {
			CreateServerSocket ();
			//CreateClientSocket (); // If want a server and client on same machine
		} else {
			CreateClientSocket ();
		}
	}

	private void CreateServerSocket(){
		Debug.Log("Create Server Socket");
		ConnectionConfig config = new ConnectionConfig();
		_reiliableChannelId = config.AddChannel(QosType.Reliable);

		HostTopology topology = new HostTopology(config, _maxConnections);
		_socketPort = 8888;//Int32.Parse(_socketPortText.text);
		_socketId = NetworkTransport.AddHost(topology, _socketPort);

		_debug.text = "Socket Open. SocketId is: " + _socketId + " port:" + _socketPort;
		Debug.Log("Socket Open. SocketId is: " + _socketId+" port:"+_socketPort);
	}

	private void CreateClientSocket(){
		Debug.Log("Create Client Socket");
		// Create our socket
		ConnectionConfig config = new ConnectionConfig();
		_reiliableChannelId = config.AddChannel(QosType.Reliable);
		HostTopology topology = new HostTopology(config, 1);
		_socketPort = _server.isOn? 8890 : 8889;// Int32.Parse(_socketPortText.text);
		_socketId = NetworkTransport.AddHost(topology, _socketPort);
		_debug.text = "Socket Open. SocketId is: " + _socketId + " port:" + _socketPort;
		Debug.Log("Socket Open. SocketId is: " + _socketId+" port:"+_socketPort);

		ConnectToServer ();
	}

	public void ConnectToServer(){
		_serverIPAdddress = _serverIPAddressText.text;

		byte error;
		_connectionId = NetworkTransport.Connect(_socketId, _serverIPAdddress, 8888, 0, out error);
		PlayerID = _connectionId;
		Debug.LogError("Connected to server. ConnectionId: " + _connectionId+" port:"+_socketPort);
		_debug.text += "\nConnected to server. ConnectionId: " + _connectionId+" port:"+_socketPort;
		NetworkError netError = (NetworkError)error;
		if (netError != NetworkError.Ok) {
			_debug.text = netError.ToString ();
			_debug.text += "\nConnectToServer: " + netError.ToString ();
		}
	}

	public void SendReliableData(byte[] buffer, int bufferLength){
		byte error;
		int connectionId;
		int socketId;
		int channel;
		if (_server.isOn) {
			connectionId = useDebug ? debugConnectionID : _otherPlayerConnectionId;
			socketId = useDebug ? debugSocketID : _otherPlayerHostId;
			channel = useDebug ? debugChannelID : _otherPlayerChannel;
		} else {
			connectionId = _connectionId;
			socketId = _socketId;
			channel = _reiliableChannelId;
		}
		//TODO: move to broadcast system if players are to be >2
		NetworkTransport.Send(socketId, connectionId, channel, buffer, bufferLength,  out error);
		NetworkError netError = (NetworkError)error;
		_debug.text += "\nSendReliableData: "+netError.ToString ();
		Debug.LogError (netError.ToString ());
	}
		
	public void Disconnect () {
		byte error;
		NetworkTransport.Disconnect(_socketId, _connectionId, out error);
		NetworkError netError = (NetworkError)error;
		_debug.text = "Disconnect: "+netError.ToString();
		Debug.LogError (netError.ToString ());
	}

	void Update()
	{
		int recHostId; 
		int connectionId; 
		int channelId; 
		byte[] recBuffer = new byte[maxBufferSize]; 
		int bufferSize = maxBufferSize;
		int dataSize;
		byte error;
		NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

		switch (recData)
		{
		case NetworkEventType.Nothing:
			break;
		case NetworkEventType.ConnectEvent:
			if(_connectionId == connectionId){
				//my active connect request approved
				Debug.LogError("my active connect request approved");
			}else{
				_otherPlayerConnectionId = connectionId;
				_otherPlayerHostId = recHostId;
				_otherPlayerChannel = channelId;
				//somebody else connect to me
				Debug.LogError(string.Format("Host {0} connected to me. {1}, chan:{2}", _otherPlayerHostId, _otherPlayerHostId,_otherPlayerChannel));
				_debug.text = (string.Format("Host {0} connected to me. {1} chan:{2}", _otherPlayerHostId, _otherPlayerHostId,_otherPlayerChannel));
			}
			break;
		case NetworkEventType.DataEvent:

			_debug.text = string.Format("Data: got some from host:{0} conn:{1} chan:{2} ",recHostId,connectionId,channelId) ;
			if (this.dataListener!=null){
				dataListener (connectionId, recBuffer, bufferSize, dataSize);
			}
			break;
		case NetworkEventType.DisconnectEvent:
			if(_connectionId == connectionId){

				NetworkError netError = (NetworkError)error;
				//my active connect request approved
				Debug.LogError("cannot connect for some reason see error: "+netError.ToString ());
				//connection which identified by connectionId cannot be established
				_debug.text ="cannot connect for some reason see error: "+netError.ToString ();

				if (netError == NetworkError.VersionMismatch) {
					Debug.LogError ("transport protocol is different");
					_debug.text ="transport protocol is different: "+netError.ToString ();
				} else if (netError == NetworkError.CRCMismatch) {
					Debug.LogError ("peer has different network configuration");
					_debug.text = "peer has different network configuration: "+netError.ToString ();
				} else if (netError == NetworkError.Timeout) {
					_debug.text = "cannot connect to other peer in period of time, possible peer is not running: "+netError.ToString ();
					Debug.LogError ("cannot connect to other peer in period of time, possible peer is not running");
				} else {
					_debug.text = "Disconnect: "+netError.ToString ();
					Debug.LogError (netError.ToString ());
				}
			}else{
				Debug.LogError("one of the established connection has been disconnected");
			}
			break;
		}
	}
}
