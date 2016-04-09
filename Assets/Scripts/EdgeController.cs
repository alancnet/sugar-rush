using UnityEngine;
using System.Collections;

public class EdgeController : MonoBehaviour {

    public GameObject vertexA;
    public GameObject vertexB;
    private Rigidbody rigidBodyA;
    private Rigidbody rigidBodyB;

    void Start()
    {
        rigidBodyA = vertexA.GetComponent<Rigidbody>();
        rigidBodyB = vertexB.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var total = vertexB.transform.position - vertexA.transform.position;
        var mid = vertexA.transform.position + (total / 2);
        var mag = total.magnitude;
        // Magic Number: 3
        var springForce = 3 - mag;
        var force = total.normalized * springForce * 10 * Time.deltaTime;
        rigidBodyA.AddForce(force * -1);
        rigidBodyB.AddForce(force);

        transform.position = mid;
        transform.rotation = Quaternion.LookRotation(vertexA.transform.position - mid) * Quaternion.Euler(90, 0, 0);

        transform.localScale = new Vector3(0.1f, mag / 2, 0.1f);
    }

}
