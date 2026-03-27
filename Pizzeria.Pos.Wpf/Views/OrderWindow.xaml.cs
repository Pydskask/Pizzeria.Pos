using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows;

namespace Pizzeria.Pos.Wpf.Views;

public partial class OrderWindow : Window
{
    public OrderWindow(string orderType, DeliveryData? deliveryData, User currentUser, int? orderId = null)
    {
        InitializeComponent();

        var productRepository = App.ServiceProvider.GetRequiredService<IProductRepository>();
        var orderRepository = App.ServiceProvider.GetRequiredService<IOrderRepository>();
        var printService = App.ServiceProvider.GetRequiredService<IPrintService>();

        var viewModel = new OrderViewModel(productRepository, orderRepository, printService);
        viewModel.Initialize(orderType, deliveryData, currentUser, orderId);

        DataContext = viewModel;
    }

    public OrderViewModel? GetViewModel()
    {
        return DataContext as OrderViewModel;
    }

    private void EditDelivery_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not OrderViewModel viewModel)
            return;

        var deliveryWindow = new DeliveryWindow
        {
            Owner = this
        };

        if (deliveryWindow.DataContext is DeliveryViewModel deliveryViewModel &&
            viewModel.DeliveryData != null)
        {
            deliveryViewModel.Phone = viewModel.DeliveryData.Phone;
            deliveryViewModel.Symbol = viewModel.DeliveryData.Symbol;
            deliveryViewModel.CustomerName = viewModel.DeliveryData.CustomerName;
            deliveryViewModel.City = viewModel.DeliveryData.City;
            deliveryViewModel.Street = viewModel.DeliveryData.Street;
            deliveryViewModel.HouseNumber = viewModel.DeliveryData.HouseNumber;
            deliveryViewModel.ApartmentNumber = viewModel.DeliveryData.ApartmentNumber;
            deliveryViewModel.PostalCode = viewModel.DeliveryData.PostalCode;
            deliveryViewModel.Notes = viewModel.DeliveryData.Notes;
            deliveryViewModel.DeliveryPrice = viewModel.DeliveryData.DeliveryPrice;
            deliveryViewModel.DeliveryTime = viewModel.DeliveryData.DeliveryTime;
            deliveryViewModel.SelectedPaymentMethod = viewModel.DeliveryData.SelectedPaymentMethod;
        }

        var dialogResult = deliveryWindow.ShowDialog();
        if (dialogResult != true)
            return;

        var deliveryResult = deliveryWindow.GetDeliveryData();
        if (deliveryResult == null)
            return;

        viewModel.ApplyEditedDeliveryData(deliveryResult);
    }
}