using ClassifiedAds.Modules.Storage.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClassifiedAds.Modules.Storage.Storages
{
    public interface IFileStorageManager
    {
        void Create(FileEntryDTO fileEntry, MemoryStream stream);
        byte[] Read(FileEntryDTO fileEntry);
        void Delete(FileEntryDTO fileEntry);
    }
}
