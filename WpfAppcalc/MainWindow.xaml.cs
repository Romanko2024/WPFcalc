using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppcalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //стрінг через TextBox
        private string _currentInput = "0"; //поточне значення(що вводиться) за умовч 0
        private double _lastValue = 0; //останнє введене число
        private string _lastOperation = ""; //остання операція
        private Stack<ICommand> _undoStack = new Stack<ICommand>(); //стек команд для Undo
        private Stack<ICommand> _redoStack = new Stack<ICommand>(); //стек команд для Redo
        public MainWindow()
        {
            InitializeComponent();

        }
        private void Number_Click(object sender, RoutedEventArgs e) //обробка кнопок чисел
        {
            //визначає яку кнопку натиснув користувач. отримує її текст
            Button button = sender as Button;
            if (button == null) return;
            string number = button.Content.ToString();

            if (_currentInput == "0" && number != ".")
                _currentInput = number;
            else
                _currentInput += number;

            Display.Text = _currentInput;
        }
        private void Operation_Click(object sender, RoutedEventArgs e) //оброб. арифм. оп.
        {
            Button button = sender as Button;
            if (button == null) return;
            //чи можна конверт в дабл
            if (!double.TryParse(_currentInput, out _lastValue)) return;

            _lastOperation = button.Content.ToString();
            _currentInput = "0";

            Display.Text = _lastValue.ToString();
        }
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(_currentInput, out double secondValue)) return;

            double result = _lastValue;

            switch (_lastOperation)
            {
                case "+":
                    result += secondValue;
                    break;
                case "−":
                    result -= secondValue;
                    break;
                case "×":
                    result *= secondValue;
                    break;
                case "÷":
                    if (secondValue == 0)
                    {
                        MessageBox.Show("Ділення на нуль неможливе!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    result /= secondValue;
                    break;
            }
            _undoStack.Push(new CalculatorCommand(_lastValue, secondValue, _lastOperation));
            _currentInput = result.ToString();
            Display.Text = _currentInput;
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            //чи є елементи в стакі Undo
            if (_undoStack.Count > 0)
            {
                //витягуємо останню ~~карту~~ команду з стеку
                ICommand command = _undoStack.Pop();
                command.Unexecute();
                //додаємо команду до стеку Redo
                _redoStack.Push(command);
            }
        }
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (_redoStack.Count > 0)
            {
                ICommand command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //очищаємо поле вводу
            _currentInput = "0";
            Display.Text = _currentInput; //виводимо на екран
        }
        private void ToggleExtraFunctions_Click(object sender, RoutedEventArgs e)
        {
            //перемик. видимості панелі доп функцій
            if (ExtraFunctionsPanel.Visibility == Visibility.Collapsed)
            {
                ExtraFunctionsPanel.Visibility = Visibility.Visible; //показуємо панель
            }
            else
            {
                ExtraFunctionsPanel.Visibility = Visibility.Collapsed; //ховаємо панель
            }
        }
        private void Pi_Click(object sender, RoutedEventArgs e)
        {
            _currentInput = Math.PI.ToString();
            Display.Text = _currentInput;
        }
        private void Exp_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(_currentInput, out double value))
            {
                _currentInput = Math.Exp(value).ToString();
                Display.Text = _currentInput;
            }
        }
        private void Sqrt_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(_currentInput, out double value))
            {
                if (value >= 0)  //чи з від'ємного числа
                {
                    _currentInput = Math.Sqrt(value).ToString();
                    Display.Text = _currentInput;
                }
                else
                {
                    MessageBox.Show("Не існує квадратного кореня з від'ємного числа", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
    public interface ICommand
    {
        void Execute();   //виконування !!прописати оба методи!3
        void Unexecute(); //скасування
    }

    //реалізує команду для калькулятора
    public class CalculatorCommand : ICommand
    {
        private double _operand1;  //перше число
        private double _operand2;  //друге число 
        private string _operation; // + − × ÷
        private double _result;

        public CalculatorCommand(double operand1, double operand2, string operation)
        {
            //збереж. операції/операнди
            _operand1 = operand1;
            _operand2 = operand2;
            _operation = operation;
            _result = Calculate();  //обчислення результату + збереження
        }
        private double Calculate()
        {
            return _operation switch //який тип операції
            {
                "+" => _operand1 + _operand2,
                "−" => _operand1 - _operand2,
                "×" => _operand1 * _operand2,
                "÷" => (_operand2 == 0) ? 0 : _operand1 / _operand2,
                _ => 0  //операція невідома - повертаємо 0
            };
        }

        // для реду
        public void Execute()
        {
            //повторне виконання обчислення 
        }

        //анду
        public void Unexecute()
        {
            //зробити зворотну операцію 
            //якщо була операція "+" треба зробити "-"
        }
    }
}
