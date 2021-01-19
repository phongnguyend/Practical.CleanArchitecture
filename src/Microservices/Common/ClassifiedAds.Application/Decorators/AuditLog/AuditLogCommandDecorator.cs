using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClassifiedAds.Application.Decorators.AuditLog
{
    [Mapping(Type = typeof(AuditLogAttribute))]
    public class AuditLogCommandDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;

        public AuditLogCommandDecorator(ICommandHandler<TCommand> handler)
        {
            _handler = handler;
        }

        public async Task HandleAsync(TCommand command)
        {
            var commandJson = JsonConvert.SerializeObject(command);
            Console.WriteLine($"Command of type {command.GetType().Name}: {commandJson}");
            await _handler.HandleAsync(command);
        }
    }
}