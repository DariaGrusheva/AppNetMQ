using App.Contracts;
using System.Net;

namespace Infrrastructure.Provider;

public record ReceiveResult(IPEndPoint EndPoint, Message? Message);

