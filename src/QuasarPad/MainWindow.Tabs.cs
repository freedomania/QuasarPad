using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            int index = _tabs.Count;
            _tabs.Add(new EditorTab
            {
                Title = title,
                FilePath = path,
                Content = content ?? "",
                IsModified = false
            });
            DocTabs.Items.Add(new TabItem { Header = CreateTabHeader(title, index) });
        }

        private FrameworkElement CreateTabHeader(string title, int tabIndex)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var label = new TextBlock
            {
                Text = title,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0)
            };

            var closeBtn = new Button
            {
                Content = "×",
                Width = 18,
                Height = 18,
                FontSize = 12,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                ToolTip = "Close tab",
                Cursor = Cursors.Hand,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Tag = tabIndex
            };
            closeBtn.Click += TabCloseButton_Click;

            panel.Children.Add(label);
            panel.Children.Add(closeBtn);
            return panel;
        }

        private void RefreshTabHeaders()
        {
            for (int i = 0; i < _tabs.Count && i < DocTabs.Items.Count; i++)
            {
                if (DocTabs.Items[i] is not TabItem ti) continue;

                if (ti.Header is StackPanel sp && sp.Children.Count >= 2)
                {
                    if (sp.Children[0] is TextBlock label)
                        label.Text = _tabs[i].DisplayTitle;
                    if (sp.Children[1] is Button btn)
                        btn.Tag = i; // keep index in sync after close/reorder
                }
                else
                {
                    ti.Header = CreateTabHeader(_tabs[i].DisplayTitle, i);
                }
            }
        }

        private void TabCloseButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sender is not Button btn) return;
            int idx = btn.Tag is int i ? i : -1;
            if (idx < 0 || idx >= _tabs.Count) return;
            CloseTabAt(idx);
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
            int idx = DocTabs.SelectedIndex;
            if (idx >= 0) CloseTabAt(idx);
        }

        private void CloseTabAt(int idx)
        {
            if (idx < 0 || idx >= _tabs.Count) return;

            // If closing the active tab, save editor first
            if (idx == DocTabs.SelectedIndex || idx == _lastTabIndex)
                SaveEditorToCurrentTab();

            var tab = _tabs[idx];
            if (tab.IsModified)
            {
                // Select that tab so user sees it while asking
                if (DocTabs.SelectedIndex != idx)
                {
                    _suppressTabSwitch = true;
                    DocTabs.SelectedIndex = idx;
                    _suppressTabSwitch = false;
                    LoadTabIntoEditor(idx);
                }

                var r = MessageBox.Show("Save changes before closing tab?", "Close Tab",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (r == MessageBoxResult.Cancel) return;
                if (r == MessageBoxResult.Yes)
                {
                    Save_Click(this, new RoutedEventArgs());
                    if (_tabs[idx].IsModified) return;
                }
            }

            if (_tabs.Count <= 1)
            {
                // Reset last remaining tab
                _tabs[0].Title = "Untitled";
                _tabs[0].FilePath = null;
                _tabs[0].Content = "";
                _tabs[0].IsModified = false;
                _currentFilePath = null;
                _isModified = false;
                _suppressTextChanged = true;
                MainEditor.Text = "";
                _suppressTextChanged = false;
                RefreshTabHeaders();
                UpdateTitle();
                UpdateStatusBar();
                return;
            }

            _tabs.RemoveAt(idx);
            DocTabs.Items.RemoveAt(idx);
            RefreshTabHeaders(); // re-index close button Tags

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
