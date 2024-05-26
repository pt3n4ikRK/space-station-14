using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using RestSharp;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using System.Threading.Tasks;
using System.Text.Json;

namespace Content.Server.Chat.Commands
{
    [AnyCommand]
    internal sealed class SayCommand : IConsoleCommand
    {
        public string Command => "say";
        public string Description => "Send chat messages to the local channel or a specified radio channel.";
        public string Help => "say <text>";

        public async void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            // Перевірка чи команду викликав гравець
            if (shell.Player is not { } player)
            {
                shell.WriteError("This command cannot be run from the server.");
                return;
            }

            // Перевірка чи гравець знаходиться в грі
            if (player.Status != SessionStatus.InGame)
                return;

            // Перевірка чи гравець має прив'язану сутність
            if (player.AttachedEntity is not { } playerEntity)
            {
                shell.WriteError("You don't have an entity!");
                return;
            }

            // Перевірка чи є аргументи
            if (args.Length < 1)
                return;

            // З'єднання аргументів у повідомлення
            var message = string.Join(" ", args).Trim();
            if (string.IsNullOrEmpty(message))
                return;


            // Відправка перекладеного повідомлення в чат
            IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ChatSystem>()
                .TrySendInGameICMessage(playerEntity, message, InGameICChatType.Speak, ChatTransmitRange.Normal, false, shell, player);
        }
    }
}
