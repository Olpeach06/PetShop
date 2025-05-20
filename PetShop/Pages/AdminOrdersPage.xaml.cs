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
        public List<STATUS> StatusList { get; set; }
        private ZAKAZ _selectedOrder;

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
                StatusList = AppConnect.model0db.STATUS.ToList();
                cmbStatus.ItemsSource = StatusList;
                cmbStatus.SelectedIndex = 0;

                AppConnect.model0db.ZAKAZ
                    .Include(z => z.USERS)
                    .Include(z => z.STATUS)
                    .Include(z => z.PURCHASE.Select(p => p.PRODUCTS))
                    .Load();

                dgOrders.ItemsSource = AppConnect.model0db.ZAKAZ.Local;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (cmbStatus.SelectedItem is STATUS selectedStatus)
            {
                dgOrders.ItemsSource = AppConnect.model0db.ZAKAZ
                    .Where(z => z.status_id == selectedStatus.status_id)
                    .Include(z => z.USERS)
                    .Include(z => z.STATUS)
                    .ToList();
            }
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            cmbStatus.SelectedIndex = 0;
            dgOrders.ItemsSource = AppConnect.model0db.ZAKAZ.Local;
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is STATUS selectedStatus)
            {
                var comboBox = sender as ComboBox;
                var order = comboBox?.DataContext as ZAKAZ;

                if (order != null && order.status_id != selectedStatus.status_id)
                {
                    // Сохраняем выбранный заказ для отмены изменений
                    _selectedOrder = order;

                    // Подтверждение изменения статуса
                    var result = MessageBox.Show($"Изменить статус заказа #{order.zakaz_id} на '{selectedStatus.name}'?",
                                              "Подтверждение",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            order.status_id = selectedStatus.status_id;
                            order.STATUS = selectedStatus;
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
                            comboBox.SelectedValue = order.status_id;
                        }
                    }
                    else
                    {
                        // Отменяем выбор в комбобоксе
                        comboBox.SelectedValue = order.status_id;
                    }
                }
            }
        }

        private void DgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Сохраняем выбранный заказ при изменении выделения
            _selectedOrder = dgOrders.SelectedItem as ZAKAZ;
        }
        private void BtnSaveStatus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.Tag as ZAKAZ;

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