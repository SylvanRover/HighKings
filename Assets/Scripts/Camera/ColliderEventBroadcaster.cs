using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ColliderEventBroadcaster : MonoBehaviour {
    public UnityEvent onClick = new UnityEvent();
    public UnityEvent onButtonHeld = new UnityEvent();
}

