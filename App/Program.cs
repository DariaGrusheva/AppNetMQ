#region Программа
    /*В данном классе запускается либо клиентская, либо серврная часть программы.
     В папке Properties в файле launchSetting.json - описана конфигурация запуска программы,
        чтобы можно было выбрать в Visual Studio что запустить, клиент или сервер.
     */
#endregion

using Core;
using Infrastructure.Persistence.Contexts;
using Infrrastructure.Provider;
using NetMQ.Sockets;
using System.Net;
using System.Net.Sockets;
// Точка подключения к серверу, указывает программы на каком IP-адресе какой порт слушает сервер.
//IPEndPoint serverEndpoint = new (IPAddress.Parse("127.0.0.1"), 12000);
IMessageSource source;
DealerSocket _dealerSocket = new DealerSocket();
RouterSocket _routerSocket = new RouterSocket();


if (args.Length == 0)
{
    //server
    using(var server = new RouterSocket())
    {
        server.Bind("tcp://*:12000");
        source = new MessageSource(_dealerSocket, server, "tcp://*:12000");

        var chat = new ChatServer(source, new ChatContext(), _routerSocket);
        await chat.Start();
    }
    
}
else
{
    //client
    using(var client = new DealerSocket())
    {
        client.Connect("tcp://127.0.0.1:12000");
        source = new MessageSource(client, _routerSocket, "tcp://127.0.0.1:12000");
        var chat = new ChatClient(args[0], source, _dealerSocket);
        await chat.Start();
    }
     
}
