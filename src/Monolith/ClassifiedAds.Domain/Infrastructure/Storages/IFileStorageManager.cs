using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.Storages
{
    public interface IFileStorageManager
    {
        void Create(IFileEntry fileEntry, Stream stream);
        Task CreateAsync(IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default);

        byte[] Read(IFileEntry fileEntry);
        Task<byte[]> ReadAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default);

        void Delete(IFileEntry fileEntry);
        Task DeleteAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default);
    }

    public interface IFileEntry
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string FileLocation { get; set; }
    }
}
