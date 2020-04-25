using System;
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

        private static long[,] ForOptimization(this Weight[][] matrix)
        {
            var height = matrix.Length;
            var width = matrix[0].Length;
            var longMatrix = new long[width, height];
            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
                longMatrix[i, j] = matrix[i][j].Time == float.MaxValue
                    ? RouteNotFoundDefaultValue
                    : Convert.ToInt64(matrix[i][j].Time);

            return longMatrix;
        }
    }
}