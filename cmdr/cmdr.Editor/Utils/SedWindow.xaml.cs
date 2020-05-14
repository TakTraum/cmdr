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
using System.Windows.Shapes;

// https://stackoverflow.com/questions/2796470/wpf-create-a-dialog-prompt
namespace cmdr.Editor.Utils
{

    public enum SedOperation
    {
        regular,
        start,
        end,
    }

    public enum SedCapitalize
    {
        none,
        lowercase,
        uppercase,
        titlecase,
    }

    public class SedResult
    {
        public string _search;
        public string _replace;

        public SedOperation _oper;
        public SedCapitalize _capitalize;

        public bool do_trim = true;

        public SedResult(string search, string replace, SedOperation oper = SedOperation.regular, SedCapitalize capitalize = SedCapitalize.none)
        {
            _search = search;
            _replace = replace;
            _oper = oper;
            _capitalize = capitalize;
        }
    }

    public partial class SedWindow : Window
    {
        private InputType _inputType = InputType.Text;

        public SedWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(SedWindow_Loaded);

            /*
            txtQuestion.Text = question;
            Title = "title";
            txtResponse.Text = defaultValue;
            _inputType = inputType;*/
        }

        // call it with: string repeatPassword = PromptDialog.Prompt("Repeat password", "Password confirm", inputType: PromptDialog.InputType.Password)

        // SedResult ret = SedWindow.Prompt();
        void SedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SearchTB.Focus();
        }

        public static SedResult Prompt()
        {
            SedWindow inst = new SedWindow();
            inst.ShowDialog();
            if (inst.DialogResult == true) {

                SedOperation oper = SedOperation.regular;
                SedCapitalize cap = SedCapitalize.none;

                if (inst.typeNormal.IsChecked == true) {
                    oper = SedOperation.regular;
                }
                if (inst.typeStart.IsChecked == true) {
                    oper = SedOperation.start;
                }
                if (inst.typeEnd.IsChecked == true) {
                    oper = SedOperation.end;
                }


                if (inst.capitalizeNone.IsChecked == true) {
                    cap = SedCapitalize.none;
                }
                if (inst.capitalizeLowerCase.IsChecked == true) {
                    cap = SedCapitalize.lowercase;
                }
                if (inst.capitalizeUpperCase.IsChecked == true) {
                    cap = SedCapitalize.uppercase;
                }
                if (inst.capitalizeTitleCase.IsChecked == true) {
                    cap = SedCapitalize.titlecase;
                }

                var ret = new SedResult(inst.SearchTB.Text, inst.ReplaceTB.Text, oper, cap);
                return ret;
            }
            return null;
        }

  
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }


}
