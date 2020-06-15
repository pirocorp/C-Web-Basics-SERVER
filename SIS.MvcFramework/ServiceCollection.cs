namespace SIS.MvcFramework
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Dependency container
    /// </summary>
    public class ServiceCollection : IServiceCollection
    {
        private readonly IDictionary<Type, Type> _dependencyContainer;

        public ServiceCollection()
        {
            this._dependencyContainer = new ConcurrentDictionary<Type, Type>();
        }

        public void Add<TSource, TDestination>() where TDestination : TSource
        {
            this._dependencyContainer[typeof(TSource)] = typeof(TDestination);
        }

        public object CreateInstance(Type type)
        {
            if (this._dependencyContainer.ContainsKey(type))
            {
                type = this._dependencyContainer[type];
            }

            var constructor = type
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(c => c.GetParameters().Count())
                .FirstOrDefault();

            var parameterValues = new List<object>();

            foreach (var parameterInfo in constructor.GetParameters())
            {
                var instance = this.CreateInstance(parameterInfo.ParameterType);

                parameterValues.Add(instance);
            }

            return constructor.Invoke(parameterValues.ToArray());
        }

        public T CreateInstance<T>()
        {
            return (T) this.CreateInstance(typeof(T));
        }
    }
}
