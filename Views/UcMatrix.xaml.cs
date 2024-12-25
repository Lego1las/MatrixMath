using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using MatrixMath.ViewModels;

namespace MatrixMath.Views
{
    public partial class UcMatrix : UserControl
    {
        public UcMatrix()
        {
            InitializeComponent();
            // Следим, когда DataContext поменяется (когда привяжут MatrixViewModel).
            DataContext = new MatrixViewModel();
            DataContextChanged += UcMatrix_DataContextChanged;
        }
        private void UcMatrix_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MatrixViewModel oldVm)
                oldVm.PropertyChanged -= OnMatrixViewModelPropertyChanged;

            if (e.NewValue is MatrixViewModel newVm)
            {
                // Подписываемся на PropertyChanged, 
                // чтобы знать, когда менять размер матрицы или её содержимое
                newVm.PropertyChanged += OnMatrixViewModelPropertyChanged;
                RedrawMatrix(newVm);
            }
        }

        private void OnMatrixViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is MatrixViewModel vm)
            {
                // Если изменились MatrixSize или Matrix - пересоздаём ячейки
                if (e.PropertyName == nameof(MatrixViewModel.MatrixSize) ||
                    e.PropertyName == nameof(MatrixViewModel.Matrix))
                {
                    RedrawMatrix(vm);
                }
            }
        }

        /// <summary>
        /// Полная перерисовка таблицы TextBox в GrdMatrix
        /// </summary>
        private void RedrawMatrix(MatrixViewModel vm)
        {
            if (vm.Matrix == null) return;
            // Сначала очищаем всё
            GrdMatrix.RowDefinitions.Clear();
            GrdMatrix.ColumnDefinitions.Clear();
            GrdMatrix.Children.Clear();

            int size = vm.MatrixSize;

            // Создаем нужное число строк и столбцов
            for (int i = 0; i < size; i++)
            {
                GrdMatrix.RowDefinitions.Add(new RowDefinition());
                GrdMatrix.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Генерируем каждую ячейку
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    int currentRow = row;
                    int currentCol = col;
                    // Сам TextBox
                    var tbx = new TextBox
                    {
                        IsReadOnly = vm.IsReadOnly,
                        Text = vm.Matrix[currentRow, currentCol].ToString(),
                        Width = 40,
                        MinHeight = 25,
                        FontWeight = FontWeights.Bold,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        BorderThickness = new Thickness(0),
                        Background = Brushes.Transparent
                    };
                    // Обработка ввода
                    tbx.KeyDown += TbxKeyDown;
                    tbx.TextChanged += (s, e) =>
                    {
                        // Запомним исходную позицию курсора (если захотим восстанавливать)
                        int oldCaret = tbx.CaretIndex;

                        // Текст, который сейчас в TextBox
                        string original = tbx.Text;

                        // 1) Сканируем, чтобы оставить только цифры и один минус
                        bool hasMinus = false;
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();

                        foreach (char c in original)
                        {
                            if (char.IsDigit(c))
                            {
                                sb.Append(c);
                            }
                            else if (c == '-')
                            {
                                // Если находим '-', просто запомним флажок
                                hasMinus = true;
                            }
                            // Прочие символы (буквы, пробел, . , и т.д.) игнорируем
                        }

                        // 2) Если в тексте был минус — вставим его один раз в начало
                        //    (другие минусы, стоящие дальше, мы уже проигнорировали)
                        if (hasMinus)
                        {
                            sb.Insert(0, '-');
                        }

                        // 3) Удаляем ведущие нули
                        string filtered = RemoveLeadingZeros(sb.ToString());

                        // 4) Если итоговая строка изменилась, обновим TextBox
                        if (filtered != original)
                        {
                            tbx.Text = filtered;
                            vm.Matrix[currentRow, currentCol] = float.Parse(tbx.Text);
                            vm.ForceMatrixChanged();
                        }
                    };

                    // Вокруг TextBox делаем Border c нужной толщиной и цветом
                    var brdStyle = new Style();
                    brdStyle.Setters.Add(new Setter
                    {
                        Property = Border.BorderThicknessProperty,
                        // левая граница = 0, если col == 0, иначе 0.5
                        // верхняя граница = 0, если row == 0, иначе 0.5
                        Value = new Thickness(
                            col == 0 ? 0 : 0.5,
                            row == 0 ? 0 : 0.5,
                            0,
                            0)
                    });
                    brdStyle.Setters.Add(new Setter
                    {
                        Property = Border.BorderBrushProperty,
                        Value = new SolidColorBrush(Colors.DeepSkyBlue)
                    });

                    // Готовая ячейка
                    var brd = new Border
                    {
                        Child = tbx,
                        Style = brdStyle
                    };

                    // Добавляем в Grid
                    GrdMatrix.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, col);
                }
            }
        }

        private string RemoveLeadingZeros(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text; // вернуть как есть

            bool negative = false;
            if (text[0] == '-')
            {
                negative = true;
                text = text.Substring(1); // отбросим минус
            }

            // Удаляем ведущие '0'
            int i = 0;
            while (i < text.Length && text[i] == '0')
            {
                i++;
            }
            text = text.Substring(i);

            // Если все символы были нулями — теперь строка пустая
            if (text == "")
            {
                // Можем выбрать логику:
                // 1) Всегда "0" (без знака)
                // 2) Или "-0", если negative == true
                //    (Но обычно «-0» не особо практично, так что решайте сами)

                // Вариант 1 (без знака):
                text = "0";

                // Вариант 2 (если нужно «-0»):
                // if (negative) text = "-0"; else text = "0";
            }

            // Возвращаем минус, если есть смысл (и хотим поддержать «-число»)
            else if (negative)
            {
                text = "-" + text;
            }

            return text;
        }

        /// <summary>
        /// Запретить пробел, как в исходном коде (опционально)
        /// </summary>
        /// 
        private static void TbxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
