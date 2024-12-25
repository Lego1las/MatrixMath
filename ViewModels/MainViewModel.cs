using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace MatrixMath.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private MatrixViewModel _matrixA;
        private MatrixViewModel _matrixInverseA;

        private MatrixViewModel _matrixB;
        private MatrixViewModel _matrixInverseB;

        private MatrixViewModel _matrixMul;
        private MatrixViewModel _matrixSum;
        private MatrixViewModel _matrixDif;

        public MainViewModel()
        {
            // Инициализация
            MatrixA = new MatrixViewModel
            {
                MatrixHeader = "Матрица A",
                MatrixSize = 3
            };

            MatrixB = new MatrixViewModel
            {
                MatrixHeader = "Матрица B",
                MatrixSize = 3
            };

            MatrixInverseA = new MatrixViewModel
            {
                MatrixHeader = "Обратная матрица A",
                IsReadOnly = true
            };
            MatrixInverseB = new MatrixViewModel
            {
                MatrixHeader = "Обратная матрица B",
                IsReadOnly = true
            };

            MatrixMul = new MatrixViewModel
            {
                MatrixHeader = "A * B",
                IsReadOnly = true
            };
            MatrixSum = new MatrixViewModel
            {
                MatrixHeader = "A + B",
                IsReadOnly = true
            };
            MatrixDif = new MatrixViewModel
            {
                MatrixHeader = "A - B",
                IsReadOnly = true
            };


            // Подписываемся на события PropertyChanged
            MatrixA.PropertyChanged += OnMatrixAChanged;
            MatrixB.PropertyChanged += OnMatrixBChanged;
            // Первичная инициализация
            MatrixA.InitMatrix();
            MatrixB.InitMatrix();
        }

        #region Свойства

        public MatrixViewModel MatrixA
        {
            get => _matrixA;
            set
            {
                if (_matrixA != value)
                {
                    _matrixA = value;
                    OnPropertyChanged();
                }
            }
        }

        public MatrixViewModel MatrixInverseA
        {
            get => _matrixInverseA;
            set
            {
                if (_matrixInverseA != value)
                {
                    _matrixInverseA = value;
                    OnPropertyChanged();
                }
            }
        }

        public MatrixViewModel MatrixB
        {
            get => _matrixB;
            set
            {
                if (_matrixB != value)
                {
                    _matrixB = value;
                    OnPropertyChanged();
                }
            }
        }

        public MatrixViewModel MatrixInverseB
        {
            get => _matrixInverseB;
            set
            {
                if (_matrixInverseB != value)
                {
                    _matrixInverseB = value;
                    OnPropertyChanged();
                }
            }
        }

        public MatrixViewModel MatrixMul
        {
            get => _matrixMul;
            set
            {
                if (_matrixMul != value)
                {
                    _matrixMul = value;
                    OnPropertyChanged();
                }
            }
        }

        public MatrixViewModel MatrixSum
        {
            get => _matrixSum;
            set
            {
                if (_matrixSum != value)
                {
                    _matrixSum = value;
                    OnPropertyChanged();
                }
            }
        }

        public MatrixViewModel MatrixDif
        {
            get => _matrixDif;
            set
            {
                if (_matrixDif != value)
                {
                    _matrixDif = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Обработчики изменений матриц A и B

        private void OnMatrixAChanged(object sender, PropertyChangedEventArgs e)
        {
            // Если меняется содержимое матрицы или её размер,
            // пересчитываем детерминант/обратную A и результаты
            if (e.PropertyName == nameof(MatrixViewModel.Matrix) ||
                e.PropertyName == nameof(MatrixViewModel.MatrixSize))
            {
                // 1) Считаем det(A) и обратную A
                MatrixA.CalcDeterminantAndInverse();

                // 2) Обновляем "обратную матрицу A"
                MatrixInverseA.MatrixSize = MatrixA.MatrixSize;
                MatrixInverseA.Matrix = MatrixA.InverseMatrix
                                        ?? new float[MatrixA.MatrixSize, MatrixA.MatrixSize];

                // 3) Пересчитываем все операции (A+B, A-B, A*B)
                RecalculateAllOperations();
            }
        }

        private void OnMatrixBChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatrixViewModel.Matrix) ||
                e.PropertyName == nameof(MatrixViewModel.MatrixSize))
            {;
                // 1) Считаем det(B) и обратную B
                MatrixB.CalcDeterminantAndInverse();

                // 2) Обновляем "обратную матрицу B"
                MatrixInverseB.MatrixSize = MatrixB.MatrixSize;
                MatrixInverseB.Matrix = MatrixB.InverseMatrix
                                        ?? new float[MatrixB.MatrixSize, MatrixB.MatrixSize];

                // 3) Пересчитываем все операции (A+B, A-B, A*B)
                RecalculateAllOperations();
            }
        }

        #endregion

        #region Пересчёт (A+B, A-B, A*B)

        private void RecalculateAllOperations()
        {
            // Если у A и B разные размеры, 
            // обнуляем матрицы результата или пропускаем
            if (MatrixA.MatrixSize != MatrixB.MatrixSize)
            {
                MatrixSum.Matrix = null;
                MatrixDif.Matrix = null;
                MatrixMul.Matrix = null;
                return;
            }

            var size = MatrixA.MatrixSize;
            // Предположим, эти методы у вас уже реализованы где-то:
            var sum = MatrixFunctions.GetMatrixSum(size, MatrixA.Matrix, MatrixB.Matrix);
            var dif = MatrixFunctions.GetMatrixDif(size, MatrixA.Matrix, MatrixB.Matrix);
            var mul = MatrixFunctions.GetMatrixMul(size, MatrixA.Matrix, MatrixB.Matrix);

            // Записываем полученные результаты в VM (чтобы UI их увидел)
            MatrixSum.MatrixSize = size;
            MatrixSum.Matrix = sum;

            MatrixDif.MatrixSize = size;
            MatrixDif.Matrix = dif;

            MatrixMul.MatrixSize = size;
            MatrixMul.Matrix = mul;
        }

        public void ForceMatrixChanged(MatrixViewModel matrix)
        {
            OnPropertyChanged(nameof(matrix));
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
