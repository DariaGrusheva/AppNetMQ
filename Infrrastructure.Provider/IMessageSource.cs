using App.Contracts;
using NetMQ;
using NetMQ.Sockets;
using System.Net;

namespace Infrrastructure.Provider;

public interface IMessageSource
{
    Task<Message> Receive(CancellationToken cancellationToken);
    //Task Send(Message message, CancellationToken cancellationToken);
    //IPEndPoint CreateEndpoint(string address, int port);
    //IPEndPoint GetServerEndPoint();
    void SendFrame(DealerSocket dealerSocket,Message message);
    Task SendMultipartMessage(RouterSocket routerSocket);
}
