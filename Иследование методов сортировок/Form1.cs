using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace Иследование_методов_сортировок
{
    public partial class Form1 : Form
    {
        public static Form1 Instance;
        public List<int> numbers = new List<int>();
        public static string asymptotics;
        private BenchmarkExample benchmark = new BenchmarkExample();

        public Form1()
        {
            InitializeComponent();
            // Инициализация ComboBox
            comboBoxAlgorithm.Items.AddRange(new[] { "Пузырьковая сортировка", "Сортировка вставками", "Сортировка выбором", "Быстрая сортировка", "Сортировка слиянием" });
            comboBoxSortMethod.Items.AddRange(new[] { "По возрастанию", "По убыванию" });
            Instance = this;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //генерация случайных чисел
            int count = int.Parse(textBox1.Text);
            int minRange = int.Parse(textBox2.Text);
            int maxRange = int.Parse(textBox7.Text);
            Random rand = new Random();
            numbers.Clear();
            ClearView(0);
            for (int i = 0; i < count; i++)
            {
                numbers.Add(rand.Next(minRange, maxRange));
                AddInView(0, numbers[i]);
            }
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var arrayNums = textBox6.Text.Replace(" ", "").Split(',').Select(t => int.Parse(t));
            foreach (var num in arrayNums)
            {
                numbers.Add(num);
                AddInView(0, num);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (numbers.Count <= 0) return;
            benchmark.Sort();

            textBox3.Text = benchmark.TotalTime.ToString();
            textBox4.Text = Math.Abs(benchmark.MemoryUsed).ToString();
            textBox5.Text = asymptotics;
        }

        public ComboBox GetComboBoxAlgorithm() => comboBoxAlgorithm;
        public ComboBox GetComboBoxSortMethod() => comboBoxSortMethod;
        public ListView GetListViewSortMethod() => listView2;
        public System.Windows.Forms.DataVisualization.Charting.Chart GetChartOutputArray() => chart2;

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    // Читаем содержимое файла в строку
                    string fileContent = File.ReadAllText(filePath);

                    var arrayNums = fileContent.Replace(" ", "").Split(',').Select(t => int.Parse(t));
                    foreach (var num in arrayNums)
                    {
                        numbers.Add(num);
                        AddInView(0, num);
                    }

                    MessageBox.Show(fileContent, "Было добавлено в массив из файла", MessageBoxButtons.OK);
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show($"Ошибка при чтении файла: {ioEx.Message}", "Ошибка", MessageBoxButtons.OK);
                }
            }
        }

        private void AddInView(int index, int value)
        {
            if (index == 0)
            {
                listView1.Items.Add(value.ToString());
                chart1.Series[0].Points.AddXY(chart1.Series[0].Points.Count, value);
            }
            else
            {
                listView1.Items.Add(value.ToString());
                chart2.Series[0].Points.AddXY(chart2.Series[0].Points.Count, value);
            }
        }

        private void ClearView(int index)
        {
            if (index == 0)
            {
                listView1.Items.Clear();
                chart1.Series[0].Points.Clear();
            }
            else
            {
                listView1.Items.Clear();
                chart2.Series[0].Points.Clear();
            }
        }
    }

    public class BenchmarkExample
    {
        public double TotalTime;
        public long MemoryUsed;

        public void Sort()
        {
            ComboBox comboBoxAlgorithm = Form1.Instance.GetComboBoxAlgorithm();
            ComboBox comboBoxSortMethod = Form1.Instance.GetComboBoxSortMethod();
            ListView listViewSortMethod = Form1.Instance.GetListViewSortMethod();
            var chart1 = Form1.Instance.GetChartOutputArray();
            List<int> numbers = Form1.Instance.numbers;
            bool ascending = comboBoxSortMethod.SelectedIndex == 0 ? true : false;

            var watcher =  Stopwatch.StartNew();
            long memoryBefore = GC.GetTotalMemory(true);
            switch (comboBoxAlgorithm.SelectedIndex)
            {
                case 0:
                    Form1.asymptotics = "n^2";
                    BubbleSort(ref numbers, ascending);
                    break;
                case 1:
                    Form1.asymptotics = "n^2";
                    InsertionSort(ref numbers, ascending);
                    break;
                case 2:
                    Form1.asymptotics = "n^2";
                    SelectionSort(ref numbers, ascending);
                    break;
                case 3:
                    Form1.asymptotics = "n log n";
                    QuickSort(ref numbers, 0, numbers.Count - 1, ascending);
                    break;
                case 4:
                    Form1.asymptotics = "n log n";
                    MergeSort(ref numbers, 0, numbers.Count - 1, ascending);
                    break;
            }
            watcher.Stop();
            long memoryAfter = GC.GetTotalMemory(true);
            TotalTime = watcher.ElapsedMilliseconds;
            MemoryUsed = memoryAfter - memoryBefore;

            listViewSortMethod.Items.Clear();
            chart1.Series[0].Points.Clear();
            for (int i = 0; i < numbers.Count; i++)
            {
                chart1.Series[0].Points.AddXY(i, numbers[i]);
                listViewSortMethod.Items.Add(numbers[i].ToString());
            }
        }

        // Big O - n^2
        private void BubbleSort(ref List<int> array, bool ascending = true)
        {
            int n = array.Count;
            bool swapped;

            for (int i = 0; i < n - 1; i++)
            {
                swapped = false;

                for (int j = 0; j < n - i - 1; j++)
                {
                    // Изменяем условие в зависимости от направления сортировки
                    if ((ascending && array[j] > array[j + 1]) || (!ascending && array[j] < array[j + 1]))
                    {
                        // Меняем элементы местами
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;

                        swapped = true;
                    }
                }

                // Если внутренний цикл не сделал ни одной перестановки, значит массив уже отсортирован
                if (!swapped)
                    break;
            }
        }

        // Big O - n^2
        private void InsertionSort(ref List<int> array, bool ascending = true)
        {
            int n = array.Count;

            for (int i = 1; i < n; i++)
            {
                int key = array[i];
                int j = i - 1;

                // Изменяем условие в зависимости от направления сортировки
                while (j >= 0 && ((ascending && array[j] > key) || (!ascending && array[j] < key)))
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = key;
            }
        }

        // Big O - n^2
        private void SelectionSort(ref List<int> array, bool ascending = true)
        {
            int n = array.Count;

            for (int i = 0; i < n - 1; i++)
            {
                // Предполагаем, что текущий элемент — минимальный (или максимальный)
                int index = i;

                for (int j = i + 1; j < n; j++)
                {
                    // Изменяем условие в зависимости от направления сортировки
                    if ((ascending && array[j] < array[index]) || (!ascending && array[j] > array[index]))
                    {
                        index = j;
                    }
                }

                // Меняем местами найденный элемент с первым элементом
                if (index != i)
                {
                    int temp = array[i];
                    array[i] = array[index];
                    array[index] = temp;
                }
            }
        }

        // Big O - n log n
        private void QuickSort(ref List<int> array, int low, int high, bool ascending = true)
        {
            if (low < high)
            {
                // Получаем индекс разделителя
                int pi = Partition(ref array, low, high, ascending);

                // Рекурсивно сортируем элементы до и после разделителя
                QuickSort(ref array, low, pi - 1, ascending);
                QuickSort(ref array, pi + 1, high, ascending);
            }
        }

        private int Partition(ref List<int> array, int low, int high, bool ascending)
        {
            // Выбираем последний элемент в качестве опорного
            int pivot = array[high];
            int i = (low - 1); // Индекс меньшего элемента

            for (int j = low; j < high; j++)
            {
                // Изменяем условие в зависимости от направления сортировки
                if ((ascending && array[j] <= pivot) || (!ascending && array[j] >= pivot))
                {
                    i++;

                    // Меняем местами элементы
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }

            // Меняем местами опорный элемент с элементом на позиции i + 1
            int temp1 = array[i + 1];
            array[i + 1] = array[high];
            array[high] = temp1;

            return i + 1;
        }

        // Big O - n log n
        private void MergeSort(ref List<int> array, int low, int high, bool ascending = true)
        {
            if (low < high)
            {
                int mid = (low + high) /  2; // Находим середину

                // Рекурсивно сортируем обе части
                MergeSort(ref array, low, mid, ascending);
                MergeSort(ref array, mid + 1, high, ascending);

                // Сливаем отсортированные части
                Merge(ref array, low, mid, high, ascending);
            }
        }

        private void Merge(ref List<int> array, int low, int mid, int high, bool ascending)
        {
            int leftSize = mid - low + 1;
            int rightSize = high - mid;

            int[] left = new int[leftSize];
            int[] right = new int[rightSize];

            // Копируем данные во временные массивы
            for (int i = 0; i < leftSize; i++)
                left[i] = array[low + i];
            for (int j = 0; j < rightSize; j++)
                right[j] = array[mid + 1 + j];

            int k = low; // Индекс для основного массива
            int iIndex = 0; // Индекс для левой части
            int jIndex = 0; // Индекс для правой части

            // Слияние временных массивов обратно в основной массив
            while (iIndex < leftSize && jIndex < rightSize)
            {
                if ((ascending && left[iIndex] <= right[jIndex]) || (!ascending && left[iIndex] >= right[jIndex]))
                {
                    array[k] = left[iIndex];
                    iIndex++;
                }
                else
                {
                    array[k] = right[jIndex];
                    jIndex++;
                }
                k++;
            }

            // Копируем оставшиеся элементы левой части, если есть
            while (iIndex < leftSize)
            {
                array[k] = left[iIndex];
                iIndex++;
                k++;
            }

            // Копируем оставшиеся элементы правой части, если есть
            while (jIndex < rightSize)
            {
                array[k] = right[jIndex];
                jIndex++;
                k++;
            }
        }
    }
}