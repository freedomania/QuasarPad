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

        private EditorTab? CurrentTab =>
            DocTabs.SelectedIndex >= 0 && DocTabs.SelectedIndex < _tabs.Count
                ? _tabs[DocTabs.SelectedIndex]
                : null;

        private void InitTabs()
        {
            AddTabInternal("Untitled", null, "");
            RefreshTabHeaders();
            DocTabs.SelectedIndex = 0;
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
            var item = new TabItem { Header = title };
            DocTabs.Items.Add(item);
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
            var tab = CurrentTab;
            if (tab == null) return;
            tab.Content = MainEditor.Text;
        }

        private void LoadTabIntoEditor(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;
            _suppressTextChanged = true;
            MainEditor.Text = _tabs[index].Content;
            _suppressTextChanged = false;
            _currentFilePath = _tabs[index].FilePath;
            _isModified = _tabs[index].IsModified;
            UpdateTitle();
            UpdateStatusBar();
        }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            SaveEditorToCurrentTab();
            string title = _tabSeq == 1 ? "Untitled" : $"Untitled {_tabSeq}";
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
            MainEditor.Text = "";
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
                // last tab: just clear
                Clear_Click(sender, e);
                var t = CurrentTab;
                if (t != null)
                {
                    t.Title = "Untitled";
                    t.FilePath = null;
                    t.IsModified = false;
                    _currentFilePath = null;
                    _isModified = false;
                    RefreshTabHeaders();
                    UpdateTitle();
                }
                return;
            }

            int idx = DocTabs.SelectedIndex;
            if (idx < 0) return;

            var tab = _tabs[idx];
            if (tab.IsModified)
            {
                var r = MessageBox.Show("Save changes before closing tab?", "Close Tab",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (r == MessageBoxResult.Cancel) return;
                if (r == MessageBoxResult.Yes)
                {
                    SaveEditorToCurrentTab();
                    Save_Click(sender, e);
                    if (CurrentTab?.IsModified == true) return;
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
            if (_suppressTabSwitch) return;
            if (e.RemovedItems.Count > 0 && DocTabs.SelectedIndex >= 0)
            {
                // save previous - find old index roughly via content already in editor
                // When selection changes, save current editor to the tab that was selected before
            }
            // Simpler: before switch, always save editor into whatever tab index was current
            // SelectionChanged fires after change, so we need to save on switching using previous index
        }

        /// <summary>Call before changing selected tab index.</summary>
        private void SwitchToTab(int newIndex)
        {
            if (newIndex < 0 || newIndex >= _tabs.Count) return;
            SaveEditorToCurrentTab();
            _suppressTabSwitch = true;
            DocTabs.SelectedIndex = newIndex;
            _suppressTabSwitch = false;
            LoadTabIntoEditor(newIndex);
        }

        private void OnTabSelectionChanged()
        {
            // Used from SelectionChanged after user clicks a tab
        }

        internal void DocTabs_SelectionChanged_Handler(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressTabSwitch || _tabs.Count == 0) return;

            // When user clicks another tab, editor still has OLD tab text until we load.
            // We need previous index. Use: save to all tabs matching is hard.
            // Approach: store _lastTabIndex
        }

        private int _lastTabIndex;

        private void DocTabs_SelectionChanged_Impl(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressTabSwitch || _tabs.Count == 0) return;

            if (_lastTabIndex >= 0 && _lastTabIndex < _tabs.Count)
            {
                _tabs[_lastTabIndex].Content = MainEditor.Text;
            }

            int idx = DocTabs.SelectedIndex;
            if (idx >= 0 && idx < _tabs.Count)
            {
                LoadTabIntoEditor(idx);
                _lastTabIndex = idx;
            }
        }
    }
}
