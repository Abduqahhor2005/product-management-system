using System.Xml.Linq;

namespace ProductManagementSystem.Services.Product;

public class ProductService : IProductService
{
    private readonly string _pathData;

    public ProductService(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;
        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration =
                new XDeclaration(XmlElements.XmlVersion, XmlElements.XmlUnicode, XmlElements.XmlBool);
            XElement element = new XElement(XmlElements.DataSource, new XElement(XmlElements.Products));
            xDocument.Add(element);
            xDocument.Save(_pathData);
        }
    }

    public async Task<IEnumerable<Entities.Product>> GetAll()
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var nodeList = xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Products)!
                .Elements(XmlElements.Product).Select(x=> new Entities.Product()
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    Name = x.Element(XmlElements.Name)!.Value,
                    Description = x.Element(XmlElements.Description)!.Value,
                    Quantity = int.Parse(x.Element(XmlElements.Quantity)!.Value),
                    Price = decimal.Parse(x.Element(XmlElements.Price)!.Value),
                    CategoryId = int.Parse(x.Element(XmlElements.CategoryId)!.Value)
                });
            return nodeList;
        }
    }

    public async Task<IEnumerable<Entities.Product>> GetProductsByCategory(int categoryId)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var nodeList = xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Products)!
                .Elements(XmlElements.Product)
                .Where(x=>int.Parse(x.Element(XmlElements.CategoryId)!.Value)==categoryId)
                .OrderByDescending(x=>x.Element(XmlElements.Price))
                .Select(x=> new Entities.Product()
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    Name = x.Element(XmlElements.Name)!.Value,
                    Description = x.Element(XmlElements.Description)!.Value,
                    Quantity = int.Parse(x.Element(XmlElements.Quantity)!.Value),
                    Price = decimal.Parse(x.Element(XmlElements.Price)!.Value),
                    CategoryId = int.Parse(x.Element(XmlElements.CategoryId)!.Value)
                });
            return nodeList;
        }
    }
    
    public async Task<IEnumerable<Entities.Product>> GetProductsByQuantity(int quantity)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var nodeList = xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Products)!
                .Elements(XmlElements.Product)
                .Where(x=>int.Parse(x.Element(XmlElements.Quantity)!.Value)<quantity)
                .Select(x=> new Entities.Product()
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    Name = x.Element(XmlElements.Name)!.Value,
                    Description = x.Element(XmlElements.Description)!.Value,
                    Quantity = int.Parse(x.Element(XmlElements.Quantity)!.Value),
                    Price = decimal.Parse(x.Element(XmlElements.Price)!.Value),
                    CategoryId = int.Parse(x.Element(XmlElements.CategoryId)!.Value)
                });
            return nodeList;
        }
    }
    
    public async Task<Entities.ProductsWithCategoryAndSupplier> GetProductsWithCategoryAndSupplier(int id)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var node = (from x in xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Products)!
                .Elements(XmlElements.Product)
                join c in xDocument.Element(XmlElements.DataSource)!
                    .Element(XmlElements.Categories)!
                    .Elements(XmlElements.Category)
                    on x.Element(XmlElements.CategoryId) equals c.Element(XmlElements.Id)
                join o in xDocument.Element(XmlElements.DataSource)!
                        .Element(XmlElements.Orders)!
                        .Elements(XmlElements.Order)
                    on x.Element(XmlElements.Id) equals o.Element(XmlElements.ProductId)
                join s in xDocument.Element(XmlElements.DataSource)!
                        .Element(XmlElements.Suppliers)!
                        .Elements(XmlElements.Supplier)
                    on o.Element(XmlElements.SupplierId) equals s.Element(XmlElements.Id)
                where int.Parse(x.Element(XmlElements.Id)!.Value)==id
                select new Entities.ProductsWithCategoryAndSupplier()
                {
                    ProductName = x.Element(XmlElements.Name)!.Value,
                    CategoryName = c.Element(XmlElements.Name)!.Value,
                    SupplierName = s.Element(XmlElements.Name)!.Value
                }).FirstOrDefault();
            return node!;
        }
    }
    
    public async Task<IEnumerable<Entities.ProductsWithCategoryAndSupplier>> 
        GetProductsWithCategoryAndSupplierByPagination(int pageNumber, int pageSize)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var node = (from x in xDocument.Element(XmlElements.DataSource)!
                    .Element(XmlElements.Products)!
                    .Elements(XmlElements.Product)
                join c in xDocument.Element(XmlElements.DataSource)!
                        .Element(XmlElements.Categories)!
                        .Elements(XmlElements.Category)
                    on x.Element(XmlElements.CategoryId) equals c.Element(XmlElements.Id)
                join o in xDocument.Element(XmlElements.DataSource)!
                        .Element(XmlElements.Orders)!
                        .Elements(XmlElements.Order)
                    on x.Element(XmlElements.Id) equals o.Element(XmlElements.ProductId)
                join s in xDocument.Element(XmlElements.DataSource)!
                        .Element(XmlElements.Suppliers)!
                        .Elements(XmlElements.Supplier)
                    on o.Element(XmlElements.SupplierId) equals s.Element(XmlElements.Id)
                select new Entities.ProductsWithCategoryAndSupplier()
                {
                    ProductName = x.Element(XmlElements.Name)!.Value,
                    CategoryName = c.Element(XmlElements.Name)!.Value,
                    SupplierName = s.Element(XmlElements.Name)!.Value
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            return node!;
        }
    }
    
    public async Task<IEnumerable<Entities.Product>> GetProductsByOrderCount()
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

            var node = from x in xDocument.Element(XmlElements.DataSource)!
                    .Element(XmlElements.Products)!
                    .Elements(XmlElements.Product)
                join o in xDocument.Element(XmlElements.DataSource)!
                        .Element(XmlElements.Orders)!
                        .Elements(XmlElements.Order)
                    on x.Element(XmlElements.Id) equals o.Element(XmlElements.ProductId)
                    group x by o into xo
                where xo.Count()<5
                select new Entities.Product()
                {
                    Id = int.Parse(xo.Key.Element(XmlElements.Id).Value),
                    Name = xo.Key.Element(XmlElements.Name).Value
                };
            return node;
        }
    }
    
    public async Task<Entities.Product> GetById(int id)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var node = xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Products)!
                .Elements(XmlElements.Product).Select(x=> new Entities.Product()
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    Name = x.Element(XmlElements.Name)!.Value,
                    Description = x.Element(XmlElements.Description)!.Value,
                    Quantity = int.Parse(x.Element(XmlElements.Quantity)!.Value),
                    Price = decimal.Parse(x.Element(XmlElements.Price)!.Value),
                    CategoryId = int.Parse(x.Element(XmlElements.CategoryId)!.Value)
                }).FirstOrDefault(x=>x.Id==id);
            return node!;
        }
    }

    public async Task<bool> Create(Entities.Product? product)
    {
        if (product == null) return false;

        using (var stream = new FileStream(_pathData, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

            int maxId = 0;
            if (xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Products)!.HasElements)
            {
                maxId = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Products)!
                    .Elements(XmlElements.Product).Select(x => int.Parse(x.Element(XmlElements.Id)!.Value))
                    .LastOrDefault();
            }
            XElement newProduct = new XElement(XmlElements.Product,
                new XElement(XmlElements.Id, maxId + 1),
                new XElement(XmlElements.Name, product.Name),
                new XElement(XmlElements.Description, product.Description),
                new XElement(XmlElements.Quantity, product.Quantity),
                new XElement(XmlElements.Price, product.Price),
                new XElement(XmlElements.CategoryId, product.CategoryId));
            xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Products)!.Add(newProduct);
            stream.SetLength(0);
            await xDocument.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
            return true;
        }
    }

    public async Task<bool> Update(Entities.Product product)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.ReadWrite))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            XElement? element = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Products)!
                .Elements(XmlElements.Product)
                .FirstOrDefault(x => int.Parse(x.Element(XmlElements.Id)!.Value) == product.Id);
            if (element == null) return false;
            element.SetElementValue(XmlElements.Name, product.Name);
            element.SetElementValue(XmlElements.Description, product.Description);
            element.SetElementValue(XmlElements.Quantity, product.Quantity);
            element.SetElementValue(XmlElements.Price, product.Price);
            element.SetElementValue(XmlElements.CategoryId, product.CategoryId);
            stream.SetLength(0);
            await xDocument.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
            return true;
        }
    }

    public async Task<bool> Delete(int id)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.ReadWrite))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            XElement? xElement = xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Products)?
                .Elements(XmlElements.Product).FirstOrDefault(x => int.Parse(x.Element(XmlElements.Id)!.Value) == id);
            if (xElement == null) return false;
            xElement.Remove();
            stream.SetLength(0);
            await xDocument.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
            return true;
        }
    }
}

file class XmlElements
{
    public const string PathData = "PathData";
    public const string DataSource = "Source";
    public const string Products = "Products";
    public const string XmlVersion = "1.0";
    public const string XmlUnicode = "utf-8";
    public const string XmlBool = "true";
    public const string Product = "Product";
    public const string Id = "Id";
    public const string Name = "Name";
    public const string Description = "Description";
    public const string Quantity = "Quantity";
    public const string Price = "Price";
    public const string CategoryId = "CategoryId";
    public const string Categories = "Categories";
    public const string Category = "Category";
    public const string Orders = "Orders";
    public const string Order = "Order";
    public const string ProductId = "ProductId";
    public const string SupplierId = "SupplierId";
    public const string Suppliers = "Suppliers";
    public const string Supplier = "Supplier";
}