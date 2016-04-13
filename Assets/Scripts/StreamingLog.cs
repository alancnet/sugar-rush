using System;
using System.Collections.Generic;
using UniRx;
class StreamingLog
{

    Dictionary<string, Subject<object>> topics = new Dictionary<string, Subject<object>>();

    public IObservable<object> GetTopic(string name)
    {
        return EnsureTopic(name).Where(x => true);
    }

    public IObservable<T> GetTopic<T>(string name)
    {
        return GetTopic(name).OfType<object, T>();
    }

    public void Publish(string name)
    {
        EnsureTopic(name).OnNext(new object());
    }

    public void Publish<T>(string name, T value)
    {
        EnsureTopic(name).OnNext(value);
    }

    public System.IDisposable Publish<T>(string name, IObservable<T> stream)
    {
        if (stream == null) throw new Exception("stream is null");
        var topic = EnsureTopic(name);
        return stream.Cast<T, object>().Pipe(topic);
    }

    Subject<object> EnsureTopic(string name)
    {
        Subject<object> ret;
        if (!topics.TryGetValue(name, out ret))
        {
            lock (topics)
            {
                if (!topics.TryGetValue(name, out ret))
                {
                    var sub = new Subject<object>();
                    ret = sub;
                    topics[name] = ret;
                    sub.DoOnCompleted(() => topics.Remove(name)).Subscribe();
                }
            }
        }
        return ret;
    }
}
