using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace VistaControls
{
    public class VistaTable : Control
    {
        private ListView? _listView;
        private GridView? _gridView;

        static VistaTable()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaTable), new FrameworkPropertyMetadata(typeof(VistaTable)));
        }

        public VistaTable()
        {
            Columns = new ObservableCollection<VistaTableColumn>();
            Columns.CollectionChanged += Columns_CollectionChanged;
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(VistaTable), new PropertyMetadata(null, OnItemsSourceChanged));

        public IEnumerable? ItemsSource
        {
            get => (IEnumerable?)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty BorderedProperty =
            DependencyProperty.Register(nameof(Bordered), typeof(bool), typeof(VistaTable), new PropertyMetadata(false, OnAppearanceChanged));

        public bool Bordered
        {
            get => (bool)GetValue(BorderedProperty);
            set => SetValue(BorderedProperty, value);
        }

        public static readonly DependencyProperty TableHeightProperty =
            DependencyProperty.Register(nameof(TableHeight), typeof(double), typeof(VistaTable), new PropertyMetadata(double.NaN, OnAppearanceChanged));

        public double TableHeight
        {
            get => (double)GetValue(TableHeightProperty);
            set => SetValue(TableHeightProperty, value);
        }

        public static readonly DependencyProperty RowClassNameSelectorProperty =
            DependencyProperty.Register(nameof(RowClassNameSelector), typeof(Func<object, int, string>), typeof(VistaTable), new PropertyMetadata(null));

        public Func<object, int, string>? RowClassNameSelector
        {
            get => (Func<object, int, string>?)GetValue(RowClassNameSelectorProperty);
            set => SetValue(RowClassNameSelectorProperty, value);
        }

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(VistaTable), new PropertyMetadata(double.NaN, OnAppearanceChanged));

        public double RowHeight
        {
            get => (double)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(ObservableCollection<VistaTableColumn>), typeof(VistaTable), new PropertyMetadata(null, OnColumnsChanged));

        public ObservableCollection<VistaTableColumn> Columns
        {
            get => (ObservableCollection<VistaTableColumn>)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _listView = GetTemplateChild("PART_ListView") as ListView;
            if (_listView != null)
            {
                _gridView = new GridView();
                _listView.View = _gridView;
                _listView.ItemsSource = ItemsSource;
                if (!double.IsNaN(TableHeight) && TableHeight > 0)
                {
                    _listView.Height = TableHeight;
                }
                _listView.AlternationCount = int.MaxValue;
                RebuildColumns();
            }
        }

        private void Columns_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildColumns();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTable table && table._listView != null)
            {
                table._listView.ItemsSource = e.NewValue as IEnumerable;
            }
        }

        private static void OnAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTable table)
            {
                if (table._listView != null && e.Property == TableHeightProperty)
                {
                    var h = (double)e.NewValue;
                    table._listView.Height = (!double.IsNaN(h) && h > 0) ? h : double.NaN;
                }
                if (e.Property == BorderedProperty)
                {
                    table.RebuildColumns();
                }
                if (e.Property == RowHeightProperty)
                {
                    table.RebuildColumns();
                }
            }
        }

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTable table)
            {
                if (e.OldValue is ObservableCollection<VistaTableColumn> oldCols)
                {
                    oldCols.CollectionChanged -= table.Columns_CollectionChanged;
                }
                if (e.NewValue is ObservableCollection<VistaTableColumn> newCols)
                {
                    newCols.CollectionChanged += table.Columns_CollectionChanged;
                }
                table.RebuildColumns();
            }
        }

        private void RebuildColumns()
        {
            if (_gridView == null) return;
            _gridView.Columns.Clear();
            if (Columns == null) return;

            for (int i = 0; i < Columns.Count; i++)
            {
                var col = Columns[i];
                var gridCol = new GridViewColumn { Header = col.Label };
                if (Bordered && !string.IsNullOrWhiteSpace(col.Prop))
                {
                    // 使用带右边框的单元格模板
                    var borderFactory = new FrameworkElementFactory(typeof(Border));
                    borderFactory.SetValue(Border.BorderBrushProperty, TryFindResource("DefaultBorderColor") as Brush ?? Brushes.LightGray);
                    // 仅非最后一列显示右边框，最后一列不画右边框
                    var isLast = (i == Columns.Count - 1);
                    borderFactory.SetValue(Border.BorderThicknessProperty, isLast ? new Thickness(0) : new Thickness(0, 0, 0, 0));
                    borderFactory.SetValue(Border.PaddingProperty, new Thickness(0));
                    if (!double.IsNaN(RowHeight) && RowHeight > 0)
                    {
                        borderFactory.SetValue(FrameworkElement.HeightProperty, RowHeight);
                        borderFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
                    }

                    var textFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textFactory.SetBinding(TextBlock.TextProperty, new Binding(col.Prop));
                    textFactory.SetValue(TextBlock.ForegroundProperty, TryFindResource("DefaultTextColor") as Brush ?? Brushes.Black);
                    textFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                    textFactory.SetValue(TextBlock.FontSizeProperty, 15d);
                    textFactory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
                    textFactory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.NoWrap);

                    borderFactory.AppendChild(textFactory);
                    gridCol.CellTemplate = new DataTemplate { VisualTree = borderFactory };
                }
                else
                {
                    gridCol.DisplayMemberBinding = string.IsNullOrWhiteSpace(col.Prop) ? null : new Binding(col.Prop);
                }
                if (!double.IsNaN(col.Width) && col.Width > 0)
                {
                    gridCol.Width = col.Width;
                }
                _gridView.Columns.Add(gridCol);
            }
        }
    }
}

