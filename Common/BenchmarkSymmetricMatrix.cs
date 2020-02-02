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
    //[SimpleJob(RunStrategy.Monitoring, launchCount: 10, warmupCount: 0, targetCount: 100)]
    //[SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 1)]
    //[DryJob]
    public class BenchmarkSymmetricMatrix
    {
        private const int MATRIX_SIZE = 20;
        private readonly Random random = new Random(0);
        private readonly Matrix matrixA;
        private readonly SymmetricMatrix sMatrixA;

        private readonly Matrix matrixB;
        private readonly Matrix matrixRes;

        private readonly int i;
        private readonly int j;


        public BenchmarkSymmetricMatrix()
        {
            matrixA = Matrix.CreateRandomMatrix(random, MATRIX_SIZE);
            sMatrixA = SymmetricMatrix.CreateRandomMatrix(random, MATRIX_SIZE);
            matrixB = Matrix.CreateRandomMatrix(random, MATRIX_SIZE);

            i = random.Next(MATRIX_SIZE);
            j = random.Next(MATRIX_SIZE);

            matrixRes = new Matrix(MATRIX_SIZE);
        }

        [Benchmark(Baseline = true), System.STAThread]
        public void NaiveProduct() => matrixA.Multiply(matrixB, matrixRes);

        [Benchmark, System.STAThread]
        public void SymmProduct() => sMatrixA.Multiply(matrixB, matrixRes);

        //[Benchmark, System.STAThread]
        //public double NaiveAccess() => matrixA[i, j];

        //[Benchmark, System.STAThread]
        //public double SymmAccess() => sMatrixA[i, j];
    }
}
