using System;
using Newtonsoft.Json;

namespace ClassifiedAds.Application.Decorators.AuditLog
{
    public class AuditLogCommandDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;

        public AuditLogCommandDecorator(ICommandHandler<TCommand> handler)
        {
            _handler = handler;
        }

        public void Handle(TCommand command)
        {
            var commandJson = JsonConvert.SerializeObject(command);
            Console.WriteLine($"Command of type {command.GetType().Name}: {commandJson}");
            _handler.Handle(command);
        }
    }
}