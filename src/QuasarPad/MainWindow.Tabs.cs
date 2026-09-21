using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using QuasarPad.Models;

namespace QuasarPad
{
    public partial class MainWindow
    {
        private readonly ObservableCollection<EditorTab> _tabs = new();
        private int _tabSeq = 1;
        private bool _suppressTabSwitch;
        private bool _suppressTextChanged;
        private int _lastTabIndex;

        private EditorTab? CurrentTab =>
            DocTabs.SelectedIndex >= 0 && DocTabs.SelectedIndex < _tabs.Count
                ? _tabs[DocTabs.SelectedIndex]
                : null;

        private void InitTabs()
        {
            AddTabInternal("Untitled", null, "");
            RefreshTabHeaders();
            _suppressTabSwitch = true;
            DocTabs.SelectedIndex = 0;
            _suppressTabSwitch = false;
            _lastTabIndex = 0;
            LoadTabIntoEditor(0);
        }

        private void AddTabInternal(string title, string? path, string content)
        {
            _tabs.Add(new EditorTab
            {
                Title = title,
                FilePath = path,
                Content = content ?? "",
                IsModified = false
            });
            DocTabs.Items.Add(new TabItem { Header = title });
        }

        private void RefreshTabHeaders()
        {
            for (int i = 0; i < _tabs.Count && i < DocTabs.Items.Count; i++)
            {
                if (DocTabs.Items[i] is TabItem ti)
                    ti.Header = _tabs[i].DisplayTitle;
            }
        }

        private void SaveEditorToCurrentTab()
        {
            if (_lastTabIndex >= 0 && _lastTabIndex < _tabs.Count)
            {
                _tabs[_lastTabIndex].Content = MainEditor.Text;
                _tabs[_lastTabIndex].IsModified = _isModified;
                _tabs[_lastTabIndex].FilePath = _currentFilePath;
            }
        }

        private void LoadTabIntoEditor(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;
            _suppressTextChanged = true;
            MainEditor.Text = _tabs[index].Content;
            _suppressTextChanged = false;
            _currentFilePath = _tabs[index].FilePath;
            _isModified = _tabs[index].IsModified;
            _lastTabIndex = index;
            UpdateTitle();
            UpdateStatusBar();
        }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            SaveEditorToCurrentTab();
            string title = _tabSeq <= 1 ? "Untitled" : $"Untitled {_tabSeq}";
            _tabSeq++;
            AddTabInternal(title, null, "");
            RefreshTabHeaders();
            _suppressTabSwitch = true;
            DocTabs.SelectedIndex = _tabs.Count - 1;
            _suppressTabSwitch = false;
            LoadTabIntoEditor(DocTabs.SelectedIndex);
            MainEditor.Focus();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _suppressTextChanged = true;
            MainEditor.Text = "";
            _suppressTextChanged = false;
            var tab = CurrentTab;
            if (tab != null)
            {
                tab.Content = "";
                tab.IsModified = true;
            }
            _isModified = true;
            RefreshTabHeaders();
            UpdateTitle();
            UpdateStatusBar();
            MainEditor.Focus();
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (_tabs.Count <= 1)
            {
                Clear_Click(sender, e);
                var t = CurrentTab;
                if (t != null)
                {
                    t.Title = "Untitled";
                    t.FilePath = null;
                    t.IsModified = false;
                    t.Content = "";
                }
                _currentFilePath = null;
                _isModified = false;
                _suppressTextChanged = true;
                MainEditor.Text = "";
                _suppressTextChanged = false;
                RefreshTabHeaders();
                UpdateTitle();
                return;
            }

            int idx = DocTabs.SelectedIndex;
            if (idx < 0) return;

            SaveEditorToCurrentTab();
            var tab = _tabs[idx];
            if (tab.IsModified)
            {
                var r = MessageBox.Show("Save changes before closing tab?", "Close Tab",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (r == MessageBoxResult.Cancel) return;
                if (r == MessageBoxResult.Yes)
                {
                    Save_Click(sender, e);
                    if (_tabs[idx].IsModified) return;
                }
            }

            _tabs.RemoveAt(idx);
            DocTabs.Items.RemoveAt(idx);
            int newIdx = Math.Min(idx, _tabs.Count - 1);
            _suppressTabSwitch = true;
            DocTabs.SelectedIndex = newIdx;
            _suppressTabSwitch = false;
            LoadTabIntoEditor(newIdx);
        }

        private void DocTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressTabSwitch || _tabs.Count == 0) return;
            if (DocTabs.SelectedIndex < 0) return;

            if (_lastTabIndex >= 0 && _lastTabIndex < _tabs.Count && _lastTabIndex != DocTabs.SelectedIndex)
            {
                _tabs[_lastTabIndex].Content = MainEditor.Text;
                _tabs[_lastTabIndex].IsModified = _isModified;
                _tabs[_lastTabIndex].FilePath = _currentFilePath;
            }

            LoadTabIntoEditor(DocTabs.SelectedIndex);
        }

        private void SyncTabAfterSave()
        {
            var tab = CurrentTab;
            if (tab == null) return;
            tab.Content = MainEditor.Text;
            tab.IsModified = false;
            tab.FilePath = _currentFilePath;
            if (!string.IsNullOrEmpty(_currentFilePath))
                tab.Title = Path.GetFileName(_currentFilePath);
            _isModified = false;
            RefreshTabHeaders();
            UpdateTitle();
        }
    }
}
