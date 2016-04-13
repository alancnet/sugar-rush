using UnityEngine;
using System.Collections;

public class EdgeController : MonoBehaviour {

    public GameObject vertexA;
    public GameObject vertexB;
    private Rigidbody rigidBodyA;
    private Rigidbody rigidBodyB;
    private LineRenderer lineRenderer;

    void Start()
    {
        rigidBodyA = vertexA.GetComponent<Rigidbody>();
        rigidBodyB = vertexB.GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        var a = vertexA.transform.position + Vector3.down * vertexA.transform.lossyScale.y / 2;
        var b = vertexB.transform.position + Vector3.up * vertexB.transform.lossyScale.y / 2; 
        var total = b - a;
        //var mid = a + (total / 2);
        var mag = total.magnitude;
        // Magic Number: 3
        var springForce = 3 - mag;
        var force = total.normalized * springForce * 10 * Time.deltaTime;
        rigidBodyA.AddForce(force * -1);
        rigidBodyB.AddForce(force);

        //lineRenderer.SetVertexCount(3);
        //lineRenderer.SetPositions(new Vector3[] {
        //    a,
        //    mid,
        //    b
        //});

        var bezier = Bezier(
            a,
            Between(a + Vector3.down * 3, b, 0.1f),
            Between(b + Vector3.up * 3, a, 0.1f),
            b,
            32
        );

        lineRenderer.SetVertexCount(bezier.Length);
        lineRenderer.SetPositions(bezier);

    }

    Vector3 Between(Vector3 a, Vector3 b, float t)
    {
        return a + ((b - a) * t);
    }

    Vector3[] Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int resolution)
    {
        Vector3[] ret = new Vector3[resolution + 1];
        for (int i = 0; i <= resolution; i++)
        {
            float t = ((float)i / (float)resolution) * (float)i;
            ret[i] = B(t/(float)resolution, p0, p1, p2, p3);
        }
        return ret;
    }
    Vector3 B(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // \mathbf{B}(t)= ((1-t)^3\mathbf{P}_0) + (3(1-t)^2t\mathbf{P}_1) + (3(1-t)t^2\mathbf{P}_2) + (t^3\mathbf{P}_3)   // \mbox{ , } 0 \le t \le 1.
        // https://en.wikipedia.org/wiki/B%C3%A9zier_curve
        return (Mathf.Pow(1 - t, 3) * p0) + (3 * Mathf.Pow(1 - t, 2) * t * p1) + (3 * (1 - t) * Mathf.Pow(t, 2) * p2) + (Mathf.Pow(t, 3) * p3);
    }
}
