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
    public unsafe class BenchmarkLoop
    {
        private const int SIZE = 2000 * 2000;
        private readonly double maxArrayValue = Math.Sqrt(double.MaxValue / SIZE);
        private readonly double[] array = new double[SIZE];
        private readonly List<double> list;

        public BenchmarkLoop()
        {
            var random = new Random(0);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.NextDouble() * maxArrayValue;
            }
            list = new List<double>(array);
        }
 
        [Benchmark(Baseline = true)]
        public void SimpleFor() => SimpleFor(array);
        [Benchmark]
        public void UnsafeSimpleFor() => UnsafeSimpleFor(array);
        [Benchmark]
        public void LengthAsParamFor() => LengthAsParamFor(array, array.Length);
        [Benchmark]
        public void Foreach() => Foreach(array);
        [Benchmark]
        public void SimdFor() => SimdFor(array);
        [Benchmark]
        public void LinqFor() => LinqFor(array);
        [Benchmark]
        public void ListSimpleFor() => ListSimpleFor(list);
        [Benchmark]
        public void ListForeach() => ListForeach(list);
        [Benchmark]
        public void ListLinqFor() => ListLinqFor(list);
        [Benchmark]
        public void ListForSpecialized() => ListForSpecialized(list);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double SimpleFor(double[] array1)
        {
            var sum = 0D;
            for (int i = 0; i < array1.Length; i++)
            {
                sum += array1[i];
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double UnsafeSimpleFor(double[] array1)
        {
            int size = array1.Length;
            fixed (double* ptr = array1)
            {
                var sum = 0D;
                for (int i = 0; i < size; i++)
                {
                    sum += ptr[i];
                }
                return sum;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double LengthAsParamFor(double[] array1, int length)
        {
            var sum = 0D;
            for (int i = 0; i < length; i++)
            {
                sum += array1[i];
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double Foreach(double[] array1)
        {
            var sum = 0D;
            foreach (double d in array1)
            {
                sum += d;
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double SimdFor(double[] array1)
        {
            Vector<double> v = Vector<double>.Zero;
            int vectorSize = Vector<double>.Count;

            var i = 0;
            for (; i <= array1.Length - vectorSize; i += vectorSize)
            {
                v += new Vector<double>(array1, i);
            }

            double sum = Vector.Dot(v, Vector<double>.One);

            for (; i < array1.Length; i++)
            {
                sum += array1[i];
            }

            return sum;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double LinqFor(double[] array1)
        {
            return array1.Sum(x => x);
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public double ListSimpleFor(List<double> list)
        {
            var sum = 0D;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }
            return sum;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public double ListForeach(List<double> list)
        {
            var sum = 0D;
            foreach (double d in list)
            {
                sum += d;
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double ListLinqFor(List<double> list)
        {
            return list.Sum(x => x);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double ListForSpecialized(List<double> list)
        {
            var sum = 0D;
            list.ForEach(d => sum += d);
            return sum;
        }
    }
}
