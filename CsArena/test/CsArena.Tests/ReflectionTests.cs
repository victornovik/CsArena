using System.Reflection;
using CsArena.Tests.models;

namespace CsArena.Tests;

public class ReflectionTests
{
    [Fact]
    public void IsClassImmutable()
    {
        bool ContainsMutableFields(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (!field.IsInitOnly) // readonly field
                    return true;
            }

            var baseType = type.BaseType;
            return baseType != null && ContainsMutableFields(baseType);
        }

        Assert.True(ContainsMutableFields(typeof(Player)));
        Assert.False(ContainsMutableFields(typeof(object)));
        Assert.False(ContainsMutableFields(typeof(Empty)));

        Readonly r = new Readonly();
        Assert.Null(r.Text2);
        Assert.False(ContainsMutableFields(r.GetType()));
    }

    private class Empty;

    private class Readonly
    {
        public readonly string? Text2 = null;
    }

    [Fact]
    public void GetProperties()
    {
        Dictionary<string, string?> GetPropertyValues(object instance)
        {
            Type type = instance.GetType();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            
            var ret = new Dictionary<string, string?>();
            foreach (var prop in props)
            {
                ret.Add(prop.Name, prop.GetValue(instance)?.ToString());
            }
            return ret;
        }

        var p = new Player {Name = "Victor", DaysSinceLastLogin = 5};
        var values = GetPropertyValues(p);
        Assert.Equal(p.Name, values["Name"]);
        Assert.Equal("5", values["DaysSinceLastLogin"]);
        Assert.Null(values["DateOfBirth"]);
    }

    [Fact]
    public void CreateByClosedGenericType()
    {
        var playerList = Create(typeof(List<Player>));
        Assert.Equal("List`1", playerList.GetType().Name);
        Assert.StartsWith("System.Collections.Generic.List`1", playerList.GetType().FullName);

        var genericArgs = playerList.GetType().GenericTypeArguments;
        Assert.Equal("Player", genericArgs[0].Name);

        var playerDict = Create(typeof(Dictionary<string, Player>));
        Assert.Equal("Dictionary`2", playerDict.GetType().Name);

        genericArgs = playerDict.GetType().GenericTypeArguments;
        Assert.Equal("String", genericArgs[0].Name);
        Assert.Equal("Player", genericArgs[1].Name);
    }

    [Fact]
    public void CreateByUnboundGenericType()
    {
        var playerList = Create(typeof(List<>), typeof(Player));
        Assert.Equal("List`1", playerList.GetType().Name);
        Assert.StartsWith("System.Collections.Generic.List`1", playerList.GetType().FullName);

        var genericArgs = playerList.GetType().GenericTypeArguments;
        Assert.Equal("Player", genericArgs[0].Name);

        var playerDict = Create(typeof(Dictionary<,>), typeof(string), typeof(Player));
        Assert.Equal("Dictionary`2", playerDict.GetType().Name);

        genericArgs = playerDict.GetType().GenericTypeArguments;
        Assert.Equal("String", genericArgs[0].Name);
        Assert.Equal("Player", genericArgs[1].Name);
    }

    private object Create(Type type)
    {
        return Activator.CreateInstance(type) ?? throw new InvalidOperationException();
    }

    private object Create(Type collectionType, params Type[] itemTypes)
    {
        var closedType = collectionType.MakeGenericType(itemTypes);
        return Activator.CreateInstance(closedType) ?? throw new InvalidOperationException();
    }

    [Fact]
    public void CallGenericMethod()
    {
        var type = typeof(Player);
        var methodInfo = type.GetMethod("GetTypeName");
        Assert.NotNull(methodInfo);

        methodInfo = methodInfo.MakeGenericMethod(typeof(DateTime));
        var ret = methodInfo.Invoke(new Player(), null);

        Assert.Equal("DateTime", ret);
    }

    [Fact]
    public void DIContainer_ResolveTypes()
    {
        // Mimic NInject container style
        var ioc = new Container();
        ioc.For<ILogger>().Use<SqlServerLogger>();

        var logger = ioc.Resolve<ILogger>();

        Assert.NotNull(logger);
        Assert.Equal(typeof(SqlServerLogger), logger.GetType());
    }

    [Fact]
    public void DIContainer_ResolveTypesWithoutDefaultCtor()
    {
        var ioc = new Container();
        ioc.For<ILogger>().Use<SqlServerLogger>();
        ioc.For<IRepository<Player>>().Use<SqlRepository<Player>>();

        var repository = ioc.Resolve<IRepository<Player>>();

        Assert.NotNull(repository);
        Assert.Equal(typeof(SqlRepository<Player>), repository.GetType());
    }

    [Fact]
    public void DIContainer_ResolveConcreteType()
    {
        var ioc = new Container();
        ioc.For<ILogger>().Use<SqlServerLogger>();
        ioc.For<IRepository<Player>>().Use<SqlRepository<Player>>();
        ioc.For<IRepository<Customer>>().Use<SqlRepository<Customer>>();

        var service = ioc.Resolve<InvoiceService>();

        Assert.NotNull(service);
        Assert.Equal(typeof(InvoiceService), service.GetType());
    }

    [Fact]
    public void DIContainer_ResolveGenericUnboundType()
    {
        var ioc = new Container();
        ioc.For<ILogger>().Use<SqlServerLogger>();
        ioc.For(typeof(IRepository<>)).Use(typeof(SqlRepository<>));

        var service = ioc.Resolve<InvoiceService>();

        Assert.NotNull(service);
        Assert.Equal(typeof(InvoiceService), service.GetType());
    }

#pragma warning disable CS9113 //Parameter is unread
    private interface ILogger;
    private class SqlServerLogger : ILogger;
    private interface IRepository<T>;

    private class SqlRepository<T>(ILogger logger) : IRepository<T>;

    private class Customer;
    private class InvoiceService(IRepository<Customer> rep, ILogger logger);
#pragma warning restore CS9113

    private class Container
    {
        public ContainerBuilder For<TSource>()
        {
            return For(typeof(TSource));
        }

        public ContainerBuilder For(Type srcType)
        {
            return new ContainerBuilder(this, srcType);
        }

        public TSource? Resolve<TSource>()
        {
            return (TSource?)Resolve(typeof(TSource));
        }

        private object? Resolve(Type srcType)
        {
            if (_typeMapping.TryGetValue(srcType, out var dstType))
            {
                return CreateInstance(dstType);
            }

            // Look in type mapping by unbound generic source type.
            // E.g. if we look for IRepository<Customer> it will look up by IRepository<>
            if (srcType.IsGenericType &&
                
                _typeMapping.TryGetValue(srcType.GetGenericTypeDefinition(), out var dstType2))
            {
                var closedType = dstType2.MakeGenericType(srcType.GenericTypeArguments);
                return CreateInstance(closedType);
            }

            // If we're requested to look for a concrete type, and it is not registered
            // then we can create its instance directly.
            if (!srcType.IsAbstract)
            {
                return CreateInstance(srcType);
            }

            throw new InvalidOperationException("Could not resolve " + srcType.FullName);
        }

        private object? CreateInstance(Type dstType)
        {
            var parameters = dstType.GetConstructors()
                .Where(ctor => ctor.IsPublic)
                .OrderByDescending(ctor => ctor.GetParameters().Length)
                .First()
                .GetParameters()
                .Select(param => Resolve(param.ParameterType))
                .ToArray();

            return Activator.CreateInstance(dstType, parameters);
        }

        private readonly Dictionary<Type, Type> _typeMapping = new();

        public class ContainerBuilder(Container container, Type srcType)
        {
            public ContainerBuilder Use<TDestination>()
            {
                return Use(typeof(TDestination));
            }

            public ContainerBuilder Use(Type dstType)
            {
                container._typeMapping.Add(srcType, dstType);
                return this;
            }
        }
    }
}

