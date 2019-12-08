using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using NumType = System.Double;

namespace Common
{
    public class Matrix
    {
        public readonly NumType[] array;
        private readonly int size;

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
        private unsafe static NumType SumProduct(NumType* ptrA, NumType* ptrB, int size)
        {
            NumType res = 0;
            for (int i = 0; i < size; i++)
            {
                res += ptrA[i] * ptrB[i];
            }
            return res;
        }
    }
}
