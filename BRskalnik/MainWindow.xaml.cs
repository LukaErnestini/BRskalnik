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
using WinForms = System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace BRskalnik
{

    #region HeaderToImageConverter

    [ValueConversion(typeof(string), typeof(bool))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance =
            new HeaderToImageConverter();

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value as string).Contains(@"\"))
            {
                Uri uri = new Uri
                ("pack://application:,,,/Images/diskdrive.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else
            {
                Uri uri = new Uri("pack://application:,,,/Images/folder.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }

    #endregion // HeaderToImageConverter

public class Direktorij
    {
        public string Name { get; set; }
        public string DateModified { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
        public string ImagePath { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Direktorij> items = new List<Direktorij>();
        private object dummyNode = null;
        private bool bubblingBulkwark;


        public MainWindow()
        {
            InitializeComponent();

            //VAJA 2.3
            ////items.Add(new Direktorij { Name = "Program Files", DateModified = "12.1.2018", Size = 1244551, Type = "folder" });
            ////items.Add(new Direktorij { Name = "Program Data", DateModified = "12.1.2018", Size = 14551, Type = "folder" });
            ////items.Add(new Direktorij { Name = "Users", DateModified = "12.1.2018", Size = 1241, Type = "folder" });
            ////ListView1.ItemsSource = items;

            ListView1.Items.Add(new Direktorij { Name = "Program Files", DateModified = "12.1.2018", Size = 1244551, Type = "folder", ImagePath = "folder-symbol.png" });
            ListView1.Items.Add(new Direktorij { Name = "Program Data", DateModified = "12.1.2018", Size = 14551, Type = "folder", ImagePath = "folder-symbol.png" });
            ListView1.Items.Add(new Direktorij { Name = "Users", DateModified = "12.1.2018", Size = 1241, Type = "folder", ImagePath = "folder-symbol.png" });

            //ListDirectory(TreeView1, "C:\\");
            
            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                item.MouseLeftButtonUp += new MouseButtonEventHandler(folder_Clicked);
                item.PreviewMouseDown += new MouseButtonEventHandler(folder_Clicked_Down);
                TreeView1.Items.Add(item);
            }

        }

        private void folder_Clicked_Down(object sender, MouseButtonEventArgs e)
        {
            bubblingBulkwark = false;
        }

        private void folder_Clicked(object sender, RoutedEventArgs e)
        {
            // Make sure the event has never been handled first
            if (bubblingBulkwark)
                return;
            // Raise bulkwark
            bubblingBulkwark = true;

            TreeViewItem item = (TreeViewItem)sender;
            ListView1.Items.Clear();
            try
            {
                foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                {
                    ListView1.Items.Add(new Direktorij { Name = s.Substring(s.LastIndexOf("\\") + 1), ImagePath = "Images\\folder.png" });
                }

                foreach (string s in Directory.GetFiles(item.Tag.ToString()))
                {
                    ListView1.Items.Add(new Direktorij { Name = s.Substring(s.LastIndexOf("\\") + 1), ImagePath = "Images\\google-drive-file.png", Size = new FileInfo(s).Length, DateModified = new FileInfo(s).LastWriteTime.ToShortDateString(), Type = s.Substring(s.IndexOf('.') + 1) });
                }
            }
            catch (Exception) { }

        }

            private void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        subitem.MouseLeftButtonUp += new MouseButtonEventHandler(folder_Clicked);
                        subitem.MouseLeftButtonDown += new MouseButtonEventHandler(folder_Clicked_Down);
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //VAJA 2.3
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject originalSource = (DependencyObject)e.OriginalSource;
            while ((originalSource != null) && !(originalSource is ListViewItem))
            {
                originalSource = VisualTreeHelper.GetParent(originalSource);
            }
            //if it didn’t find a ListViewItem anywhere in the hierarch, it’s because the user
            //didn’t click on one. Therefore, if the variable isn’t null, run the code
            if (originalSource != null)
            {
                Direktorij item = (Direktorij)ListView1.ItemContainerGenerator.ItemFromContainer(originalSource);
                MessageBox.Show(item.Name);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ListView1.SelectedItems.Count != 0)
                ListView1.Items.Remove(ListView1.SelectedItems[0]);
            else
                MessageBox.Show("Ni izbran noben direktorij ali datoteka.");
        }
    }
}
