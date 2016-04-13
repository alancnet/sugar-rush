using UnityEngine;
using System.Collections;
using UniRx;
using System.Collections.Generic;
using System.Linq;

public class GraphSceneController : MonoBehaviour {
    public GameObject vertexModel;
    public GameObject edgeModel;
    public class Node
    {
        public readonly string id;
        public readonly GameObject gameObject;
        public readonly Subject<EdgeController> edges;
        public readonly Subject<string> labels;

        public Node(string id = null, GameObject gameObject = null, Subject<EdgeController> edges = null, Subject<string> labels = null)
        {
            this.id = id;
            this.gameObject = gameObject;
            this.edges = edges;
            this.labels = labels;
        }

        public Node clone(string id = null, GameObject gameObject = null, Subject<EdgeController> edges = null, Subject<string> labels = null)
        {
            return new Node(
                id ?? this.id,
                gameObject ?? this.gameObject,
                edges ?? this.edges,
                labels ?? this.labels
            );
        }

        public override string ToString()
        {
            return string.Format("node {{ id: {0}; gameObject: {1}; edges: {2}; labels: {3} }}", id, gameObject, edges, labels);
        }
    }

    Dictionary<string, Node> nodes = new Dictionary<string, Node>();

    Node NodeLifecycle(Node node, GraphEvent ev)
    {
        return ev.Match<Node>()
            .Case<CreateVertex, Node>(cv => {
                var newObj = Object.Instantiate(vertexModel);
                newObj.transform.SetParent(vertexModel.transform.parent);

                // Name changes
                var labels = new Subject<string>();
                labels.Subscribe(label => {
                    node.gameObject.GetComponentsInChildren<TextMesh>().ToList().ForEach(text => text.text = label);
                });

                // When edges come
                var edges = new Subject<EdgeController>();
                edges.Take(1).Subscribe(edge =>
                {
                    var other = edge.vertexA == newObj ? edge.vertexB : edge.vertexA;
                    newObj.transform.position = other.transform.position + (other.transform.position + new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)).normalized;
                    newObj.SetActive(true);
                });

                return nodes[cv.id] = new Node(
                    id: cv.id,
                    gameObject: newObj,
                    labels: labels,
                    edges: edges
                );
            })
            .Case<CreateEdge, Node>(ce => {
                var newObj = Object.Instantiate(edgeModel);
                newObj.SetActive(true);
                newObj.transform.SetParent(edgeModel.transform.parent);
                var edge = newObj.GetComponent<EdgeController>();
                
                var nodeA = nodes[ce.sourceId];
                var nodeB = nodes[ce.targetId];
                
                edge.vertexA = nodeA.gameObject;
                edge.vertexB = nodeB.gameObject;

                nodeA.edges.OnNext(edge);
                nodeB.edges.OnNext(edge);

                return nodes[ce.id] = new Node(
                    id: ce.id,
                    gameObject: newObj
                );
            })
            .Case<UpdateValue, Node>(uv => {
                if (uv.name == "name")
                {
                    node.gameObject.GetComponentsInChildren<TextMesh>().ToList().ForEach(text => text.text = (string)uv.value);
                }
                return node;
            })
            .Default(node)
            .result;
    }

	// Use this for initialization
	void Start () {
        Game.log
            .GetTopic<GraphEvent>("graph-events")
            .Do(ev => Debug.Log(ev.ToString()))
            .GroupBy(x => x.id)
            .SelectMany(ev => ev.Scan(new Node(), NodeLifecycle))
            .Do(node => Debug.Log(node.ToString()))
            .Subscribe(node =>
            {
                if (node.id != null) nodes[node.id] = node;
            });


        Game.log.Publish("graph-scene-ready");
	}

    int ii = 0;
	// Update is called once per frame
	void Update () {
        if (ii++==11)
        {
            Debug.LogWarning("Doing hacky hard coded testing");
            var jupiter = Game.graph.CreateVertex("100", "name", "jupiter", "age", 5000, "type", "god");
            var saturn = Game.graph.CreateVertex("101", "name", "saturn", "age", 10000, "type", "titan");
            var sky = Game.graph.CreateVertex("102", "name", "sky", "type", "location");
            var neptune = Game.graph.CreateVertex("103", "name", "neptune", "age", 4500, "type", "god");
            var hercules = Game.graph.CreateVertex("104", "name", "hercules", "age", 30, "type", "demigod");
            var sea = Game.graph.CreateVertex("105", "name", "sea", "type", "location");
            var alcmene = Game.graph.CreateVertex("106", "name", "alcmene", "age", 45, "type", "human");
            var pluto = Game.graph.CreateVertex("107", "name", "pluto", "age", 4000, "type", "god");
            var nemean = Game.graph.CreateVertex("108", "name", "nemean", "type", "monster");
            var hydra = Game.graph.CreateVertex("109", "name", "hydra", "type", "monster");
            var cerberus = Game.graph.CreateVertex("110", "name", "cerberus", "type", "monster");
            var tartarus = Game.graph.CreateVertex("111", "name", "tartarus", "type", "location");

            jupiter.CreateEdge("200", saturn, "type", "father");
            jupiter.CreateEdge("201", sky, "type", "lives", "reason", "loves fresh breezes");
            jupiter.CreateEdge("202", neptune, "type", "brother");
            jupiter.CreateEdge("203", pluto, "type", "brother");

            neptune.CreateEdge("210", jupiter, "type", "brother");
            neptune.CreateEdge("211", pluto, "type", "brother");
            neptune.CreateEdge("212", sea, "type", "lives", "reason", "loves waves");

            hercules.CreateEdge("220", jupiter, "type", "father");
            hercules.CreateEdge("221", alcmene, "type", "mother");
            hercules.CreateEdge("222", nemean, "type", "battled", "time", 1, "place", new float[] { 38.1f, 23.7f });
            hercules.CreateEdge("223", hydra, "type", "battled", "time", 2, "place", new float[] { 37.7f, 23.9f });
            hercules.CreateEdge("224", cerberus, "type", "battled", "time", 12, "place", new float[] { 39f, 22f });

            pluto.CreateEdge("230", jupiter, "type", "brother");
            pluto.CreateEdge("231", neptune, "type", "brother");
            pluto.CreateEdge("232", cerberus, "type", "pet");
            pluto.CreateEdge("233", tartarus, "type", "lives", "reason", "no fear of death");

            cerberus.CreateEdge("240", tartarus, "type", "lives");
            

        }
	}
}
