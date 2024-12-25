using GalaSoft.MvvmLight.CommandWpf;
using MatrixMath;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MatrixMath.ViewModels
{
    public class MatrixViewModel : INotifyPropertyChanged
    {
        private int _matrixSize;
        private float[,] _matrix;
        private float? _determinant;
        private float[,] _inverseMatrix;
        private bool _isReadOnly;
        private bool _buttonsVisible;
        private string _matrixHeader;

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Событие, уведомляющее, что какая-то ячейка изменилась (если нужно).
        /// </summary>
        public event EventHandler CellChanged;

        public MatrixViewModel()
        {
            _matrixSize = 3;
            _matrix = new float[_matrixSize, _matrixSize];
            _determinant = null;
            _buttonsVisible = true;
            _matrixHeader = string.Empty;
        }

        /// <summary>
        /// Размер матрицы.
        /// При изменении пересоздаём float[,] и сбрасываем детерминант.
        /// </summary>
        public int MatrixSize
        {
            get => _matrixSize;
            set
            {
                if (_matrixSize != value)
                {
                    _matrixSize = value;

                    // Пересоздаём массив:
                    _matrix = new float[_matrixSize, _matrixSize];
                    _determinant = null;
                    _inverseMatrix = null;

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Основная матрица (двухмерный массив).
        /// </summary>
        public float[,] Matrix
        {
            get => _matrix;
            set
            {
                if (_matrix != value)
                {
                    _matrix = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Детерминант (null, если ещё не считали или равен 0).
        /// </summary>
        public float? Determinant
        {
            get => _determinant;
            private set
            {
                if (_determinant != value)
                {
                    _determinant = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Обратная матрица (null, если не существует).
        /// </summary>
        public float[,] InverseMatrix
        {
            get => _inverseMatrix;
            private set
            {
                if (_inverseMatrix != value)
                {
                    _inverseMatrix = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Заголовок матрицы (то, что отображается, например, над матрицей).
        /// </summary>
        public string MatrixHeader
        {
            get => _matrixHeader;
            set
            {
                if (_matrixHeader != value)
                {
                    _matrixHeader = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Флаг "только для чтения".
        /// При установке true, скрываем кнопки +/-, блокируем ввод и т.д. (по желанию).
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    ButtonsVisible = !_isReadOnly;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Показывать ли кнопки +/-. 
        /// </summary>
        public bool ButtonsVisible
        {
            get => _buttonsVisible;
            set
            {
                if (_buttonsVisible != value)
                {
                    _buttonsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Инициализировать матрицу нулями.
        /// </summary>
        public void InitMatrix()
        {
            var newMatrix = new float[_matrixSize, _matrixSize];
            for (int i = 0; i < _matrixSize; i++)
            {
                for (int j = 0; j < _matrixSize; j++)
                {
                    newMatrix[i, j] = 1.0f;
                }
            }

            Matrix = newMatrix;
        }

        /// <summary>
        /// Вычисляем детерминант и обратную матрицу (используя ваши функции).
        /// </summary>
        public void CalcDeterminantAndInverse()
        {
            // Вызов MatrixFunctions:
            Determinant = MatrixFunctions.CalcDeterminant(_matrixSize, _matrix);

            if (!Determinant.HasValue || Determinant.Value == 0)
            {
                InverseMatrix = null;
            }
            else
            {
                InverseMatrix = MatrixFunctions.CalcInverseMatrix(_matrixSize,
                                                                  Determinant.Value,
                                                                  _matrix);
            }
        }

        // Команды для кнопок +/-:
        public void ForceMatrixChanged()
        {
            OnPropertyChanged(nameof(Matrix));
        }

        private RelayCommand _matrixSizeInc;
        public ICommand MatrixSizeInc
        {
            get
            {
                if (_matrixSizeInc == null)
                {
                    _matrixSizeInc = new RelayCommand(
                        () =>
                        {
                            if (MatrixSize < 10)
                            {
                                MatrixSize++;
                                InitMatrix();
                            }
                        },
                        () => !IsReadOnly // или убрать, если не надо блокировать
                    );
                }
                return _matrixSizeInc;
            }
        }

        private RelayCommand _matrixSizeDec;
        public ICommand MatrixSizeDec
        {
            get
            {
                if (_matrixSizeDec == null)
                {
                    _matrixSizeDec = new RelayCommand(
                        () =>
                        {
                            if (MatrixSize > 1)
                            {
                                MatrixSize--;
                                InitMatrix();
                            }
                        },
                        () => !IsReadOnly && MatrixSize > 1
                    );
                }
                return _matrixSizeDec;
            }
        }

        /// <summary>
        /// Вспомогательный метод для уведомления WPF об изменениях свойства.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
