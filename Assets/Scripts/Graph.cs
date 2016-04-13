using System.Collections.Generic;
using System.Linq;
using UniRx;

public abstract class GraphObject {
    protected IObserver<GraphEvent> log;
    public IObservable<GraphEvent> events;

    public GraphObject()
    {
        var sub = new Subject<GraphEvent>();
        events = sub;
        log = sub;
    }
}
public class Graph : GraphObject
{
    public Vertex CreateVertex(string id, params object[] keyVals)
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        for (var i = 0; i < keyVals.Length; i += 2)
        {
            values[keyVals[i].ToString()] = keyVals[i + 1];
        }
        return CreateVertex(id, values);
    }
    public Vertex CreateVertex(string id, IEnumerable<KeyValuePair<string, object>> values = null)
    {
        Vertex ret = new Vertex(id);
        ret.events.Pipe(log);
        log.OnNext(new CreateVertex(id));
        if (values != null) values.ToObservable().Subscribe(kv => ret.UpdateValue(kv.Key, kv.Value));
        return ret;
    }
}

public abstract class StoreObject : GraphObject
{
    public readonly string id;
    protected IDictionary<string, object> store = new Dictionary<string, object>();
    public void UpdateValue(string name, object value)
    {
        store[name] = value;
        log.OnNext(new UpdateValue(id, name, value));
    }
    public void DeleteValue(string name)
    {
        if (store.ContainsKey(name))
        {
            store.Remove(name);
            log.OnNext(new DeleteValue(id, name));
        }
    }

    public void Delete()
    {
        log.OnNext(new Delete(id));
        log.OnCompleted();
    }

    protected StoreObject(string id)
    {
        this.id = id;
    }

}

public class Vertex : StoreObject {
    public HashSet<Edge> edges = new HashSet<Edge>();
    public Edge CreateEdge(string id, Vertex other, params object[] keyVals)
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        for (var i = 0; i < keyVals.Length; i += 2)
        {
            values[keyVals[i].ToString()] = keyVals[i + 1];
        }
        return CreateEdge(id, other, values);
    }

    public Edge CreateEdge(string id, Vertex other, IEnumerable<KeyValuePair<string, object>> values = null)
    {
        Edge ret = new Edge(id);
        ret.events.Pipe(log);
        edges.Add(ret);
        log.OnNext(new CreateEdge(id, this.id, other.id));
        if (values != null) values.ToObservable().Subscribe(kv => ret.UpdateValue(kv.Key, kv.Value));
        return ret;
    }
    public Vertex(string id) : base(id) { }
}

public class Edge : StoreObject {
    public Edge(string id) : base(id) { }
}

// Event Classes
public class GraphEvent
{
    public readonly string id;
    public GraphEvent(string id)
    {
        this.id = id;
    }
}
public class CreateVertex : GraphEvent
{
    public CreateVertex(string id) : base(id) { }
    public override string ToString()
    {
        return string.Format("create-vertex {{ id: {0}; }}", id);
    }
}
public class CreateEdge : GraphEvent
{
    public readonly string sourceId;
    public readonly string targetId;
    public CreateEdge(string id, string sourceId, string targetId):base(id)
    {
        this.sourceId = sourceId;
        this.targetId = targetId;
    }
    public override string ToString()
    {
        return string.Format("create-edge {{ id: {0}; sourceId: {1}; targetId: {2}; }}", id, sourceId, targetId);
    }
}
public class Delete : GraphEvent
{
    public Delete(string id) : base(id) {}
    public override string ToString()
    {
        return string.Format("delete {{ id: {0}; }}");
    }
}
public class UpdateValue : GraphEvent
{
    public readonly string name;
    public readonly object value;
    public UpdateValue(string id, string name, object value) : base(id)
    {
        this.name = name;
        this.value = value;
    }
    public override string ToString()
    {
        return string.Format("update-value {{ id: {0}; name: {1}; value: {2}; }}", id, name, value);
    }
}
public class DeleteValue : GraphEvent
{
    public readonly string name;
    public DeleteValue(string id, string name):base(id)
    {
        this.name = name;
    }
    public override string ToString()
    {
        return string.Format("delete-value {{ id: {0}; name: {1}; }}", id, name);
    }
}
