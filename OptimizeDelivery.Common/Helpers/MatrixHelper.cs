using System;
using System.Collections.Generic;
using Itinero.Algorithms.Weights;

namespace Common.Helpers
{
    public static class MatrixHelper
    {
        private const long RouteNotFoundDefaultValue = -1;

        public static long[,] ForOptimization(this float[][] matrix)
        {
            var height = matrix.Length;
            var width = matrix[0].Length;
            var longMatrix = new long[width, height];
            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
                longMatrix[i, j] = matrix[i][j] == float.MaxValue
                    ? RouteNotFoundDefaultValue
                    : Convert.ToInt64(matrix[i][j]);

            return longMatrix;
        }

        public static void OutputMatrix(this float[][] matrix)
        {
            var rowLength = matrix.GetLength(0);
            var colLength = matrix[0].GetLength(0);
            for (var i = 0; i < rowLength; i++)
            {
                for (var j = 0; j < colLength; j++) Console.Write($"{matrix[i][j],12} ");
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

        public static T[,] CreateRectangularArray<T>(this IList<T[]> arrays)
        {
            var minorLength = arrays[0].Length;
            var ret = new T[arrays.Count, minorLength];
            for (var i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException("All arrays must be the same length");
                }
                for (var j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }
    }
}