using System;

namespace MatrixMath
{
    class MatrixFunctions
    {
        #region ВЫЧИСЛЕНИЕ ОПРЕДЕЛИТЕЛЯ, ПРИВЕДЕНИЕМ МАТРИЦЫ К ДИАГОНАЛЬНОМУ ВИДУ
        //поиск столбца для перестановки (в случае, когда диагональный элемент равен 0)
        //возвращает 0, если в столбце все элементы нулевые
        private static int FindColForSwap(int c, int sizeMatrix, float[,] mass)
        {
            for (var i = c + 1; i < sizeMatrix; i++)
                if (Math.Abs(mass[c, i]) > 0.0000001) return i;

            return 0;
        }
        //меняем местами столбца с номерами k и l
        private static void SwapColumns(int k, int l, int n, float[,] mass)
        {
            for (var i = 0; i < n; i++)
            {
                var tmp = mass[i, k];
                mass[i, k] = mass[i, l];
                mass[i, l] = tmp;
            }
        }



        //вычисление определителя

        public static float CalcDeterminant(int n, float[,] mass)
        {
            float det = 1;
            var M = mass;

            //приводим матрицу к диагональному виду
            for (var i = 0; i < n; i++)
            {

                //если на диагонали стоит 0, то определяем столбец, с которым можно поменять i-й
                //если такого столбца нет, то определитель равен 0
                if (Math.Abs(M[i, i]) < 0.0000001)
                {
                    var cs = FindColForSwap(i, n, M);
                    if (cs == 0) return 0; //если все элементы строки равны 0, то и сам определитель равен 0
                    //если столбцы (или строки) матрицы поменять местами, то определитель поменяет знак 
                    det = -det;
                    //меняем местами столбцы с номерами i и cs
                    SwapColumns(i, cs, n, M);
                }
                var a = M[i, i];

                for (var j = i + 1; j < n; j++)
                {
                    var b = M[j, i];
                    for (var k = i; k < n; k++)
                        //прибавляем к j-ой строке строку с номером i, умноженную на -b/a
                        //чтобы j-ой строке на i-м месте получить 0
                        M[j, k] = -M[i, k] * b / a + M[j, k];
                }
            }
            //определитель диагональной матрицы равен произведению диагональных элементов
            for (var i = 0; i < n; i++)
                det *= M[i, i];

            return det;
        }
        #endregion

        #region ВЫЧИСЛЕНИЕ ОБРАТНОЙ МАТРИЦЫ

        private static float[,] GetMinor(int matrixSize, int row, int col, float[,] m)
        {
            var minor = new float[matrixSize - 1, matrixSize - 1];
            var r = 0;

            for (var i = 0; i < matrixSize; i++)
            {
                if (i == row) continue;

                var c = 0;
                for (var j = 0; j < matrixSize; j++)
                {

                    if (j != col)
                    {
                        minor[r, c] = m[i, j];
                        c++;
                    }

                }
                r++;

            }
            return minor;
        }
        public static float[,] CalcInverseMatrix(int matrixSize, float determinant, float[,] m)
        {
            var c = new float[matrixSize, matrixSize];
            for (var i = 0; i < matrixSize; i++)
            {
                for (var j = 0; j < matrixSize; j++)
                {
                    c[i, j] = CalcDeterminant(matrixSize - 1, GetMinor(matrixSize, i, j, m));
                    c[i, j] *= ((i + j) % 2 == 1) ? -1 : 1;
                    c[i, j] /= determinant;
                }
            }
            c = GetTransposedMatrix(matrixSize, c);
            return c;
        }
        #endregion

        public static float[,] GetTransposedMatrix(int matrixSize, float[,] m)
        {
            var tm = m;
            for (var i = 0; i < matrixSize; i++)
            {
                for (var j = i + 1; j < matrixSize; j++)
                {
                    var x = tm[i, j];
                    tm[i, j] = tm[j, i];
                    tm[j, i] = x;
                }
            }
            return tm;
        }

        public static float[,] GetMatrixSum(int matrixSize, float[,] a, float[,] b)
        {
            var tm = a;
            for (var i = 0; i < matrixSize; i++)
                for (var j = 0; j < matrixSize; j++) tm[i, j] += b[i, j];


            return tm;
        }
        public static float[,] GetMatrixDif(int matrixSize, float[,] a, float[,] b)
        {
            var tm = a;
            for (var i = 0; i < matrixSize; i++)
                for (var j = 0; j < matrixSize; j++) tm[i, j] -= b[i, j];
            return tm;
        }
        /// <summary>
        /// произведение матриц
        /// </summary>
        public static float[,] GetMatrixMul(int matrixSize, float[,] a, float[,] b)
        {
            var c = new float[matrixSize, matrixSize];
            for (var i = 0; i < matrixSize; i++)
                for (var j = 0; j < matrixSize; j++)
                {
                    c[i, j] = 0;
                    for (var k = 0; k < matrixSize; k++)
                        c[i, j] += a[i, k] * b[k, j];
                }
            return c;
        }

    }
}
