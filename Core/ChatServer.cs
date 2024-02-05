using App.Contracts;
using Infrastructure.Persistence.Contexts;
using Infrrastructure.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NetMQ.Sockets;

namespace Core;

public class ChatServer : ChatBase
{
    private readonly IMessageSource _source;
    private readonly ChatContext _context;
    private HashSet<User> _users = [];
    private readonly RouterSocket _routerSocket;


    public ChatServer(IMessageSource source, ChatContext context, RouterSocket routerSocket)
    {
        _source = source;
        _context = context;
        _routerSocket = routerSocket;
    }

    public override async Task Start()
    {
        await Task.CompletedTask;
        await Task.Run(Listener);
    }

    protected override async Task Listener()
    {
        while (!CancellationToken.IsCancellationRequested)
        {
            try
            {
                Message result = (await _source.Receive(CancellationToken)) ?? throw new Exception("Message is null");

                switch (result.Command)
                {
                    case Command.None:
                        await MessageHandler(result);
                        break;
                    case Command.Join:
                        await MessageHandler(result);
                        break;
                    case Command.Exit:
                        await ExitHandler(result);
                        break;
                    case Command.Users:
                        break;
                    case Command.Confirm:
                        break;
                }

            }
            catch (Exception exc)
            {

                await Console.Out.WriteLineAsync(exc.Message);
            }
        }
    }

    private async Task ExitHandler(Message result)
    {
        var user = User.FromDomain(await _context.Users.FirstAsync(x => x.Id == result.SenderId));
        user.LastOnline = DateTime.Now;
        await _context.SaveChangesAsync();

        _users.Remove(_users.First(u=>u.Id == result.SenderId));
    }

    private async Task MessageHandler(Message result)
    {
        await SendAllAsync(result);
        /*if (result.RecipientId < 0)
        {
            await SendAllAsync(result);
        }
        else
        {
            await _source.Send(
                result.Message,
                _users.First(u => u.Id == result.Message.SenderId).EndPoint!,
                CancellationToken);
            var recipientEndpoint = _users.FirstOrDefault(u => u.Id == result.Message.SenderId)?.EndPoint;
            if (recipientEndpoint is not null)
            {
                await _source.Send(
                    result.Message,
                    recipientEndpoint,
                    CancellationToken);
            }
        }*/
    }

    /*private async Task JoinHandler(Message result)
    {
        User? user = _users.FirstOrDefault(u => u.Name == !.Text);
        if (user is null)
        {
            user = new User() { Name = result!.Text };
            _users.Add(user);
        }
       // user.EndPoint = result.EndPoint;

        await _source.Send(
            new Message() { Command = Command.Join, RecipientId = user.Id },
            user.EndPoint!,
            CancellationToken);

        await SendAllAsync(new Message() { Command = Command.Confirm, Text = $"{user.Name} joined to chat" });
        await SendAllAsync(new Message() { Command = Command.Users, RecipientId = user.Id, Users = _users });

        var unreaded = await _context.Messages.Where(x => x.RecipientId == user.Id).ToListAsync();

        foreach (var message in unreaded)
        {
            await _source.Send(
            Message.FromDomain(message),
            user.EndPoint!,
            CancellationToken);
        }
    }*/

    private async Task SendAllAsync(Message message)
    {
        await _source.SendMultipartMessage(_routerSocket);
        /*foreach (User user in _users)
        {
            await _source.Send(
            message,
            user.EndPoint!,
            CancellationToken);
        }*/
    }
}