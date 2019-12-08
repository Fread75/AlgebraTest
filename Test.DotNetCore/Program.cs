using BenchmarkDotNet.Running;
using System;
using System.Numerics;
using Common;

namespace Test.DotNetCore
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
