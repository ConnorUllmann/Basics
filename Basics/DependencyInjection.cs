using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace Basics
{
    abstract class GlobalContainer : Container
    {
        private static Lazy<Container> container = new Lazy<Container>();
        public static ContainerTransaction Register<T>() => container.Value.Register<T>();
        public static object Create<I>(params object[] _constructorArguments) => container.Value.Create<I>(_constructorArguments);
    }

    public class Container
    {
        public class ContainerTransaction
        {
            private Container owner;
            private Type type;
            private Type interfaceType;
            internal ContainerTransaction(Container _owner, Type _type) { owner = _owner; type = _type; }

            public ContainerTransaction As<I>()
            {
                interfaceType = typeof(I);
                if (interfaceType == type)
                    throw new ArgumentException("Interface type and target type cannot be the same.");
                owner.AddInterfaceMapping<I>(type);
                return this;
            }

            public object SingleInstance(params object[] _constructorArguments) => owner.AddSingleInstance(interfaceType, type, _constructorArguments);
        }

        private ConcurrentDictionary<Type, Type> interfaceToType = new ConcurrentDictionary<Type, Type>();
        private ConcurrentDictionary<Tuple<Type, Type>, object> interfaceTypePairToObject = new ConcurrentDictionary<Tuple<Type, Type>, object>();

        private void AddInterfaceMapping<I>(Type _type) => interfaceToType[typeof(I)] = _type;

        private object AddSingleInstance(Type _interfaceType, Type _type, params object[] _constructorArguments)
        {
            var key = InterfaceTypePair(_interfaceType, _type);
            var obj = Create(_interfaceType, _constructorArguments);
            return interfaceTypePairToObject.GetOrAdd(key, obj);
        }

        private static Tuple<Type, Type> InterfaceTypePair(Type _interfaceType, Type _type) => new Tuple<Type, Type>(_interfaceType, _type);

        public ContainerTransaction Register<T>() => new ContainerTransaction(this, typeof(T));

        public object Create<I>(params object[] _constructorArguments) => Create(typeof(I), _constructorArguments);
        private object Create(Type _interfaceType, params object[] _constructorArguments)
        {
            if (!interfaceToType.TryGetValue(_interfaceType, out Type type))
                throw new ArgumentException($"Container doesn't have these types registered together: {_interfaceType.Name} & {type.Name}");

            var key = InterfaceTypePair(_interfaceType, type);
            if (interfaceTypePairToObject.TryGetValue(key, out object obj))
                return obj;

            return Activator.CreateInstance(type, _constructorArguments);
        }
    }
}
