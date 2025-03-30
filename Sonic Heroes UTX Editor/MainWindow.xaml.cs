using AquaModelLibrary.Data.Sega.SonicHeroes;
using Heroes_Text_Editor.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;

namespace Heroes_Text_Editor
{
    public partial class MainWindow : Window
    {
        private string fileName;
        private List<List<string>> data;
        private List<string> categoryLabels;
        private bool isBigEndian = false;
        private ICollectionView displayEntriesView;

        private int currentCollectionIndex = -1;
        private int currentEntryIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            PreferencesCheck();
            ClearUIData();
            ListBox_Collections.ItemsSource = null;
            ListBox_Entries.ItemsSource = null;
        }

        private void Button_OpenUTXFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog
            {
                Filter = ".utx files|*.utx"
            };
            if (dialog.ShowDialog() == false)
            {
                return;
            }
            ClearData();

            fileName = dialog.FileName;
            byte[] readFile = File.ReadAllBytes(dialog.FileName);
            (data, isBigEndian) = UTX.DumpUTX(readFile);

            categoryLabels = new List<string>();
            for(int i = 0; i < data.Count; i++)
            {
                categoryLabels.Add("Slot " + i + ":     " + data[i].Count + " entries");
            }

            ListBox_Collections.ItemsSource = categoryLabels;
        }
        private void Button_SaveUTXFile(object sender, RoutedEventArgs e)
        {
            if (CheckBox_ChooseWhereToSave.IsChecked == true)
            {
                var dialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog
                {
                    Filter = ".utx files|*.utx",
                    DefaultExt = ".utx",
                    AddExtension = true,
                };
                if (dialog.ShowDialog() == false)
                {
                    MessageBox.Show("Save cancelled.", "Cancelled");
                    return;
                }
                fileName = dialog.FileName;
            }
            try
            {
                var outfile = UTX.CompileUTX(data, isBigEndian);
                File.WriteAllBytes(fileName, outfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Exception Occurred");
            }
        }

        private void ClearData()
        {
            if (data != null)
            {
                data.Clear();
            }
            if (categoryLabels != null)
            {
                categoryLabels.Clear();
            }
            fileName = "";
            currentCollectionIndex = -1;
            ClearUIData();
            ListBox_Collections.ItemsSource = null;
            ListBox_Entries.ItemsSource = null;
        }

        private void ClearUIData()
        {
            currentEntryIndex = -1;
            TextBox_EditSubtitle.Clear();
        }

        private void ListBox_Collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearUIData();
            if (ListBox_Collections.SelectedIndex == -1)
            {
                ListBox_Entries.ItemsSource = null;
                return;
            }
            currentCollectionIndex = ListBox_Collections.SelectedIndex;
            ListBox_Entries.SelectedIndex = -1;
            displayEntriesView = CollectionViewSource.GetDefaultView(data[currentCollectionIndex]);

            ListBox_Entries.ItemsSource = displayEntriesView;
        }

        private void ListBox_Entries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock_CurrentEntry_Index.Text = "Index: None";
            if (ListBox_Entries.SelectedItem == null)
            {
                ClearUIData();
                return;
            }
            currentEntryIndex = ListBox_Entries.SelectedIndex;
            if (currentEntryIndex == -1)
            {
                ClearUIData();
                return;
            }
            TextBlock_CurrentEntry_Index.Text = "Index: " + currentEntryIndex.ToString();
            TextBox_EditSubtitle.Text = data[currentCollectionIndex][currentEntryIndex];
        }

        private void Button_SaveCurrentEntry_Click(object sender, RoutedEventArgs e)
        {
            var savedEntryIndex = currentEntryIndex;
            if (ListBox_Entries.SelectedItem == null)
                return;
            if (currentEntryIndex == -1)
            {
                MessageBox.Show("Error, subtitle not found, report this bug and what you did to cause it.", "Impossible Bug. If you see this screenshot it!");
                return;
            }
            var replacementText = TextBox_EditSubtitle.Text;
            replacementText = replacementText.Replace("\r\n", "\n");
            replacementText = replacementText.Replace("\0", "");
            replacementText += '\0';
            data[currentCollectionIndex][currentEntryIndex] = replacementText;
            displayEntriesView.Refresh();
            TextBox_EditSubtitle.Clear();
            ListBox_Entries.SelectedIndex = -1; // trigger deselect-reselect event
            ListBox_Entries.SelectedIndex = savedEntryIndex;
        }

        private void Button_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Interface by dreamsyntax\n" +
                ".utx reverse engineering done by Shadowth117\n" +
                "Program icon by RaphaelDrewBoltman\n\n" +
                "Uses modified version of DarkTheme by Otiel\n" +
                "Uses Ookii.Dialogs for dialogs\n\n" +
                "https://github.com/Heroes-Hacking-Central\n\nto check for updates for this software.", "About Sonic Heroes UTX Editor");
        }

        private void CheckBox_DarkMode_Checked(object sender, RoutedEventArgs e)
        {
            ThemeHelper.ApplySkin(Skin.Dark);
            SetGroupBoxBorder(0.1d);
            PreferencesSave();
        }

        private void CheckBox_DarkMode_Unchecked(object sender, RoutedEventArgs e)
        {
            ThemeHelper.ApplySkin(Skin.Light);
            SetGroupBoxBorder(1);
            PreferencesSave();
        }

        private void SetGroupBoxBorder(double multiplier)
        {
            double thickness = 1.0d * multiplier;
            var value = new Thickness(thickness, thickness, thickness, thickness);
            GroupBoxEntries.BorderThickness = value;
            GroupBoxFileControls.BorderThickness = value; ;
            GroupBoxCollections.BorderThickness = value;
            GroupBoxMisc.BorderThickness = value;
            GroupBoxCurrentEntryAttributes.BorderThickness = value;
        }

        private void PreferencesCheck()
        {
            string themeConfig = AppDomain.CurrentDomain.BaseDirectory + "/preferences.ini";
            if (File.Exists(themeConfig))
            {
                foreach (string i in File.ReadAllLines(themeConfig))
                {
                    if (i.StartsWith("Dark"))
                        CheckBox_DarkMode.IsChecked = true;
                }
            }
        }

        private void CoreWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CheckBox_MonoSpace_Checked(object sender, RoutedEventArgs e)
        {
            TextBox_EditSubtitle.FontFamily = new FontFamily("Courier New");
        }

        private void CheckBox_MonoSpace_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBox_EditSubtitle.FontFamily = new FontFamily("Segoe UI");
        }

        private void PreferencesSave()
        {
            string themeConfig = AppDomain.CurrentDomain.BaseDirectory + "/preferences.ini";
            string prefs = "";
            if (CheckBox_DarkMode.IsChecked ?? false)
            {
                prefs += "Dark\n";
            }
            else
            {
                prefs += "Light\n";
            }
            File.WriteAllText(themeConfig, prefs);
        }
    }
}
