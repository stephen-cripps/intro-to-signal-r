namespace BlazorServer.Persistence;

public interface IMessageStore
{
    void AddMessage(string message);
    IEnumerable<string> GetMessages();
}

public class MessageStore : IMessageStore
{
    private List<string> Messages { get; } = [];

    public void AddMessage(string message)
    {
        Messages.Add(message);
    }

    public IEnumerable<string> GetMessages()
    {
        return Messages;
    }
}
