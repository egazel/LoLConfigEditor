using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using Newtonsoft.Json;
using static LeagueOfLegendsConfigEditor.CFGHelper;
using static LeagueOfLegendsConfigEditor.JsonParserHelper;
using UserControl = System.Windows.Controls.UserControl;
using CheckBox = System.Windows.Controls.CheckBox;
using TextBox = System.Windows.Controls.TextBox;
using Newtonsoft.Json.Linq;

namespace LeagueOfLegendsConfigEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        internal static string cfgFolderPth = @"C:\Riot Games\League of Legends\Config\";

        internal void GenerateGameCfgUI()
        {
            OptionsStackPanel.Children.Clear();
            foreach (DictionaryEntry de in gameCfgBreakout)
            {
                string keyStr = de.Key.ToString();
                string valStr = de.Value.ToString();
                if (keyStr != null)
                {
                    Grid grid = new Grid();
                    if (keyStr.Contains('[')) // If its a section key
                    {
                        System.Windows.Controls.Label lb = new System.Windows.Controls.Label();
                        lb.Content = keyStr;
                        lb.FontSize = 30;
                        lb.Margin = new Thickness(25, 0, 25, 10);
                        lb.Padding = new Thickness(0, 0, 0, 0);

                        // Add the label to a border to customize visual
                        Border bd = new Border();
                        bd.CornerRadius = new CornerRadius(5);
                        bd.Background = new SolidColorBrush(Colors.Lavender);
                        bd.Child = lb;

                        grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        grid.Children.Add(bd);

                        UserControl u1 = new UserControl();
                        u1.Content = grid;
                        OptionsStackPanel.Children.Add(u1);
                    }
                    else // If it's editable content
                    {
                        ColumnDefinition col0 = new ColumnDefinition();
                        ColumnDefinition col1 = new ColumnDefinition();
                        col0.Width = new GridLength(300);

                        grid.ColumnDefinitions.Add(col0);
                        grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        grid.ColumnDefinitions.Add(col1);
                        grid.Margin = new Thickness(20, 0, 20, 0);

                        System.Windows.Controls.Label lb = new System.Windows.Controls.Label();
                        lb.Content = keyStr;
                        lb.FontSize = 13;
                        lb.Foreground = new SolidColorBrush(Colors.MidnightBlue);
                        lb.VerticalAlignment = VerticalAlignment.Center;
                        lb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        Grid.SetColumn(lb, 0);
                        grid.Children.Add(lb);

                        // If it's a 0/1 (will be represented by a checkbox except special cases)
                        if ((valStr == "0" || valStr == "1") && !keyStr.Contains("Speed") && !keyStr.Contains("Quality"))
                        {
                            System.Windows.Controls.CheckBox cb = new CheckBox();
                            cb.VerticalAlignment = VerticalAlignment.Center;
                            cb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                            cb.IsChecked = (Convert.ToInt32(valStr) == 1);
                            cb.Name = keyStr.Replace(" ", "_");
                            cb.Unchecked += new RoutedEventHandler(cb_checkStateChanged);
                            cb.Checked += new RoutedEventHandler(cb_checkStateChanged);
                            Grid.SetColumn(cb, 1);
                            grid.Children.Add(cb);
                        }
                        else // If it's an open field
                        {
                            TextBox tb = new TextBox();
                            tb.VerticalAlignment = VerticalAlignment.Center;
                            tb.TextAlignment = TextAlignment.Right;
                            tb.Text = valStr;
                            tb.Width = 85;
                            // Replace space by underscore because char not allowed in the name
                            tb.Name = keyStr.Replace(" ", "_");

                            // Callback when user modify to save value
                            tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
                            Grid.SetColumn(tb, 1);
                            grid.Children.Add(tb);
                        }
                        UserControl u1 = new UserControl();
                        u1.Content = grid;
                        OptionsStackPanel.Children.Add(u1);
                    }
                }
            }
        }

        private void scrollBarLineUp(object sender, RoutedEventArgs e)
        {
            scrollBar.LineUp();
        }

        private void scrollBarLineDown(object sender, RoutedEventArgs e)
        {
            scrollBar.LineDown();
        }

        private void tb_TextChanged(object? sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if ((gameCfgBreakout[tb.Name.Replace("_", " ")]).ToString() != tb.Text)
            {
                // if value changed, feedback via colour changing
                tb.Background = new SolidColorBrush(Colors.Goldenrod);
            }
            // update modified value in the dict with cleaned key
            gameCfgBreakout[(tb.Name).Replace("_", " ")] = tb.Text;
        }

        private void cb_checkStateChanged(object? sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int cbVal = Convert.ToInt32(cb.IsChecked);
            if ((gameCfgBreakout[cb.Name.Replace("_", " ")]).ToString() != cbVal.ToString())
            {
                // if value changed, feedback via colour changing
                cb.Background = new SolidColorBrush(Colors.Goldenrod);
            }
            // update modified value in the dict with cleaned key
            gameCfgBreakout[(cb.Name).Replace("_", " ")] = cbVal;
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            SaveGameCfg();
            GenerateGameCfgUI();
        }

        private void btnRevertChanges_Click(object sender, RoutedEventArgs e)
        {
            ParseGameCfgToDict();
            GenerateGameCfgUI();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameCfgTab.IsSelected)
            {
                if (gameCfgBreakout.Count == 0)
                {
                    ParseGameCfgToDict();
                    GenerateGameCfgUI();
                }
            }
            else
            {
                // TODO BIND DATAS via template to correctly edit/save
                CrawlJson(TreeView1);
            }
        }
    }
}