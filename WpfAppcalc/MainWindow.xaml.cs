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
    }
}
