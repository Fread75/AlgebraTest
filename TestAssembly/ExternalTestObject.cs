using System;
using System.Runtime.CompilerServices;

namespace TestAssembly
{
    public sealed class ExternalTestObject
    {
        private readonly int aValue;
        private readonly int bValue;
        private readonly int cValue;

        public ExternalTestObject(int a, int b, int c)
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
}
