using System.Text;
using System.Text.Json;

namespace App.Contracts.Extensions;

/// <summary>
/// Класс расширений, используемых в данном приложении.
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    /// Расширение, которое производить десериализацию (обратное преобразование)
    /// из универсального формата Json в класс нашего приложения Message
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Message? ToMessage(this byte[] data)
        => JsonSerializer.Deserialize<Message>(Encoding.UTF8.GetString(data));

    /// <summary>
    /// Расширение, которое преобразовывает класс Message в массив байтов.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static byte[] ToBytes(this Message message)
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
}
