using Microsoft.EntityFrameworkCore.Storage;
using Mimarlik.Core.Interfaces;
using Mimarlik.Infrastructure.Data;
using Mimarlik.Infrastructure.Repositories;

namespace Mimarlik.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MimarlikDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(MimarlikDbContext context)
    {
        _context = context;
        Categories = new CategoryRepository(_context);
        Projects = new ProjectRepository(_context);
        Photos = new PhotoRepository(_context);
        ContentBlocks = new ContentBlockRepository(_context);
        Languages = new LanguageRepository(_context);
        Translations = new TranslationRepository(_context);
    }

    public ICategoryRepository Categories { get; }
    public IProjectRepository Projects { get; }
    public IPhotoRepository Photos { get; }
    public IContentBlockRepository ContentBlocks { get; }
    public ILanguageRepository Languages { get; }
    public ITranslationRepository Translations { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}