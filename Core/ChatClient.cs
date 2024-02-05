using App.Contracts;
using Infrrastructure.Provider;
using NetMQ;
using NetMQ.Sockets;
using System.Net;

namespace Core;

/// <summary>
/// Класс реализующий логику работы клиента (программы которая будет установлена на ПК пользователей) чата.
/// Наследуется от базового класса ChatBase.
/// </summary>
public class ChatClient : ChatBase
{
    //мое
    static void Client (int number)
    {
        using (var client = new RequestSocket())
        {
            var r = new Random();
            Task.Delay(r.Next(100, 1000)).Wait();

            client.Connect("tcp://127.0.0.1:5556");
            client.SendFrame(number.ToString());

        }
    }
   
    //
    
    private readonly User _user;
   // private readonly IPEndPoint _serverEndpoint;
    private readonly IMessageSource _source;
    private IEnumerable<User> _users = [];
    private readonly DealerSocket _dealerSocket;

    public ChatClient(string username, IMessageSource source, DealerSocket dealerSocket)
    {
        _user = new User { Name = username };
        //_serverEndpoint = serverEndpoint;
        _source = source;
        _dealerSocket = dealerSocket;
        
    }

    /// <summary>
    /// Override - переопределенный метод абстарктного класса ChatBase для запуска приложения клиента.
    /// </summary>
    /// <returns></returns>
    public override async Task Start()
    {
        var join = new Message { Text = _user.Name, Command = Command.Join };
        _source.SendFrame(_dealerSocket, join);
        
        Task.Run(Listener);

        while (!CancellationToken.IsCancellationRequested)
        { 
            string input = (await Console.In.ReadLineAsync()) ?? string.Empty;
            Message message;
            if (input.Trim().Equals("/exit", StringComparison.CurrentCultureIgnoreCase))
            {
                message = new() { SenderId = _user.Id, Command = Command.Exit };
            }
            else
            {
                message = new() { Text = input, SenderId = _user.Id, Command = Command.None };

            }
            
            _source.SendFrame(_dealerSocket, message);
        }

    }

    /// <summary>
    /// Переоперделенный метод абстарктного класса ChatBase для пролушивания клиентом
    /// неоходимого порта и принятия сообщений по нему.
    /// </summary>
    /// <returns></returns>
    protected override async Task Listener()
    { 
        while (!CancellationToken.IsCancellationRequested)
        {
            try
            {
                Message result = await _source.Receive(CancellationToken);
                if(result is null)
                {
                    throw new Exception("Message is null");
                }

                if(result.Command == Command.Join)
                {
                   JoinHandler(result);
                }
                else if (result.Command == Command.Users)
                {
                    UsersHandler(result);
                }
                else if (result.Command == Command.None)
                {
                    MessageHandler(result);
                }
            }
            catch (Exception exc)
            {
                await Console.Out.WriteLineAsync(exc.Message);
            }
        }
    }

    private void MessageHandler(Message message)
    {
        Console.WriteLine($"{_users.First(u=>u.Id == message.SenderId)}: {message.Text}");
    }

    private void UsersHandler(Message message)
    {
        _users = message.Users;
    }

    private void JoinHandler(Message message)
    {
        _user.Id = message.RecipientId;
        Console.WriteLine("Join success");
    }
}
