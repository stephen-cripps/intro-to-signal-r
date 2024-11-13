namespace WPFClient;

using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

public partial class MainWindow : Window
{
    private readonly HubConnection _connection;
    private readonly HubConnection _counterConnection;

    public MainWindow()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7088/chathub")
            .WithAutomaticReconnect()
            .Build();

        _counterConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7088/counterhub")
            .WithAutomaticReconnect()
            .Build();

        _connection.Reconnecting += _ =>
        {
            Dispatcher.Invoke(() => { Messages.Items.Add("Attempting to reconnect..."); });

            return Task.CompletedTask;
        };

        _connection.Reconnected += _ =>
        {
            Dispatcher.Invoke(() => { Messages.Items.Add("Reconnected!"); });

            return Task.CompletedTask;
        };

        _connection.Closed += _ =>
        {
            Dispatcher.Invoke(() =>
            {
                Messages.Items.Add("Connection closed.");
                OpenConnection.IsEnabled = true;
                SendMessage.IsEnabled = false;
            });

            return Task.CompletedTask;
        };
    }

    private async void OpenConnection_Click(object sender, RoutedEventArgs e)
    {
        _connection.On<string>("ReceiveMessage",
            message => { Dispatcher.Invoke(() => { Messages.Items.Add(message); }); });

        try
        {
            await _connection.StartAsync();

            // There's a bit of a race condition here, but it's fine for this sample
            Messages.Items.Add("Connection started!");
            OpenConnection.IsEnabled = false;
            SendMessage.IsEnabled = true;
        }
        catch (Exception ex)
        {
            Messages.Items.Add(ex.Message);
        }
    }

    private async void SendMessage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _connection.SendAsync("SendMessage", "WPF Client", MessageInput.Text);
            MessageInput.Text = string.Empty;
        }
        catch (Exception ex)
        {
            Messages.Items.Add(ex.Message);
        }
    }

    private async void OpenCounter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _counterConnection.StartAsync();
        }
        catch (Exception ex)
        {
            Messages.Items.Add("Error Connecting to Counter: " + ex.Message);
        }
    }

    private async void IncrementCounter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _counterConnection.SendAsync("AddToTotal", "WPF Client", 1);
        }
        catch (Exception ex)
        {
            Messages.Items.Add("Error Incrementing Counter: " + ex.Message);
        }
    }
}
