﻿using ClassifiedAds.Domain.Notification;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.Domain.Entities
{
    public class EmailMessage : AggregateRoot<Guid>, IEmailMessage
    {
        public string From { get; set; }

        public string Tos { get; set; }

        public string CCs { get; set; }

        public string BCCs { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public int RetriedCount { get; set; }

        public string Log { get; set; }

        public DateTimeOffset? SentDateTime { get; set; }

        public Guid? CopyFromId { get; set; }

        public ICollection<EmailMessageAttachment> EmailMessageAttachments { get; set; }
    }
}
