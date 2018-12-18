using System;
using Newtonsoft.Json;
using ClassifiedAds.ApplicationServices.Commands;

namespace ClassifiedAds.ApplicationServices.Decorators
{
    public class AuditLogDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;

        public AuditLogDecorator(ICommandHandler<TCommand> handler)
        {
            _handler = handler;
        }

        public void Handle(TCommand command)
        {
            string commandJson = JsonConvert.SerializeObject(command);
            Console.WriteLine($"Command of type {command.GetType().Name}: {commandJson}");
            _handler.Handle(command);
        }
    }
}