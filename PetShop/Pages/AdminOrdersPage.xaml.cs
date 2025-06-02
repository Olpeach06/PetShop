using PetShop.AppData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetShop.Pages
{
    public partial class AdminOrdersPage : Page
    {
        public List<Statuses> StatusList { get; set; }
        private Zakazy _selectedOrder;

        public AdminOrdersPage()
        {
            InitializeComponent();
            LoadData();
            this.DataContext = this;
        }

        private void LoadData()
        {
            try
            {
                StatusList = AppConnect.model0db.Statuses.ToList();
                cmbStatus.ItemsSource = StatusList;
                cmbStatus.SelectedIndex = 0;

                AppConnect.model0db.Zakazy
                    .Include(z => z.Users)
                    .Include(z => z.Statuses)
                    .Include(z => z.Purchases.Select(p => p.Products))
                    .Load();

                dgOrders.ItemsSource = AppConnect.model0db.Zakazy.Local;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (cmbStatus.SelectedItem is Statuses selectedStatus)
            {
                dgOrders.ItemsSource = AppConnect.model0db.Zakazy
                    .Where(z => z.StatusId == selectedStatus.StatusId)
                    .Include(z => z.Users)
                    .Include(z => z.Statuses)
                    .ToList();
            }
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            cmbStatus.SelectedIndex = 0;
            dgOrders.ItemsSource = AppConnect.model0db.Zakazy.Local;
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is Statuses selectedStatus)
            {
                var comboBox = sender as ComboBox;
                var order = comboBox?.DataContext as Zakazy;

                if (order != null && order.StatusId != selectedStatus.StatusId)
                {
                    // Сохраняем выбранный заказ для отмены изменений
                    _selectedOrder = order;

                    // Подтверждение изменения статуса
                    var result = MessageBox.Show($"Изменить статус заказа #{order.ZakazId} на '{selectedStatus.Name}'?",
                                              "Подтверждение",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            order.StatusId = selectedStatus.StatusId;
                            order.Statuses = selectedStatus;
                            AppConnect.model0db.SaveChanges();

                            // Обновляем отображение
                            dgOrders.Items.Refresh();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при обновлении статуса: {ex.Message}", "Ошибка",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                            // Откатываем изменения
                            AppConnect.model0db.Entry(order).Reload();
                            comboBox.SelectedValue = order.StatusId;
                        }
                    }
                    else
                    {
                        // Отменяем выбор в комбобоксе
                        comboBox.SelectedValue = order.StatusId;
                    }
                }
            }
        }

        private void DgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Сохраняем выбранный заказ при изменении выделения
            _selectedOrder = dgOrders.SelectedItem as Zakazy;
        }
        private void BtnSaveStatus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.Tag as Zakazy;

            if (order != null)
            {
                try
                {
                    AppConnect.model0db.SaveChanges();
                    dgOrders.Items.Refresh();
                    button.Background = Brushes.LightGreen;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                    button.Background = Brushes.LightPink;
                }
            }
        }
    }
}