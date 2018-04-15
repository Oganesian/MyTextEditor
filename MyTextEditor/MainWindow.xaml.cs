using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyTextEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentFile;
        public MainWindow()
        {
            InitializeComponent();

            var range = new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Redo();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Copy();
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Cut();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Paste();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox senderObject = (TextBox)sender;
            if (senderObject.Text.Equals("Search..."))
            {
                senderObject.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox senderObject = (TextBox)sender;
            if (senderObject.Text.Equals(""))
            {
                senderObject.Text = "Search...";
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd);
            string source = range.Text;
            string pattern = SearchQuery.Text;
            List<TextRange> list = new List<TextRange>();
            range.ApplyPropertyValue(TextElement.BackgroundProperty, null);

            foreach (Paragraph paragraph in textEditor.Document.Blocks)
            {
                foreach (Inline inline in paragraph.Inlines)
                {
                    TextPointer start = inline.ContentStart;
                    TextPointer end = inline.ContentEnd;
                    TextRange r = new TextRange(start, end);
                    string src = r.Text;
                    int startIndex = 0;
                    int index = src.IndexOf(pattern, startIndex);

                    while (index != -1 && !src.Equals("") && !pattern.Equals(""))
                    {
                        TextRange tmp = new TextRange(start.GetPositionAtOffset(index),
                            start.GetPositionAtOffset(index + pattern.Length));
                        startIndex = index + pattern.Length;
                        index = src.IndexOf(pattern, startIndex);
                        list.Add(tmp);
                    }
                }
            }
            range.ApplyPropertyValue(TextElement.BackgroundProperty, null);

            foreach (TextRange textRange in list)
            {
                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, "#66FFFF00");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SearchGrid.Visibility = Visibility.Collapsed;
            textEditor.Margin = new Thickness(0, 20, 0, 0);
            TextRange range = new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd);
            range.ApplyPropertyValue(TextElement.BackgroundProperty, null);
        }

        private void MenuItem_Find_Click(object sender, RoutedEventArgs e)
        {
            SearchGrid.Visibility = Visibility.Visible;
            textEditor.Margin = new Thickness(0, 20, 0, 40);
            SearchQuery.Focus();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save the open file?", "",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                Save_Click(sender, e);
                textEditor.Document.Blocks.Clear();
            }
            else if(result == MessageBoxResult.No)
            {
                textEditor.Document.Blocks.Clear();
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            New_Click(sender, e);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                currentFile = openFileDialog.FileName;
                textEditor.Document.Blocks.Clear();
                textEditor.Document.Blocks.Add(new Paragraph(new Run(File.ReadAllText(openFileDialog.FileName))));
            }
               
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (currentFile == "" || currentFile == null)
            {
                SaveAs_Click(sender, e);
            }
            else
            {
                File.WriteAllText(currentFile,
                    new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd).Text);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName,
                    new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd).Text);
        }
    }
}