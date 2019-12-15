using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TestAssembly;
using TestCompiledAssembly;

namespace Common
{
    public sealed class TestObject
    {
        private readonly int aValue;
        private readonly int bValue;
        private readonly int cValue;

        public TestObject(int a, int b, int c)
        {
            aValue = a;
            bValue = b;
            cValue = c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAValue() => aValue;
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int GetBValue() => bValue;

        public int GetCValue() => cValue;

    }


    [DryJob]
    [DisassemblyDiagnoser(printIL: true)]
    public class TestInlining
    {
        private TestObject to = new TestObject(5, 10, 89);
        private ExternalTestObject eto = new ExternalTestObject(8, 45, 7);
        private CompiledExternalTestObject ceto = new CompiledExternalTestObject(8, 4, 3);

        [Benchmark]
        public void CallA() => to.GetAValue();
        [Benchmark]
        public void CallB() => to.GetBValue();
        [Benchmark]
        public void CallC() => to.GetCValue();


        [Benchmark]
        public void AssemblyCallA() => eto.GetAValue();
        [Benchmark]
        public void AssemblyCallB() => eto.GetBValue();
        [Benchmark]
        public void AssemblyCallC() => eto.GetCValue();


        [Benchmark]
        public void CompiledAssemblyCallA() => ceto.GetAValue();
        [Benchmark]
        public void CompiledAssemblyCallB() => ceto.GetBValue();
        [Benchmark]
        public void CompiledAssemblyCallC() => ceto.GetCValue();
    }
}
