using System;
using System.Collections.Generic;
using Common;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using NumType = System.Single;

namespace Common.Float
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
            var maxValue = (NumType)Math.Min(500, Math.Sqrt(NumType.MaxValue / size));

            var array = new NumType[size * size];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (NumType)(random.NextDouble() * maxValue);
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    sb.Append(array[(i * size) + j]);
                    sb.Append('\t');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public void ProductWithTransposed_UseMatrix(Matrix B, Matrix res)
        {
            B.TransposeInPlace();

            const int matrixSize = 4;
            int lineSize = size / 4;
            Matrix4x4[] rowA = new Matrix4x4[lineSize];
            Matrix4x4[] rowB = new Matrix4x4[lineSize];
            Matrix4x4[] rowRes = new Matrix4x4[lineSize];

            for (int i = 0; i  <= size - matrixSize; i += matrixSize)
            {
                FillMatrixLine(array, rowA, i);
                for (int j = 0; j <= size - matrixSize; j += matrixSize)
                {
                    FillColMatrixLine(B.array, rowB, j);
                    rowRes[j / 4] = Multiply(rowA, rowB);
                }
                FillWithMatrix4x4(res.array, rowRes, i);

            }
        }

        public void TransposeInPlace()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = i; j < size; j++)
                {
                    var temp = array[i * size + j];
                    array[i * size + j] = array[j * size + i];
                    array[j * size + i] = temp;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillWithMatrix4x4(NumType[] res, Matrix4x4[] row, int rowNumber)
        {
            var row1 = rowNumber * size;
            var row2 = (rowNumber + 1) * size;
            var row3 = (rowNumber + 2) * size;
            var row4 = (rowNumber + 3) * size;

            for (int i = 0; i < row.Length; i++)
            {
                var col1 = i * 4;
                var col2 = col1 + 1;
                var col3 = col2 + 1;
                var col4 = col3 + 1;

                res[row1 + col1] = row[i].M11;
                res[row1 + col2] = row[i].M12;
                res[row1 + col3] = row[i].M13;
                res[row1 + col4] = row[i].M14;
                res[row2 + col1] = row[i].M21;
                res[row2 + col2] = row[i].M22;
                res[row2 + col3] = row[i].M23;
                res[row2 + col4] = row[i].M24;
                res[row3 + col1] = row[i].M31;
                res[row3 + col2] = row[i].M32;
                res[row3 + col3] = row[i].M33;
                res[row3 + col4] = row[i].M34;
                res[row4 + col1] = row[i].M41;
                res[row4 + col2] = row[i].M42;
                res[row4 + col3] = row[i].M43;
                res[row4 + col4] = row[i].M44;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillMatrixLine(NumType[] array, Matrix4x4[] row, int rowNumber)
        {
            var row1 = rowNumber * size;
            var row2 = (rowNumber + 1) * size;
            var row3 = (rowNumber + 2) * size;
            var row4 = (rowNumber + 3) * size;

            for (int i = 0; i < row.Length; i++)
            {
                var col1 = i * 4;
                var col2 = col1 + 1;
                var col3 = col2 + 1;
                var col4 = col3 + 1;

                row[i] = new Matrix4x4(
                    array[row1 + col1],
                    array[row1 + col2],
                    array[row1 + col3],
                    array[row1 + col4],
                    array[row2 + col1],
                    array[row2 + col2],
                    array[row2 + col3],
                    array[row2 + col4],
                    array[row3 + col1],
                    array[row3 + col2],
                    array[row3 + col3],
                    array[row3 + col4],
                    array[row4 + col1],
                    array[row4 + col2],
                    array[row4 + col3],
                    array[row4 + col4]
                    );
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillColMatrixLine(NumType[] array, Matrix4x4[] row, int col)
        {
            var col2 = col + 1;
            var col3 = col2 + 1;
            var col4 = col3 + 1;

            for (int i = 0; i < row.Length; i++)
            {
                var offset = i * 4;

                var row1 = offset * size;
                var row2 = (offset + 1) * size;
                var row3 = (offset + 2) * size;
                var row4 = (offset + 3) * size;

                row[i] = new Matrix4x4(
                    array[row1 + col],
                    array[row1 + col2],
                    array[row1 + col3],
                    array[row1 + col4],
                    array[row2 + col],
                    array[row2 + col2],
                    array[row2 + col3],
                    array[row2 + col4],
                    array[row3 + col],
                    array[row3 + col2],
                    array[row3 + col3],
                    array[row3 + col4],
                    array[row4 + col],
                    array[row4 + col2],
                    array[row4 + col3],
                    array[row4 + col4]
                    );
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix4x4 Multiply(Matrix4x4[] rowA, Matrix4x4[] rowB)
        {
            Matrix4x4 res = new Matrix4x4();
            for (int i = 0; i < rowA.Length; i++)
            {
                res += Matrix4x4.Multiply(rowA[i], rowB[i]);
            }
            return res;
        }
    }
}
