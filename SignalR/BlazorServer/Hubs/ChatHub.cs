namespace BlazorServer.Hubs;

using Microsoft.AspNetCore.SignalR;
using Persistence;

public class ChatHub : Hub
{
    private readonly IMessageStore _messageStore;

    public ChatHub(IMessageStore messageStore)
    {
        _messageStore = messageStore;
    }

    public Task SendMessage(string user, string message)
    {
        var formattedMessage = $"{user}: {message}";
        _messageStore.AddMessage(formattedMessage);
        return Clients.All.SendAsync("ReceiveMessage", formattedMessage);
    }

    public override async Task OnConnectedAsync()
    {
        var messages = _messageStore.GetMessages();
        foreach (var historicMessage in messages)
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", historicMessage);

        var message = $"{Context.ConnectionId} joined the chat";
        _messageStore.AddMessage(message);
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}
