#region Core - ядро программы. В данной библиотеке классов содержатся методы
    /*реализующие логику работы клиента и сервера приложения */ 
#endregion
using System.Text.Json;

namespace Core;

/// <summary>
/// Абстрактный класс родитель, который содержит общие свойства и методы
/// для классов потомков.
/// </summary>
public abstract class  ChatBase
{
    /// <summary>
    /// Источник, содержащий токен (признак) отмены (завершения) задачи (Task).
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
    /// <summary>
    /// Токен отмены задачи, полученный из CancellationTokenSource
    /// </summary>
    protected CancellationToken CancellationToken => CancellationTokenSource.Token;
    /// <summary>
    /// Абстрактный метод для прослушивания портов и дальнейшего получения сообщения.
    /// Конкретная реализация бует в классах потомках.
    /// </summary>
    /// <returns></returns>
    protected abstract Task Listener();
    /// <summary>
    /// Абстрактный метод для зпуска чата. Так как чат делится на клиента и сервера, то они
    /// будут содержать конкретуню реализацию данного метода
    /// </summary>
    /// <returns></returns>
    public abstract Task Start();
}
