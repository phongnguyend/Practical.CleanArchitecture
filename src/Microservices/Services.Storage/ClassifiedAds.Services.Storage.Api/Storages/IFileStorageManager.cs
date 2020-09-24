using ClassifiedAds.Services.Storage.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClassifiedAds.Services.Storage.Storages
{
    public interface IFileStorageManager
    {
        void Create(FileEntryDTO fileEntry, MemoryStream stream);
        byte[] Read(FileEntryDTO fileEntry);
        void Delete(FileEntryDTO fileEntry);
    }
}
