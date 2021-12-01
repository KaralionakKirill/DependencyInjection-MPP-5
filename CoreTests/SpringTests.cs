using System;
using Core.SummerCore;
using NUnit.Framework;
using LifeCycle = Core.SummerCore.LifeCycle;

namespace CoreTests
{
    [TestFixture]
    public class SpringTests
    {
        private interface IA
        {
            int Rnd { get; }
        }

        private interface IB
        {
            int Rnd { get; }
            IA A { get; }
        }

        interface IC<T>
        {
            int Rnd { get; }
            T t { get; }
        }

        class C1 : IC<IA>
        {
            private int _rnd = new Random().Next();
            public int Rnd => _rnd;
            private IA _a;
            public IA t => _a;
        }
        
        class C2 : IC<IA>
        {
            private int _rnd = new Random().Next();
            public int Rnd => _rnd;
            private IA _a;
            public IA t => _a;
        }
        
        class A : IA
        {
            private int _rnd = new Random().Next();
            public int Rnd => _rnd;
        }

        class B : IB
        {
            private IA _a;
            public IA A => _a;
            private int _rnd = new Random().Next();
            public int Rnd => _rnd;

            public B(IA a)
            {
                _a = a;
            }
        }

        [Test]
        public void Resolve_DifferentValues_Instance()
        {
            var c = new SpringConfig();
            c.Register<IA, A>(LifeCycle.Instance);
            var s = new Spring(c);
            int prev = s.Resolve<IA>().Rnd;
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curr = s.Resolve<IA>().Rnd;
                    Assert.AreNotEqual(prev, curr);
                }
            });
        }
        
        [Test]
        public void Resolve_SameValues_Singleton()
        {
            var c = new SpringConfig();
            c.Register<IA, A>(LifeCycle.Singleton);
            var s = new Spring(c);
            int prev = s.Resolve<IA>().Rnd;
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curr = s.Resolve<IA>().Rnd;
                    Assert.AreEqual(prev, curr);
                }
            });
        }

        [Test]
        public void Resolve_Instance_Instance()
        {
            var c = new SpringConfig();
            c.Register<IA, A>(LifeCycle.Instance);
            c.Register<IB, B>(LifeCycle.Instance);
            var s = new Spring(c);
            IB prev = s.Resolve<IB>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    IB curr = s.Resolve<IB>();
                    Assert.AreNotEqual(prev.Rnd, curr.Rnd);
                    Assert.AreNotEqual(prev.A.Rnd, curr.A.Rnd);
                }
            });
        }
        
        [Test]
        public void Resolve_Instance_Singleton()
        {
            var c = new SpringConfig();
            c.Register<IA, A>(LifeCycle.Instance);
            c.Register<IB, B>(LifeCycle.Singleton);
            var s = new Spring(c);
            int preva = s.Resolve<IA>().Rnd;
            IB prev = s.Resolve<IB>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curra = s.Resolve<IA>().Rnd;
                    IB curr = s.Resolve<IB>();
                    Assert.AreNotEqual(preva, curra);
                    Assert.AreEqual(prev.Rnd, curr.Rnd);
                    Assert.AreEqual(prev.A.Rnd, curr.A.Rnd);
                }
            });
        }
        
        [Test]
        public void Resolve_Singleton_Instance()
        {
            var c = new SpringConfig();
            c.Register<IA, A>(LifeCycle.Singleton);
            c.Register<IB, B>(LifeCycle.Instance);
            var s = new Spring(c);
            int preva = s.Resolve<IA>().Rnd;
            IB prev = s.Resolve<IB>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curra = s.Resolve<IA>().Rnd;
                    IB curr = s.Resolve<IB>();
                    Assert.AreEqual(preva, curra);
                    Assert.AreNotEqual(prev.Rnd, curr.Rnd);
                    Assert.AreEqual(prev.A.Rnd, curr.A.Rnd);
                }
            });
        }
        
        [Test]
        public void Resolve_Singleton_Singleton()
        {
            var c = new SpringConfig();
            c.Register<IA, A>(LifeCycle.Singleton);
            c.Register<IB, B>(LifeCycle.Singleton);
            var s = new Spring(c);
            int preva = s.Resolve<IA>().Rnd;
            IB prev = s.Resolve<IB>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curra = s.Resolve<IA>().Rnd;
                    IB curr = s.Resolve<IB>();
                    Assert.AreEqual(preva, curra);
                    Assert.AreEqual(prev.Rnd, curr.Rnd);
                    Assert.AreEqual(prev.A.Rnd, curr.A.Rnd);
                }
            });
        }

        [Test]
        public void ResolveAll_Count_Correct()
        {
            var c = new SpringConfig();
            c.Register<IC<IA>, C1>(LifeCycle.Instance);
            c.Register<IC<IA>, C2>(LifeCycle.Instance);
            var s = new Spring(c);
            Assert.AreEqual(2, s.ResolveAll<IC<IA>>().Length);
        }
    }
}