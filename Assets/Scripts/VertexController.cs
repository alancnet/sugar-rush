using UnityEngine;
using System.Collections;

public class VertexController : MonoBehaviour {

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        SendMessageUpwards("BroadcastVertexAt", this.transform.position);
        
        // Normalize rotation
        this.transform.LookAt(this.transform.position + Vector3.forward);
//        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.identity, Time.deltaTime);
    }

    void VertexAt(Vector3 v)
    {
        if (v != this.transform.position) {
            var overall = this.transform.position - v;
            var normal = overall.normalized;
            var mag = overall.magnitude;
            // Magic Number: 5 and 100
            var force = normal * Mathf.Sqrt(Mathf.Max(0, 5 - mag)) * Time.deltaTime * 100;

            //Debug.Log(string.Format("AddForce: overall: {0}, normal: {1}, mag: {2}, force: {3}, Mathf.Sqrt(mag): {4}, Time.deltaTime: {5};", overall, normal, mag, force, Mathf.Sqrt(mag), Time.deltaTime));
            this.rb.AddForce(force, ForceMode.Acceleration);
        }
    }
}
