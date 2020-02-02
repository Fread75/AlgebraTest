using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace Common
{

    [DisassemblyDiagnoser(printIL:true, recursiveDepth: 5)]
    //[SimpleJob(RunStrategy.Monitoring, launchCount: 10, warmupCount: 0, targetCount: 100)]
    //[SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 1)]
    [DryJob]
    public class BenchmarkPure
    {

        private double a;

        public BenchmarkPure()
        {
            var random = new Random();
            a = random.NextDouble();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public double GetAValue() => a;

        [Pure]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public double PureGetAValue() => a;


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
        public bool SimpleTest()
        {
            var min = 0.2;
            var max = 0.9;

            if (GetAValue() < min)
                return false;


            if (GetAValue() > max)
                return false;

            return true;
        }


        [Benchmark, System.STAThread]
        public bool PureTest()
        {
            var min = 0.2;
            var max = 0.9;

            if (PureGetAValue() < min)
                return false;


            if (PureGetAValue() > max)
                return false;

            return true;
        }
    }
}
