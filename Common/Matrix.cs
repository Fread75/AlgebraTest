using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using NumType = System.Double;

namespace Common
{
    internal static class Algebra
    {
        internal static void Multiply<TMatrix>(TMatrix a, TMatrix b, TMatrix result) where TMatrix : IMatrix
        {
            for (int i = 0; i < a.Size; i++)
            {
                for (int j = 0; j < a.Size; j++)
                {
                    double temp = 0;
                    for (int k = 0; k < a.Size; k++)
                    {
                        temp += a[i, k] * b[k, j];
                    }
                    result[i, j] = temp;
                }
            }
        }
        internal static void Multiply(Matrix a, Matrix b, Matrix result)
        {
            for (int i = 0; i < a.Size; i++)
            {
                for (int j = 0; j < a.Size; j++)
                {
                    double temp = 0;
                    for (int k = 0; k < a.Size; k++)
                    {
                        temp += a[i, k] * b[k, j];
                    }
                    result[i, j] = temp;
                }
            }
        }
    }


    internal interface IMatrix
    {
        NumType this[int i, int j] { get; set; }
        int Size { get; }
    }


    public sealed class Matrix : IMatrix
    {
        public readonly NumType[] array;
        private readonly int size;

        public int Size => size;

        public NumType this [int i, int j]
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => array[i * size + j];
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => array[i * size + j] = value;
        }

        public Matrix(int size) : this (new NumType[size * size], size) { }

        public Matrix(NumType[] array, int size)
        {
            this.array = array;
            this.size = size;
        }

        public static Matrix CreateRandomMatrix(Random random, int size)
        {
            var maxValue = Math.Min(500, Math.Sqrt(NumType.MaxValue / size));

            var array = new NumType[size * size];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.NextDouble() * maxValue;
            }
            return new Matrix(array, size);
        }

        public void ProductWithTransposed(Matrix B, Matrix res)
        {
            for (int i = 0; i < size; i++)
            {
                var offset = i * size;
                var spanRowA = new SafeSpan<NumType>(array, offset, size);
                var spanRowRes = new SafeSpan<NumType>(res.array, offset, size);

                for (int j = 0; j < size; j++)
                {
                    var spanRowB = new SafeSpan<NumType>(B.array, j * size, size);
                    spanRowRes[j] = SumProduct(spanRowA, spanRowB);
                }
            }
        }

        public void ProductWithTransposed_MultiThreading(Matrix B, Matrix res)
        {
            //Parallel.For(0, size, i =>
            //{
            //    var offset = i * size;
            //    var spanRowA = new SafeSpan<NumType>(array, offset, size);
            //    var spanRowRes = new SafeSpan<NumType>(res.array, offset, size);

            //    for (int j = 0; j < size; j++)
            //    {
            //        var spanRowB = new SafeSpan<NumType>(B.array, j * size, size);
            //        spanRowRes[j] = SumProduct(spanRowA, spanRowB);
            //    }
            //});
        }

        public void ProductWithTransposed_Simd(Matrix B, Matrix res)
        {
            for (int i = 0; i < size; i++)
            {
                var offset = i * size;
                var spanRowA = new SafeSpan<NumType>(array, offset, size);
                var spanRowRes = new SafeSpan<NumType>(res.array, offset, size);

                for (int j = 0; j < size; j++)
                {
                    var spanRowB = new SafeSpan<NumType>(B.array, j * size, size);
                    spanRowRes[j] = SumProduct_Simd(spanRowA, spanRowB);
                }
            }
        }

        public unsafe void ProductWithTransposed_Unsafe(Matrix B, Matrix res)
        {
            fixed (NumType* ptrA = array)
            fixed (NumType* ptrRes = res.array)
            fixed (NumType* ptrB = B.array)
            {
                for (int i = 0; i < size; i++)
                {
                    var offset = i * size;
                    var spanRowA = ptrA + offset;
                    var spanRowRes = ptrRes + offset;

                    for (int j = 0; j < size; j++)
                    {
                        var spanRowB = ptrB + (j * size);
                        spanRowRes[j] = SumProduct(spanRowA, spanRowB, size);
                    }
                }
            }
        }

        public void ProductWithTransposed_Span472(Matrix B, Matrix res)
        {
            for (int i = 0; i < size; i++)
            {
                var offset = i * size;
                var spanRowA = new Span<NumType>(array, offset, size);
                var spanRowRes = new Span<NumType>(res.array, offset, size);

                for (int j = 0; j < size; j++)
                {
                    var spanRowB = new Span<NumType>(B.array, j * size, size);
                    spanRowRes[j] = SumProduct(spanRowA, spanRowB);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NumType SumProduct(SafeSpan<NumType> spanA, SafeSpan<NumType> spanB)
        {
            NumType res = 0;
            for (int i = 0; i < spanA.Length; i++)
            {
                res += spanA[i] * spanB[i];
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NumType SumProduct_Simd(SafeSpan<NumType> spanA, SafeSpan<NumType> spanB)
        {
            var simdLength = Vector<NumType>.Count;

            var resVec = Vector<NumType>.Zero;

            var i = 0;
            for (; i <= spanA.Length - simdLength; i += simdLength)
            {
                var va = new Vector<NumType>(spanA.Array, spanA.Start + i);
                var vb = new Vector<NumType>(spanB.Array, spanB.Start + i);
                resVec += (va * vb);
            }

            var res = Vector.Dot(resVec, Vector<NumType>.One);

            for (; i < spanA.Length; ++i)
            {
                res += spanA[i] + spanB[i];
            }

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NumType SumProduct(Span<NumType> spanA, Span<NumType> spanB)
        {
            NumType res = 0;
            for (int i = 0; i < spanA.Length; i++)
            {
                res += spanA[i] * spanB[i];
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe NumType SumProduct(NumType* ptrA, NumType* ptrB, int size)
        {
            NumType res = 0;
            for (int i = 0; i < size; i++)
            {
                res += ptrA[i] * ptrB[i];
            }
            return res;
        }

        private void Write(string file)
        {
            using (var writer = File.OpenWrite(file))
            {
                writer.Write(null, 0, 0);
                writer.Flush();
            }
        }
        public void Multiply(Matrix other, Matrix res)
        {
            for (int i = 0; i < res.Size; i++)
            {
                for (int j = 0; j < res.Size; j++)
                {
                    double value = 0;
                    for (int k = 0; k < res.Size; k++)
                    {
                        value += this[i, j] * other[j, i];
                    }
                    res[i, j] = value;
                }
            }
        }

    }
}
