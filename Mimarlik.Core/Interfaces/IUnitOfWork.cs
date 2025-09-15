namespace Mimarlik.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    IProjectRepository Projects { get; }
    IPhotoRepository Photos { get; }
    IContentBlockRepository ContentBlocks { get; }
    ILanguageRepository Languages { get; }
    ITranslationRepository Translations { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}