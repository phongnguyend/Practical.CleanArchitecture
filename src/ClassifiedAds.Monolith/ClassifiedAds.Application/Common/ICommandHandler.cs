using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Application
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}
