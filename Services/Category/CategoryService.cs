using System.Xml.Linq;

namespace ProductManagementSystem.Services.Category;

public class CategoryService : ICategoryService
{
    private readonly string _pathData;
    public CategoryService(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration(XmlElements.XmlVersion, XmlElements.XmlUnicode, XmlElements.XmlBool);
            XElement xElement = new XElement(XmlElements.DataSource, new XElement(XmlElements.Categories));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public async Task<IEnumerable<Entities.Category>> GetAll()
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var nodeList = xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Categories)!
                .Elements(XmlElements.Category).Select(x => new Entities.Category() 
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    Name = x.Element(XmlElements.Name)!.Value,
                    Description = x.Element(XmlElements.Description)!.Value
                });
            return nodeList!;
        }
    }
    
    public async Task<IEnumerable<Entities.CategoryWithProductCount>> GetCategoriesWithProductCount()
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var nodeList = from c in xDocument.Element(XmlElements.DataSource)?
                    .Element(XmlElements.Categories)!
                .Elements(XmlElements.Category) 
                join p in xDocument.Element(XmlElements.DataSource)?
                    .Element(XmlElements.Products)!
                    .Elements(XmlElements.Product)
                    on c.Element(XmlElements.Id) equals p.Element(XmlElements.CategoryId)
                    group p by c into pc
                select new Entities.CategoryWithProductCount() 
                {
                    CategoryName = pc.Key.Element(XmlElements.Name).Value,
                    ProductCount = pc.Count()
                };
            return nodeList!;
        }
    }


    public async Task<Entities.Category> GetById(int id)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var node = xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Categories)!
                .Elements(XmlElements.Category).Select(x => new Entities.Category() 
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    Name = x.Element(XmlElements.Name)!.Value,
                    Description = x.Element(XmlElements.Description)!.Value
                }).FirstOrDefault(x=>x.Id==id);
            return node!;
        }
    }

    public async Task<bool> Create(Entities.Category category)
    {
        using (var stream = new FileStream(_pathData, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            int maxId = 0;
            if (xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Categories)!.HasElements)
            {
                maxId = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Categories)!
                    .Elements(XmlElements.Category).Select(x => int.Parse(x.Element(XmlElements.Id)!.Value))
                    .LastOrDefault();
            }
            bool isName = xDocument.Elements(XmlElements.Category).Any(x => (string)x.Element(XmlElements.Name)! == category.Name);
            if (isName) return false;
            XElement newCategory = new XElement(XmlElements.Category,
                new XElement(XmlElements.Id, maxId + 1),
                new XElement(XmlElements.Name, category.Name),
                new XElement(XmlElements.Description, category.Description));
            xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Categories)!.Add(newCategory);
            stream.SetLength(0);
            await xDocument.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
            return true;
        }
    }

    public async Task<bool> Update(Entities.Category category)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.ReadWrite))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream,LoadOptions.None,CancellationToken.None);
            XElement? root = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Categories)
                ?.Elements(XmlElements.Category).FirstOrDefault(p => int.Parse(p.Element(XmlElements.Id)!.Value) == category.Id);
            if (root == null) return false;
            root.SetElementValue(XmlElements.Name, category.Name);
            root.SetElementValue(XmlElements.Description, category.Description);
            stream.SetLength(0);
            await xDocument.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
            return true;
        }
    }

    public async Task<bool> Delete(int id)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.ReadWrite))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream,LoadOptions.None,CancellationToken.None);
            XElement? root = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Categories)
                ?.Elements(XmlElements.Category).FirstOrDefault(p => int.Parse(p.Element(XmlElements.Id)!.Value) == id);
            if (root == null) return false;
            root.Remove();
            stream.SetLength(0);
            await xDocument.SaveAsync(stream,SaveOptions.None,CancellationToken.None);
            return true;
        }
    }
}

file class XmlElements
{
    public const string PathData = "PathData";
    public const string DataSource = "Source";
    public const string Categories = "Categories";
    public const string XmlVersion = "1.0";
    public const string XmlUnicode = "utf-8";
    public const string XmlBool = "true";
    public const string Category = "Category";
    public const string Id = "Id";
    public const string Name = "Name";
    public const string Description = "Description";
    public const string Products = "Products";
    public const string Product = "Product";
    public const string CategoryId = "CategoryId";
}