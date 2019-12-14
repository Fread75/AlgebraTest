using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

namespace Common
{
    [DisassemblyDiagnoser(recursiveDepth: 3)]
    //[DryJob]
    public unsafe class Benchmark2Loops
    {
        private const int SIZE = 2000 * 2000;
        private readonly double maxArrayValue = Math.Sqrt(double.MaxValue / SIZE);
        private readonly double[] array1 = new double[SIZE];
        private readonly double[] array2 = new double[SIZE];
        
        public Benchmark2Loops()
        {
            var random = new Random(0);
            for (int i = 0; i < array1.Length; i++)
            {
                array1[i] = random.NextDouble() * maxArrayValue;
                array2[i] = random.NextDouble() * maxArrayValue;
            }
        }
 
        [Benchmark(Baseline = true)]
        public void SimpleFor() => SimpleFor(array1, array2);
        [Benchmark]
        public void SimpleForWithCheck() => SimpleForWithCheck(array1, array2);
        [Benchmark]
        public void UnsafeFor() => UnsafeFor(array1, array2);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double SimpleFor(double[] array1, double[] array2)
        {
            var sum = 0D;
            for (int i = 0; i < array1.Length; i++)
            {
                sum += array1[i] + array2[i];
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double SimpleForWithCheck(double[] array1, double[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return 0;
            }

            var sum = 0D;
            for (int i = 0; i < array1.Length; i++)
            {
                sum += array1[i] + array2[i];
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double UnsafeFor(double[] array1, double[] array2)
        {
            var sum = 0D;
            fixed (double* ptr2 = array2)
            {
                for (int i = 0; i < array1.Length; i++)
                {
                    sum += array1[i] + ptr2[i];
                }
            }
            return sum;
        }

    }
}
