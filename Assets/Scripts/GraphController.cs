using UnityEngine;
using System.Collections;

public class GraphController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void BroadcastVertexAt(Vector3 v)
    {
        BroadcastMessage("VertexAt", v);
    }
}
