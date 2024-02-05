#region Библиотека классов App.Contracts - репозиторий Contract
/* Содержит основные модели данного приложения. Расширения для этих моделей
 Список команд для сервера*/
#endregion
namespace App.Contracts;

/// <summary>
/// Перечисление списка команд, которые можно передать серверу либо клиенту.
/// </summary>
public enum Command 
{
    None,
    Join,
    Exit,
    Users,
    Confirm
}