using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading.Tasks;

namespace Basics.Test
{
    public class DependencyInjectionTests
    {
        [Fact]
        //TODO: Occasionally fails when running tests!
        public void Container_SingleInstance_Multithreaded()
        {
            var container = new Container();
            var tasks = new List<Task>();

            for (var i = 0; i < 100; i++)
            {
                var j = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var obj = (Foo)container.Register<Foo>().As<IFoo>().SingleInstance($"single", j);
                    Assert.Equal(0, obj.value);
                    System.Diagnostics.Debug.WriteLine(obj.value.ToString());
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }


        [Fact]
        public void Container_SingleInstance()
        {
            var container = new Container();

            var singleA = (Foo)container.Register<Foo>().As<IFoo>().SingleInstance("singleA", 10);
            CheckIFooSingleInstance(container, singleA);

            var singleB = (Bar)container.Register<Bar>().As<IFoo>().SingleInstance("singleB", 20);
            CheckIFooSingleInstance(container, singleB);

            var singleC = (Caz)container.Register<Caz>().As<IFoo>().SingleInstance("singleC", 30);
            CheckIFooSingleInstance(container, singleC);
        }

        [Fact]
        public void Container_ITest()
        {
            var container = new Container();

            container.Register<Foo>().As<IFoo>();
            CheckIFoo<Foo>(container);

            container.Register<Bar>().As<IFoo>();
            CheckIFoo<Bar>(container);

            container.Register<Caz>().As<IFoo>();
            CheckIFoo<Caz>(container);
        }

        private void CheckIFooSingleInstance(Container container, IFoo singleInstance)
        {
            Assert.Equal(singleInstance, container.Create<IFoo>("a", 5));
            Assert.Equal(singleInstance, container.Create<IFoo>("b"));
            Assert.Equal(singleInstance, container.Create<IFoo>());
        }

        private void CheckIFoo<T>(Container container) where T : IFoo
        {
            var name = typeof(T).Name;
            var a = container.Create<IFoo>("a", 5);
            var b = container.Create<IFoo>("b");
            var c = container.Create<IFoo>();
            Assert.Equal(IFooToString(name, "a", 5), a.ToString());
            Assert.Equal(IFooToString(name, "b", -1), b.ToString());
            Assert.Equal(IFooToString(name, "default", -1), c.ToString());
        }

        public static string IFooToString(string prefix, string name, int value) => $"[{prefix} | {name}: {value}]";

        interface IFoo
        {
            string name { get; set; }
            int value { get; set; }
        }

        class Foo : IFoo
        {
            public string name { get; set; }
            public int value { get; set; }

            public Foo() : this("default") { }
            public Foo(string _name) : this(_name, -1) { }
            public Foo(string _name, int _value) { name = _name; value = _value; }

            public override string ToString() => IFooToString(GetType().Name, name, value);
        }

        class Bar : Foo
        {
            public Bar() : this("default") { }
            public Bar(string _name) : this(_name, -1) { }
            public Bar(string _name, int _value) : base(_name, _value) { }
        }

        class Caz : IFoo
        {
            public string name { get; set; }
            public int value { get; set; }
            
            public Caz() : this("default") { }
            public Caz(string _name) : this(_name, -1) { }
            public Caz(string _name, int _value) { name = _name; value = _value; }

            public override string ToString() => IFooToString(GetType().Name, name, value);
        }

    }
}
