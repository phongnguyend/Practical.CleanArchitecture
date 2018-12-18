using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.ApplicationServices.Commands
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}
