using System;
using UniRx;
using UnityEngine;
static class Extensions
{
    /// <summary>
    /// Passes all items and errors from source to sink. Does not pass completes.
    /// </summary>
    /// <typeparam name="T">Data type of the pipe</typeparam>
    /// <param name="source">Source of data</param>
    /// <param name="sink">Where to send it</param>
    /// <returns>Subscription</returns>
    public static System.IDisposable Pipe<T>(this IObservable<T> source, IObserver<T> sink) {
        if (source == null) throw new Exception("source is null");
        if (sink == null) throw new Exception("sink is null");
        return source.Do(x => sink.OnNext(x)).DoOnError(err => sink.OnError(err)).Subscribe();
    }

    public class MatchMonad<R>
    {
        public readonly object source;
        public readonly Type type;
        public readonly R result;
        public readonly bool matched;
        public MatchMonad(object source, Type type, bool matched, R result)
        {
            this.source = source;
            this.type = type;
            this.matched = matched;
            this.result = result;
        }
        public MatchMonad<R> Case<T>(Action<T> action)
        {
            if (!matched && typeof(T).IsAssignableFrom(type))
            {
                action((T)source);
                return new MatchMonad<R>(source, type, true, default(R));
            }
            else
            {
                return this;
            }
        }
        public MatchMonad<R> Case<T>(Action action)
        {
            if (!matched && typeof(T).IsAssignableFrom(type))
            {
                action();
                return new MatchMonad<R>(source, type, true, default(R));
            }
            else
            {
                return this;
            }
        }
        public MatchMonad<R> Case<T, U>(Func<T, U> func) where U : R
        {
            if (!matched && typeof(T).IsAssignableFrom(type))
                return new MatchMonad<R>(source, type, true, func((T)source));
            else
                return this;
        }
        public MatchMonad<R> Case<T, U>(U val) where U : R
        {
            if (!matched && typeof(T).IsAssignableFrom(type))
                return new MatchMonad<R>(source, type, true, val);
            else
                return this;
        }


        public MatchMonad<R> Default(Action<object> action)
        {
            if (matched) return this;
            action(source);
            return new MatchMonad<R>(source, type, true, default(R));
        }
        public MatchMonad<R> Default(Action action)
        {
            if (matched) return this;
            action();
            return new MatchMonad<R>(source, type, true, default(R));
        }
        public MatchMonad<R> Default<U>(Func<object, U> func) where U : R
        {
            if (matched) return this;
            return new MatchMonad<R>(source, type, true, func(source));
        }
        public MatchMonad<R> Default<U>(U val) where U : R
        {
            if (matched) return this;
            return new MatchMonad<R>(source, type, true, val);
        }

    }
    public static MatchMonad<R> Match<R>(this object source)
    {
        return new MatchMonad<R>(source, source.GetType(), false, default(R));
    }
    public static MatchMonad<object> Match(this object source)
    {
        return new MatchMonad<object>(source, source.GetType(), false, null);
    }

    public static IObservable<T> Debug<T>(this IObservable<T> stream)
    {
        return stream.Do(x => UnityEngine.Debug.Log(string.Format("debug: {0}", ((object)x) ?? (object)"null")));
    }
}