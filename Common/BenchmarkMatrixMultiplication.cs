using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DisassemblyDiagnoser(printIL:true, recursiveDepth: 3)]
    //[SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 1)]
    //[DryJob]
    public class BenchmarkMatrixMultiplication
    {
        private const int MATRIX_SIZE = 1000;
        private readonly Random random = new Random(0);
        private readonly Matrix matrixA;
        private readonly Matrix matrixB;
        private readonly Matrix matrixRes;


        private readonly Float.Matrix matrixFA;
        private readonly Float.Matrix matrixFB;
        private readonly Float.Matrix matrixFRes;


        public BenchmarkMatrixMultiplication()
        {
            matrixA = Matrix.CreateRandomMatrix(random, MATRIX_SIZE);
            matrixB = Matrix.CreateRandomMatrix(random, MATRIX_SIZE);
            matrixRes = new Matrix(MATRIX_SIZE);

            matrixFA = Float.Matrix.CreateRandomMatrix(random, MATRIX_SIZE);
            matrixFB = Float.Matrix.CreateRandomMatrix(random, MATRIX_SIZE);
            matrixFRes = new Float.Matrix(MATRIX_SIZE);
        }


        [Benchmark(Baseline = true)]
        public void ProductWithTransposed() => matrixA.ProductWithTransposed(matrixB, matrixRes);
        
        [Benchmark]
        public void ProductWithTransposed_Simd() => matrixA.ProductWithTransposed_Simd(matrixB, matrixRes);

        [Benchmark]
        public void ProductWithTransposed_Span472() => matrixA.ProductWithTransposed_Span472(matrixB, matrixRes);

        [Benchmark]
        public void ProductWithTransposed_Unsafe() => matrixA.ProductWithTransposed_Unsafe(matrixB, matrixRes);

        [Benchmark]
        public void FProductWithTransposed() => matrixFA.ProductWithTransposed(matrixFB, matrixFRes);

        //[Benchmark]
        public void FProductWithTransposed_Simd() => matrixFA.ProductWithTransposed_Simd(matrixFB, matrixFRes);

        //[Benchmark]
        public void FProductWithTransposed_Span472() => matrixFA.ProductWithTransposed_Span472(matrixFB, matrixFRes);

        //[Benchmark]
        public void FProductWithTransposed_Unsafe() => matrixFA.ProductWithTransposed_Unsafe(matrixFB, matrixFRes);

        //[Benchmark]
        public void FProductWithTransposed_Matrix() => matrixFA.ProductWithTransposed_UseMatrix(matrixFB, matrixFRes);
    }
}
