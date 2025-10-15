using System;
using System.Windows;
using System.Windows.Controls;

namespace CALCULATRICE
{
    public partial class MainWindow : Window
    {
        private string currentInput = "";
        private string lastOperator = "";
        private double lastValue = 0;
        private bool newNumber = true;
        private double memoryValue = 0; //  mémoire ajoutée

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BTN_Number_Click(object sender, RoutedEventArgs e)
        {
            string number = (sender as Button)?.Content?.ToString() ?? "";
            if (newNumber)
            {
                currentInput = number;
                newNumber = false;
            }
            else
            {
                currentInput += number;
            }
            TB_Display.Text = currentInput;
        }

        private void BTN_Operator_Click(object sender, RoutedEventArgs e)
        {
            string op = (sender as Button)?.Content?.ToString() ?? "";

            if (string.IsNullOrEmpty(currentInput) && string.IsNullOrEmpty(lastOperator) && op != "-")
            {
                MessageBox.Show("Erreur : impossible de commencer par un opérateur sauf '-'.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(currentInput) && op == "-")
            {
                currentInput = "-";
                TB_Display.Text = currentInput;
                newNumber = false;
                return;
            }

            if (!newNumber)
            {
                Calculate();
            }

            lastOperator = op;
            newNumber = true;
        }

        private void BTN_Racine_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentInput))
            {
                MessageBox.Show("Erreur : impossible de commencer par la racine.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (double.TryParse(currentInput ?? "0", out double value))
            {
                if (value < 0)
                {
                    MessageBox.Show("Erreur : racine carrée d'un nombre négatif.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                lastValue = Math.Sqrt(value);
                currentInput = lastValue.ToString();
                TB_Display.Text = currentInput;
                newNumber = true;
            }
        }

        private void BTN_CLR_Click(object sender, RoutedEventArgs e)
        {
            currentInput = "";
            lastOperator = "";
            lastValue = 0;
            TB_Display.Text = "";
            newNumber = true;
        }

        private void BTN_Equal_Click(object sender, RoutedEventArgs e)
        {
            Calculate();
            lastOperator = "";
            newNumber = true;
        }

        private void BTN_Percentage_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(currentInput ?? "0", out double value))
            {
                value /= 100;
                currentInput = value.ToString();
                TB_Display.Text = currentInput;
                newNumber = true;
            }
        }

        private void BTN_Memory_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(TB_Display.Text ?? "0", out double val))
            {
                memoryValue = val;               //  enregistre la valeur
                currentInput = "";               //  vide l'écran
                TB_Display.Text = "";            //  met le TextBox à zéro
                newNumber = true;                //  prêt pour un nouveau nombre
                MessageBox.Show($"Valeur {val} enregistrée en mémoire.", "Mémoire", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void BTN_PasteMemory_Click(object sender, RoutedEventArgs e)
        {
            currentInput = memoryValue.ToString();
            TB_Display.Text = currentInput;
            newNumber = false;
        }

        private void BTN_Puissance_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(currentInput ?? "0", out double baseVal))
            {
                InputBox inputBox = new InputBox("Entrez la puissance :");
                if (inputBox.ShowDialog() == true)
                {
                    if (double.TryParse(inputBox.Result, out double power))
                    {
                        lastValue = Math.Pow(baseVal, power);
                        currentInput = lastValue.ToString();
                        TB_Display.Text = currentInput;
                        newNumber = true;
                    }
                    else
                    {
                        MessageBox.Show("Puissance invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Calculate()
        {
            if (double.TryParse(currentInput ?? "0", out double currentValue))
            {
                try
                {
                    switch (lastOperator)
                    {
                        case "+": lastValue += currentValue; break;
                        case "-": lastValue -= currentValue; break;
                        case "*": lastValue *= currentValue; break;
                        case "/":
                            if (currentValue == 0)
                            {
                                MessageBox.Show("Erreur : division par zéro.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            lastValue /= currentValue;
                            break;
                        case "%": lastValue = lastValue * currentValue / 100; break;
                        default: lastValue = currentValue; break;
                    }

                    if (Math.Abs(lastValue) > double.MaxValue)
                    {
                        MessageBox.Show("Erreur : dépassement numérique.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        lastValue = 0;
                        currentInput = "";
                        TB_Display.Text = "";
                        return;
                    }

                    currentInput = lastValue.ToString();
                    TB_Display.Text = currentInput;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    //  Petite fenêtre d’entrée pour la puissance
    public class InputBox : Window
    {
        public string Result { get; private set; }
        private TextBox input;

        public InputBox(string message)
        {
            Title = "Entrée requise";
            Width = 300;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };
            panel.Children.Add(new TextBlock { Text = message, Margin = new Thickness(0, 0, 0, 10) });

            input = new TextBox();
            panel.Children.Add(input);

            Button okButton = new Button { Content = "OK", Width = 60, Margin = new Thickness(0, 10, 0, 0) };
            okButton.Click += (s, e) => { Result = input.Text; DialogResult = true; Close(); };
            panel.Children.Add(okButton);

            Content = panel;
        }
    }
}
