using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using Common;
using Extreme.Mathematics.LinearAlgebra;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Console.WriteLine("Vector<float>.Count : " + Vector<float>.Count);
            Console.WriteLine("Vector<double>.Count : " + Vector<double>.Count);
            Console.WriteLine("sizeof(float) : " + sizeof(float));
            Console.WriteLine("sizeof(double) : " + sizeof(double));
            unsafe
            {
                Console.WriteLine("sizeof(Vector<double>) : " + sizeof(Vector<double>));
                Console.WriteLine("sizeof(Matrix4x4) : " + sizeof(Matrix4x4));
            };

            var summary = BenchmarkRunner.Run<BenchmarkMatrixMultiplication>();
        }
    }
}
