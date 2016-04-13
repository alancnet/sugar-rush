using UnityEngine;
using System.Collections;

public class FocusController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var y = Input.GetAxis("Horizontal");
        var x = Input.GetAxis("Vertical");

        this.transform.Rotate(x, y, 0);
	}
}
