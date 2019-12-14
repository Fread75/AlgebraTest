using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

using DenseMatrix = MathNet.Numerics.LinearAlgebra.Double.DenseMatrix;

namespace Common
{
    [DisassemblyDiagnoser(printIL:true, recursiveDepth: 5)]
    [SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 1)]
    //[DryJob]
    public class BenchmarkMatrixMultiplication
    {
        private const int MATRIX_SIZE = 400;
        private readonly Random random = new Random(0);
        private readonly Matrix matrixA;
        private readonly Matrix matrixB;
        private readonly Matrix matrixRes;

        private readonly DenseMatrix mnMatrixA;
        private readonly DenseMatrix mnMatrixB;
        private readonly DenseMatrix mnMatrixRes;

        public BenchmarkMatrixMultiplication()
        {
            matrixA = Matrix.CreateRandomMatrix(random, MATRIX_SIZE);
            matrixB = Matrix.CreateRandomMatrix(random, MATRIX_SIZE);
            matrixRes = new Matrix(MATRIX_SIZE);
            
            mnMatrixA = (DenseMatrix)Matrix<double>.Build.Random(MATRIX_SIZE, MATRIX_SIZE);
            mnMatrixB = (DenseMatrix)Matrix<double>.Build.Random(MATRIX_SIZE, MATRIX_SIZE);
            mnMatrixRes = (DenseMatrix)Matrix<double>.Build.Dense(MATRIX_SIZE, MATRIX_SIZE);
        }


        [GlobalSetup(Target = nameof(MathNetNumericsManagedProduct))]
        public void SetupManaged()
        {
            Control.UseSingleThread();
            Control.UseManaged();
        }

        [GlobalSetup(Target = nameof(MathNetNumericsManagedReferenceProduct))]
        public void SetupManagedReference()
        {
            Control.UseSingleThread();
            Control.UseManagedReference();
        }
        [GlobalSetup(Target = nameof(MathNetNumericsMklProduct))]
        public void SetupMkl()
        {
            Control.UseSingleThread();
            Control.UseNativeMKL();
        }

        [GlobalSetup(Target = nameof(MathNetNumericsManagedProduct_MultiThreading))]
        public void SetupManaged_MultiThreading()
        {
            Control.UseMultiThreading();
            Control.UseManaged();
        }

        [GlobalSetup(Target = nameof(MathNetNumericsManagedReferenceProduct_MultiThreading))]
        public void SetupManagedReference_MultiThreading()
        {
            Control.UseMultiThreading();
            Control.UseManagedReference();
        }
        [GlobalSetup(Target = nameof(MathNetNumericsMklProduct_MultiThreading))]
        public void SetupMkl_MultiThreading()
        {
            Control.UseMultiThreading();
            Control.UseNativeMKL();
        }

        [Benchmark(Baseline = true), System.STAThread]
        public void NaiveProduct() => matrixA.ProductWithTransposed(matrixB, matrixRes);

        [Benchmark, System.STAThread]
        public void MathNetNumericsManagedProduct() => mnMatrixA.TransposeAndMultiply(mnMatrixB, mnMatrixRes);
        [Benchmark, System.STAThread]
        public void MathNetNumericsManagedReferenceProduct() => mnMatrixA.TransposeAndMultiply(mnMatrixB, mnMatrixRes);
        [Benchmark, System.STAThread]
        public void MathNetNumericsMklProduct() => mnMatrixA.TransposeAndMultiply(mnMatrixB, mnMatrixRes);
        [Benchmark, System.STAThread]
        public void MathNetNumericsManagedProduct_MultiThreading() => mnMatrixA.TransposeAndMultiply(mnMatrixB, mnMatrixRes);
        [Benchmark, System.STAThread]
        public void MathNetNumericsManagedReferenceProduct_MultiThreading() => mnMatrixA.TransposeAndMultiply(mnMatrixB, mnMatrixRes);
        [Benchmark, System.STAThread]
        public void MathNetNumericsMklProduct_MultiThreading() => mnMatrixA.TransposeAndMultiply(mnMatrixB, mnMatrixRes);
        [Benchmark]
        public void ProductWithTransposed_Simd() => matrixA.ProductWithTransposed_Simd(matrixB, matrixRes);
        [Benchmark]
        public void MathNetNumerics_Simd() => mnMatrixA.TransposeAndMultiply(mnMatrixB, mnMatrixRes);
        [Benchmark]
        public void ProductWithTransposed_Span472() => matrixA.ProductWithTransposed_Span472(matrixB, matrixRes);
        [Benchmark]
        public void ProductWithTransposed_Unsafe() => matrixA.ProductWithTransposed_Unsafe(matrixB, matrixRes);
    }
}
