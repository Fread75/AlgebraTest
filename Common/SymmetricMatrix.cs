using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Common
{
    public class SymmetricMatrix
    {
        private readonly double[] storage;
        private readonly int count;

        private SymmetricMatrix(int count, double[] storage)
        {
            this.count = count;
            this.storage = storage;
        }

        public SymmetricMatrix(int count)
            : this(count, new double[(1 + count) * count / 2])
        {
        }

        public double this[int i, int j]
        {
            get => storage[GetInternalIndex(i, j)];
            set => storage[GetInternalIndex(i, j)] = value;
        }
        public static SymmetricMatrix CreateRandomMatrix(Random random, int size)
        {
            var maxValue = Math.Min(500, Math.Sqrt(double.MaxValue / size));

            var array = new double[(1 + size) * size / 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.NextDouble() * maxValue;
            }
            return new SymmetricMatrix(size, array);
        }


        private static int GetInternalIndex(int i, int j)
        {
            if (i > j)
                return (1 + j) * j / 2 + i;

            return (1 + i) * i / 2 + j;
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
