using PetShop.AppData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class AdminOrdersPage : Page
    {
        public List<STATUS> StatusList { get; set; }

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
                    order.status_id = selectedStatus.status_id;
                    try
                    {
                        AppConnect.model0db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении статуса: {ex.Message}", "Ошибка",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        LoadData();
                    }
                }
            }
        }
    }
}