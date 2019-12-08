using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public struct SafeSpan<T>
    {
        public readonly T[] Array;
        public readonly int Start;
        public readonly int Length;

        public SafeSpan(T[] array, int start, int length)
        {
            Array = array;
            Start = start;
            Length = length;
        }

        public T this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array[Start + i];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array[Start + i] = value;
        }
    }
}
