using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace Common
{
    public struct TestStruct
    {
        public readonly int a1;
        public readonly int a2;
        public readonly int a3;
        public readonly int a4;
        public readonly int a5;
        public readonly int a6;

        public int TotalValue() => a1 + a2 + a3 + a4 + a5 + a6;
    }


    [DisassemblyDiagnoser(printIL:true, recursiveDepth: 5)]
    //[SimpleJob(RunStrategy.Monitoring, launchCount: 10, warmupCount: 0, targetCount: 100)]
    //[SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 1)]
    [DryJob]
    public class BenchmarkReadonly
    {

        public double[] array;
        public readonly double[] readonlyArray;

        public  TestStruct testStruct;
        public readonly TestStruct testReadonlyStruct;

        public BenchmarkReadonly()
        {


            array = new double[1000];
            readonlyArray = new double[1000];
        }

        //[Benchmark(Baseline = true), System.STAThread]
        //public bool AccessArray()
        //{
        //    return array[3] != 3 && array[2] != 3 && array[1] != 3 && array[0] != 3;
        //}

        //[Benchmark, System.STAThread]
        //public bool AccessArrayWithLocal()
        //{
        //    var array = this.array;
        //    return array[3] != 3 && array[2] != 3 && array[1] != 3 && array[0] != 3;
        //}


        //[Benchmark, System.STAThread]
        //public bool AccessReadonlyArray()
        //{
        //    return readonlyArray[3] != 3 && readonlyArray[2] != 3 && readonlyArray[1] != 3 && readonlyArray[0] != 3;
        //}

        //[Benchmark, System.STAThread]
        //public double NaiveAccess() => matrixA[i, j];

        //[Benchmark, System.STAThread]
        //public double SymmAccess() => sMatrixA[i, j];


        [Benchmark, System.STAThread]
        public int SimpleTest() => testStruct.TotalValue();


        [Benchmark, System.STAThread]
        public int ReadOnlyTest() => testReadonlyStruct.TotalValue();
    }
}
