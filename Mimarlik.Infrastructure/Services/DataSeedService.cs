using Microsoft.EntityFrameworkCore;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;
using Mimarlik.Infrastructure.Data;

namespace Mimarlik.Infrastructure.Services;

public class DataSeedService
{
    private readonly MimarlikDbContext _context;

    public DataSeedService(MimarlikDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllDataAsync()
    {
        await SeedCategoriesAsync();
        await SeedProjectsAsync();
        await SeedContentBlocksAsync();
        await SeedPhotosAsync();
        await SeedTranslationsAsync();

        Console.WriteLine("Test data seeding completed successfully!");
    }

    private async Task SeedCategoriesAsync()
    {
        if (await _context.Categories.CountAsync() > 1) // Already has more than default
            return;

        var categories = new List<Category>
        {
            new Category
            {
                Title = "Konut Projeleri",
                Description = "Özel konut ve villa projeleri",
                Slug = "konut-projeleri",
                Status = ContentStatus.Published,
                SortOrder = 2
            },
            new Category
            {
                Title = "Ticari Projeler", 
                Description = "Ofis ve ticari alan projeleri",
                Slug = "ticari-projeler",
                Status = ContentStatus.Published,
                SortOrder = 3
            },
            new Category
            {
                Title = "Kamu Binaları",
                Description = "Okul, hastane ve kamu binası projeleri", 
                Slug = "kamu-binalari",
                Status = ContentStatus.Published,
                SortOrder = 4
            },
            new Category
            {
                Title = "İç Mimari",
                Description = "İç mimari ve dekorasyon projeleri",
                Slug = "ic-mimari", 
                Status = ContentStatus.Published,
                SortOrder = 5
            }
        };

        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }

    private async Task SeedProjectsAsync()
    {
        if (await _context.Projects.CountAsync() > 4) // Already has test projects
            return;

        var categories = await _context.Categories.ToListAsync();
        
        var projects = new List<Project>
        {
            new Project
            {
                Title = "Modern Villa Projesi",
                Description = "450 m² modern villa tasarımı. Doğa ile uyumlu, sürdürülebilir malzemeler kullanılarak tasarlanmış lüks villa.",
                Slug = "modern-villa-projesi-2",
                Location = "Bebek, İstanbul",
                CompletionDate = new DateTime(2024, 6, 15),
                Client = "Özel Müşteri",
                Area = 450.00m,
                AreaUnit = "m²",
                CategoryId = categories.FirstOrDefault(c => c.Slug == "konut-projeleri")?.Id,
                Status = ContentStatus.Published,
                SortOrder = 1,
                IsFeatured = true,
                MetaTitle = "Modern Villa Projesi - Bebek İstanbul",
                MetaDescription = "450 m² modern villa tasarımı. Sürdürülebilir malzemeler ve doğa ile uyumlu tasarım.",
                MetaKeywords = "modern villa, İstanbul, sürdürülebilir tasarım"
            },
            new Project
            {
                Title = "Kurumsal Ofis Kompleksi",
                Description = "15.000 m² alana sahip modern ofis kompleksi. LEED Gold sertifikalı, enerji verimli tasarım.",
                Slug = "kurumsal-ofis-kompleksi-2",
                Location = "Levent, İstanbul",
                CompletionDate = new DateTime(2024, 12, 20),
                Client = "ABC Holding",
                Area = 15000.00m,
                AreaUnit = "m²",
                CategoryId = categories.FirstOrDefault(c => c.Slug == "ticari-projeler")?.Id,
                Status = ContentStatus.Published,
                SortOrder = 2,
                IsFeatured = true,
                MetaTitle = "Kurumsal Ofis Kompleksi - Levent",
                MetaDescription = "15.000 m² LEED Gold sertifikalı modern ofis kompleksi tasarımı.",
                MetaKeywords = "ofis kompleksi, LEED, enerji verimli, İstanbul"
            },
            new Project
            {
                Title = "Çağdaş Sanat Müzesi",
                Description = "8.500 m² çağdaş sanat müzesi. Esnek sergi alanları ve interaktif mekanlar.",
                Slug = "cagdas-sanat-muzesi",
                Location = "Beyoğlu, İstanbul",
                CompletionDate = new DateTime(2025, 3, 10),
                Client = "İstanbul Büyükşehir Belediyesi",
                Area = 8500.00m,
                AreaUnit = "m²",
                CategoryId = categories.FirstOrDefault(c => c.Slug == "kamu-binalari")?.Id,
                Status = ContentStatus.Published,
                SortOrder = 3,
                IsFeatured = false,
                MetaTitle = "Çağdaş Sanat Müzesi - Beyoğlu",
                MetaDescription = "8.500 m² çağdaş sanat müzesi tasarımı. Esnek sergi alanları ve modern mimari.",
                MetaKeywords = "sanat müzesi, Beyoğlu, çağdaş mimari"
            },
            new Project
            {
                Title = "Boutique Hotel İç Tasarımı",
                Description = "45 odalı boutique hotel iç mimari projesi. Yerel kültür ve modern konforu birleştiren tasarım.",
                Slug = "boutique-hotel-ic-tasarimi",
                Location = "Karaköy, İstanbul",
                CompletionDate = new DateTime(2024, 8, 30),
                Client = "Tourism Investments Ltd.",
                Area = 3200.00m,
                AreaUnit = "m²",
                CategoryId = categories.FirstOrDefault(c => c.Slug == "ic-mimari")?.Id,
                Status = ContentStatus.Published,
                SortOrder = 4,
                IsFeatured = true,
                MetaTitle = "Boutique Hotel İç Tasarımı - Karaköy",
                MetaDescription = "45 odalı boutique hotel iç mimari projesi. Yerel kültür ve modern konfor.",
                MetaKeywords = "boutique hotel, iç tasarım, Karaköy"
            },
            new Project
            {
                Title = "Ekolojik Konut Kompleksi",
                Description = "120 daireli ekolojik konut kompleksi. Sıfır atık prensibi ve yenilenebilir enerji.",
                Slug = "ekolojik-konut-kompleksi",
                Location = "Ataşehir, İstanbul",
                CompletionDate = new DateTime(2025, 6, 15),
                Client = "Green Living Development",
                Area = 25000.00m,
                AreaUnit = "m²",
                CategoryId = categories.FirstOrDefault(c => c.Slug == "konut-projeleri")?.Id,
                Status = ContentStatus.Published,
                SortOrder = 5,
                IsFeatured = false,
                MetaTitle = "Ekolojik Konut Kompleksi - Ataşehir",
                MetaDescription = "120 daireli ekolojik konut kompleksi. Sürdürülebilir yaşam alanları.",
                MetaKeywords = "ekolojik konut, sürdürülebilir, Ataşehir"
            }
        };

        await _context.Projects.AddRangeAsync(projects);
        await _context.SaveChangesAsync();
    }

    private async Task SeedContentBlocksAsync()
    {
        if (await _context.ContentBlocks.AnyAsync())
            return;

        var projects = await _context.Projects.Where(p => p.Id > 4).ToListAsync(); // Only new projects
        var contentBlocks = new List<ContentBlock>();

        foreach (var project in projects)
        {
            contentBlocks.AddRange(new List<ContentBlock>
            {
                new ContentBlock
                {
                    ProjectId = project.Id,
                    Type = ContentBlockType.Heading,
                    Content = $"<h2>Proje Konsepti</h2>",
                    Properties = "{}",
                    SortOrder = 1,
                    Status = ContentStatus.Published
                },
                new ContentBlock
                {
                    ProjectId = project.Id,
                    Type = ContentBlockType.Paragraph,
                    Content = $"<p>{project.Title} projesi, modern mimari anlayışı ile fonksiyonelliği birleştiren yenilikçi bir yaklaşımla tasarlanmıştır. Sürdürülebilirlik ilkeleri gözetilerek hazırlanan projede, çevresel etki minimize edilmiş ve kullanıcı konforu maksimize edilmiştir.</p>",
                    Properties = "{}",
                    SortOrder = 2,
                    Status = ContentStatus.Published
                },
                new ContentBlock
                {
                    ProjectId = project.Id,
                    Type = ContentBlockType.Heading,
                    Content = $"<h3>Teknik Özellikler</h3>",
                    Properties = "{}",
                    SortOrder = 3,
                    Status = ContentStatus.Published
                },
                new ContentBlock
                {
                    ProjectId = project.Id,
                    Type = ContentBlockType.List,
                    Content = $"<ul><li><strong>Toplam Alan:</strong> {project.Area} {project.AreaUnit}</li><li><strong>Lokasyon:</strong> {project.Location}</li><li><strong>Müşteri:</strong> {project.Client}</li><li><strong>Tamamlanma Tarihi:</strong> {project.CompletionDate?.ToString("MMMM yyyy")}</li></ul>",
                    Properties = "{}",
                    SortOrder = 4,
                    Status = ContentStatus.Published
                },
                new ContentBlock
                {
                    ProjectId = project.Id,
                    Type = ContentBlockType.Quote,
                    Content = "<blockquote><p>Bu proje, mimari mükemmellik ile işlevselliğin buluştuğu noktayı temsil ediyor. Her detay özenle düşünülmüş ve kullanıcı deneyimi odağında tasarlanmıştır.</p></blockquote>",
                    Properties = "{}",
                    SortOrder = 5,
                    Status = ContentStatus.Published
                }
            });
        }

        await _context.ContentBlocks.AddRangeAsync(contentBlocks);
        await _context.SaveChangesAsync();
    }

    private async Task SeedPhotosAsync()
    {
        if (await _context.Photos.CountAsync() > 10) // Already has photos
            return;

        var projects = await _context.Projects.Where(p => p.Id > 4).Take(3).ToListAsync(); // Only new projects
        var photos = new List<Photo>();

        foreach (var project in projects)
        {
            for (int i = 1; i <= 4; i++)
            {
                photos.Add(new Photo
                {
                    FileName = $"seed_project_{project.Id}_photo_{i}.jpg",
                    OriginalFileName = $"{project.Slug}_photo_{i}.jpg",
                    FilePath = $"photos/seed_project_{project.Id}_photo_{i}.jpg",
                    FileSize = $"{(800 + i * 150)} KB",
                    MimeType = "image/jpeg",
                    Width = 1920,
                    Height = 1080,
                    AltText = $"{project.Title} - Fotoğraf {i}",
                    Caption = $"{project.Title} projesinden {i}. görünüm",
                    Description = $"{project.Title} projesinin {i}. fotoğrafı. Projenin mimari detaylarını ve genel görünümünü sergileyen profesyonel fotoğraf çekimi.",
                    ProjectId = project.Id,
                    Status = ContentStatus.Published,
                    SortOrder = i,
                    IsHomepageSlider = i == 1 && project.IsFeatured,
                    SliderText = i == 1 && project.IsFeatured ? $"{project.Title} - {project.Location}" : string.Empty,
                    HasWatermark = true
                });
            }
        }

        // Genel slider fotoğrafları
        photos.AddRange(new List<Photo>
        {
            new Photo
            {
                FileName = "seed_slider_architecture_1.jpg",
                OriginalFileName = "modern_architecture_showcase.jpg",
                FilePath = "photos/seed_slider_architecture_1.jpg",
                FileSize = "1.2 MB",
                MimeType = "image/jpeg",
                Width = 1920,
                Height = 1080,
                AltText = "Modern Mimari Tasarım - Mimarlık Studio",
                Caption = "Çağdaş Mimari Anlayışımız",
                Description = "Modern mimari yaklaşımımızı yansıtan genel showcase fotoğrafı. İnovasyonun gelenekle buluştuğu tasarım dili.",
                ProjectId = null,
                Status = ContentStatus.Published,
                SortOrder = 1,
                IsHomepageSlider = true,
                SliderText = "Modern Mimari ile Geleceği Tasarlıyoruz",
                HasWatermark = true
            },
            new Photo
            {
                FileName = "seed_slider_sustainability_2.jpg", 
                OriginalFileName = "sustainable_design_concept.jpg",
                FilePath = "photos/seed_slider_sustainability_2.jpg",
                FileSize = "1.1 MB",
                MimeType = "image/jpeg",
                Width = 1920,
                Height = 1080,
                AltText = "Sürdürülebilir Tasarım - Çevre Dostu Mimari",
                Caption = "Sürdürülebilir Gelecek için Tasarım",
                Description = "Çevre bilinci ile hazırlanmış sürdürülebilir mimari projelerimizin konsept görseli.",
                ProjectId = null,
                Status = ContentStatus.Published,
                SortOrder = 2,
                IsHomepageSlider = true,
                SliderText = "Doğa ile Uyumlu Sürdürülebilir Mimari",
                HasWatermark = true
            },
            new Photo
            {
                FileName = "seed_slider_innovation_3.jpg",
                OriginalFileName = "architectural_innovation.jpg", 
                FilePath = "photos/seed_slider_innovation_3.jpg",
                FileSize = "1.3 MB",
                MimeType = "image/jpeg",
                Width = 1920,
                Height = 1080,
                AltText = "İnovatif Mimari Çözümler",
                Caption = "Teknoloji ve Tasarımın Buluşması",
                Description = "En güncel teknolojiler ile tasarlanan inovatif mimari projelerimiz.",
                ProjectId = null,
                Status = ContentStatus.Published,
                SortOrder = 3,
                IsHomepageSlider = true,
                SliderText = "İnovasyon ve Tasarımın Mükemmel Uyumu",
                HasWatermark = true
            }
        });

        await _context.Photos.AddRangeAsync(photos);
        await _context.SaveChangesAsync();
    }

    private async Task SeedTranslationsAsync()
    {
        if (await _context.Translations.AnyAsync())
            return;

        var languages = await _context.Languages.ToListAsync();
        var englishLang = languages.FirstOrDefault(l => l.Code == "en");
        var germanLang = languages.FirstOrDefault(l => l.Code == "de");
        
        if (englishLang == null) return;
        
        var categories = await _context.Categories.ToListAsync();
        var projects = await _context.Projects.Where(p => p.Id > 4).Take(3).ToListAsync();
        var translations = new List<Translation>();

        // Kategori çevirileri
        foreach (var category in categories)
        {
            translations.Add(new Translation
            {
                EntityName = "Category",
                EntityId = category.Id,
                FieldName = "Title",
                LanguageId = englishLang.Id,
                Value = GetCategoryEnglishTitle(category.Slug)
            });

            translations.Add(new Translation
            {
                EntityName = "Category",
                EntityId = category.Id,
                FieldName = "Description",
                LanguageId = englishLang.Id,
                Value = GetCategoryEnglishDescription(category.Slug)
            });

            if (germanLang != null)
            {
                translations.Add(new Translation
                {
                    EntityName = "Category",
                    EntityId = category.Id,
                    FieldName = "Title",
                    LanguageId = germanLang.Id,
                    Value = GetCategoryGermanTitle(category.Slug)
                });
            }
        }

        // Proje çevirileri
        foreach (var project in projects)
        {
            translations.Add(new Translation
            {
                EntityName = "Project",
                EntityId = project.Id,
                FieldName = "Title",
                LanguageId = englishLang.Id,
                Value = GetProjectEnglishTitle(project.Slug)
            });

            translations.Add(new Translation
            {
                EntityName = "Project",
                EntityId = project.Id,
                FieldName = "Description",
                LanguageId = englishLang.Id,
                Value = GetProjectEnglishDescription(project.Slug)
            });
        }

        await _context.Translations.AddRangeAsync(translations);
        await _context.SaveChangesAsync();
    }

    private string GetCategoryEnglishTitle(string slug) => slug switch
    {
        "konut-projeleri" => "Residential Projects",
        "ticari-projeler" => "Commercial Projects", 
        "kamu-binalari" => "Public Buildings",
        "ic-mimari" => "Interior Architecture",
        _ => "Architecture Projects"
    };

    private string GetCategoryEnglishDescription(string slug) => slug switch
    {
        "konut-projeleri" => "Private residential and villa projects",
        "ticari-projeler" => "Office and commercial space projects",
        "kamu-binalari" => "School, hospital and public building projects",
        "ic-mimari" => "Interior architecture and decoration projects",
        _ => "Professional architecture projects"
    };

    private string GetCategoryGermanTitle(string slug) => slug switch
    {
        "konut-projeleri" => "Wohnprojekte",
        "ticari-projeler" => "Gewerbeprojekte",
        "kamu-binalari" => "Öffentliche Gebäude",
        "ic-mimari" => "Innenarchitektur",
        _ => "Architekturprojekte"
    };

    private string GetProjectEnglishTitle(string slug) => slug switch
    {
        "modern-villa-projesi-2" => "Modern Villa Project",
        "kurumsal-ofis-kompleksi-2" => "Corporate Office Complex",
        "cagdas-sanat-muzesi" => "Contemporary Art Museum",
        "boutique-hotel-ic-tasarimi" => "Boutique Hotel Interior Design",
        "ekolojik-konut-kompleksi" => "Ecological Housing Complex",
        _ => "Architecture Project"
    };

    private string GetProjectEnglishDescription(string slug) => slug switch
    {
        "modern-villa-projesi-2" => "450 m² modern villa design. Luxury villa designed in harmony with nature using sustainable materials.",
        "kurumsal-ofis-kompleksi-2" => "Modern office complex with 15,000 m² area. LEED Gold certified, energy efficient design.",
        "cagdas-sanat-muzesi" => "8,500 m² contemporary art museum. Flexible exhibition spaces and interactive venues.",
        "boutique-hotel-ic-tasarimi" => "45-room boutique hotel interior architecture project. Design combining local culture and modern comfort.",
        "ekolojik-konut-kompleksi" => "120-unit ecological housing complex. Zero waste principle and renewable energy.",
        _ => "Professional architecture project"
    };
}