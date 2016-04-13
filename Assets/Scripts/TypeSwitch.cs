//using System;
//static class TypeSwitch
//{
//    // http://stackoverflow.com/questions/298976/is-there-a-better-alternative-than-this-to-switch-on-type

//    public static void Do(object source, params CaseInfo[] cases)
//    {
//        var type = source.GetType();
//        foreach (var entry in cases)
//        {
//            if (entry.isDefault || entry.target.IsAssignableFrom(type))
//            {
//                entry.action(source);
//                break;
//            }
//        }
//    }

//    public static CaseInfo Case<T>(Action action)
//    {
//        return new CaseInfo(
//            action: x => action(),
//            target: typeof(T)
//        );
//    }

//    public static CaseInfo Case<T>(Action<T> action)
//    {
//        return new CaseInfo(
//            action: (x) => action((T)x),
//            target: typeof(T)
//        );
//    }

//    public static CaseInfo Default(Action action)
//    {
//        return new CaseInfo(
//            action: x => action(),
//            isDefault: true
//        );
//    }
//}
//public class CaseInfo
//{
//    public readonly bool isDefault;
//    public readonly Type target;
//    public readonly Action<object> action;
//    public CaseInfo(bool isDefault = false, Type target = null, Action<object> action = null)
//    {
//        this.isDefault = isDefault;
//        this.target = target;
//        this.action = action;
//    }
//}
