using App.Contracts;
using App.Contracts.Extensions;
using NetMQ;
using NetMQ.Sockets;
using System.Net;
using System.Net.Sockets;
using System.Threading;
//using NetMQ;
//using NetMQ.Sockets;

namespace Infrrastructure.Provider;

public class MessageSource : IMessageSource
{

    private readonly UdpClient _udpClient;

    private readonly DealerSocket _dealerSocket;
    private readonly RouterSocket _routerSocket;
    private readonly string _host;

    public MessageSource(DealerSocket dealerSocket, RouterSocket routerSocket, string host)
    {
        _dealerSocket = dealerSocket;
        _routerSocket = routerSocket;
        _host = host;   
    }

    public async Task<Message> Receive(CancellationToken cancellationToken)
    {
        //var msg = await _dealerSocket.ReceiveFrameBytesAsync(cancellationToken);
        var msg = _dealerSocket.ReceiveFrameBytes();
        Message message = MessageExtensions.ToMessage(msg);
        return message;
    }

    /*public async Task Send(Message message, IPEndPoint endpoint, CancellationToken cancellationToken)
    {
        await _udpClient.SendAsync(message.ToBytes(), endpoint, cancellationToken);
    }*/


    public void SendFrame(DealerSocket dealerSocket, Message message)
    {
        //_dealerSocket.Connect(_host);
        _dealerSocket.SendFrame(message.ToBytes());
    }

    public async Task SendMultipartMessage(RouterSocket routerSocket)
    {
        //routerSocket.Bind(_host);
        var msg = await routerSocket.ReceiveMultipartMessageAsync();
        Task.Run(() =>
        {
            var responseMessage = new NetMQMessage();
            responseMessage.Append(msg.First());
            responseMessage.Append(msg.Last());
            routerSocket.SendMultipartMessage(responseMessage);
        });
    }
}

