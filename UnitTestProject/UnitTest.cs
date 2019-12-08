using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest
    {
        private const int DEFAULT_SIZE = 200;
        private const double PRECISION = 10e-15;
        private const float FLOAT_PRECISION = 10e-6f;

        private readonly Matrix matA;
        private readonly Matrix matB;
        private readonly Matrix expectedRes;

        private readonly Common.Float.Matrix matFA;
        private readonly Common.Float.Matrix matFB;
        private readonly Common.Float.Matrix expectedFRes;

        public UnitTest()
        {
            var random = new Random(0);
            matA = Matrix.CreateRandomMatrix(random, DEFAULT_SIZE);
            matB = Matrix.CreateRandomMatrix(random, DEFAULT_SIZE);
            expectedRes = new Matrix(DEFAULT_SIZE);

            matA.ProductWithTransposed(matB, expectedRes);

            matFA = Common.Float.Matrix.CreateRandomMatrix(random, DEFAULT_SIZE);
            matFB = Common.Float.Matrix.CreateRandomMatrix(random, DEFAULT_SIZE);
            expectedFRes = new Common.Float.Matrix(DEFAULT_SIZE);

            matFA.ProductWithTransposed(matFB, expectedFRes);
        }

        [TestMethod]
        public void ProductWithTransposed_Simd()
        {
            var res = new Matrix(DEFAULT_SIZE);

            matA.ProductWithTransposed_Simd(matB, res);

            AssertArrayEqual(expectedRes.array, res.array, PRECISION);
        }

        [TestMethod]
        public void ProductWithTransposed_Span472()
        {
            var res = new Matrix(DEFAULT_SIZE);

            matA.ProductWithTransposed_Span472(matB, res);

            AssertArrayEqual(expectedRes.array, res.array);
        }

        [TestMethod]
        public void ProductWithTransposed_Unsafe()
        {
            var res = new Matrix(DEFAULT_SIZE);

            matA.ProductWithTransposed_Unsafe(matB, res);

            AssertArrayEqual(expectedRes.array, res.array);
        }


        [TestMethod]
        public void FProductWithTransposed_Simd()
        {
            var res = new Common.Float.Matrix(DEFAULT_SIZE);

            matFA.ProductWithTransposed_Simd(matFB, res);

            AssertArrayEqual(expectedFRes.array, res.array, FLOAT_PRECISION);
        }

        [TestMethod]
        public void FProductWithTransposed_UseMatrix()
        {
            var res = new Common.Float.Matrix(DEFAULT_SIZE);

            matFA.ProductWithTransposed_UseMatrix(matFB, res);

            AssertArrayEqual(expectedFRes.array, res.array, FLOAT_PRECISION);
        }

        [TestMethod]
        public void FProductWithTransposed_Span472()
        {
            var res = new Common.Float.Matrix(DEFAULT_SIZE);

            matFA.ProductWithTransposed_Span472(matFB, res);

            AssertArrayEqual(expectedFRes.array, res.array);
        }

        [TestMethod]
        public void FProductWithTransposed_Unsafe()
        {
            var res = new Common.Float.Matrix(DEFAULT_SIZE);

            matFA.ProductWithTransposed_Unsafe(matFB, res);

            AssertArrayEqual(expectedFRes.array, res.array);
        }

        private void AssertArrayEqual<T>(T[] arrayExpected, T[] arrayActual)
        {
            Assert.AreEqual(arrayExpected.Length, arrayActual.Length);

            for (int i = 0; i < arrayExpected.Length; i++)
            {
                Assert.AreEqual(arrayExpected[i], arrayActual[i]);
            }
        }

        private void AssertArrayEqual(double[] arrayExpected, double[] arrayActual, double precision)
        {
            Assert.AreEqual(arrayExpected.Length, arrayActual.Length);

            for (int i = 0; i < arrayExpected.Length; i++)
            {
                var expected = arrayExpected[i];
                var actual = arrayActual[i];
                var relativeDifference = Math.Abs(expected - actual) / expected;
                Assert.IsTrue(relativeDifference < precision, $"expected : {expected}, got : {actual} (diff : {relativeDifference}");
            }
        }

        private void AssertArrayEqual(float[] arrayExpected, float[] arrayActual, float precision)
        {
            Assert.AreEqual(arrayExpected.Length, arrayActual.Length);

            for (int i = 0; i < arrayExpected.Length; i++)
            {
                var expected = arrayExpected[i];
                var actual = arrayActual[i];
                var relativeDifference = Math.Abs(expected - actual) / expected;
                Assert.IsTrue(relativeDifference < precision, $"expected : {expected}, got : {actual} (diff : {relativeDifference}");
            }
        }
    }
}
