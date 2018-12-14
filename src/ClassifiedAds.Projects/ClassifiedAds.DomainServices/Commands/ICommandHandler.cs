using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices.Commands
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}
