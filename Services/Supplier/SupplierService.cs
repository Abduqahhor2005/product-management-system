using System.Xml.Linq;

namespace ProductManagementSystem.Services.Supplier;

public class SupplierService : ISupplierService
{
    private readonly string _pathData;

    public SupplierService(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;
        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration =
                new XDeclaration(XmlElements.XmlVersion, XmlElements.XmlUnicode, XmlElements.XmlBool);
            XElement element = new XElement(XmlElements.DataSource, new XElement(XmlElements.Suppliers));
            xDocument.Add(element);
            xDocument.Save(_pathData);
        }
    }

    public async Task<IEnumerable<Entities.Supplier>> GetAll()
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            var nodeList = xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Suppliers)!
                .Elements(XmlElements.Supplier).Select(x => new Entities.Supplier()
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    Name = x.Element(XmlElements.Name)!.Value,
                    ContactPerson = x.Element(XmlElements.ContactPerson)!.Value,
                    Email = x.Element(XmlElements.Email)!.Value,
                    Phone = x.Element(XmlElements.Phone)!.Value
                });
            return nodeList;
        }
    }
    
    public async Task<IEnumerable<Entities.Supplier>> GetSuppliersByProductQuantity(int productQuantity)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            var nodeList = from s in xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Suppliers)!
                .Elements(XmlElements.Supplier)
                join o in xDocument.Element(XmlElements.DataSource)!
                    .Element(XmlElements.Orders)!
                    .Elements(XmlElements.Order) 
                    on s.Element(XmlElements.Id) equals o.Element(XmlElements.SupplierId)
                join p in xDocument.Element(XmlElements.DataSource)!
                        .Element(XmlElements.Products)!
                        .Elements(XmlElements.Product) 
                    on o.Element(XmlElements.ProductId) equals p.Element(XmlElements.Id)
                    where int.Parse(p.Element(XmlElements.Quantity)!.Value)==productQuantity
                select new Entities.Supplier()
                {
                    Id = int.Parse(s.Element(XmlElements.Id)!.Value),
                    Name = s.Element(XmlElements.Name)!.Value,
                    ContactPerson = s.Element(XmlElements.ContactPerson)!.Value,
                    Email = s.Element(XmlElements.Email)!.Value,
                    Phone = s.Element(XmlElements.Phone)!.Value
                };
            return nodeList;
        }
    }

    public async Task<Entities.Supplier> GetById(int id)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            var node = xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Suppliers)!
                .Elements(XmlElements.Supplier).Select(x => new Entities.Supplier()
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    Name = x.Element(XmlElements.Name)!.Value,
                    ContactPerson = x.Element(XmlElements.ContactPerson)!.Value,
                    Email = x.Element(XmlElements.Email)!.Value,
                    Phone = x.Element(XmlElements.Phone)!.Value
                }).FirstOrDefault(x => x.Id == id);
            return node!;
        }
    }

    public async Task<bool> Create(Entities.Supplier? supplier)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.ReadWrite))
        {
            if (supplier == null) return false;
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            int maxId = 0;
            if (xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Suppliers)!.HasElements)
            {
                maxId = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Suppliers)!
                    .Elements(XmlElements.Supplier).Select(x => int.Parse(x.Element(XmlElements.Id)!.Value))
                    .LastOrDefault();
            }

            bool isName = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Suppliers)!
                .Elements(XmlElements.Supplier)
                .Any(x => (string)x.Element(XmlElements.Email)! == supplier.Email);
            if (isName) return false;

            XElement element = new XElement(XmlElements.Supplier,
                new XElement(XmlElements.Id, maxId + 1),
                new XElement(XmlElements.Name, supplier.Name),
                new XElement(XmlElements.ContactPerson, supplier.ContactPerson),
                new XElement(XmlElements.Email, supplier.Email),
                new XElement(XmlElements.Phone, supplier.Phone));
            xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Suppliers)!.Add(element);
            stream.SetLength(0);
            await xDocument.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
            return true;
        }
    }

    public async Task<bool> Update(Entities.Supplier supplier)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.ReadWrite))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            XElement? element = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Suppliers)!
                .Elements(XmlElements.Supplier)
                .FirstOrDefault(x => int.Parse(x.Element(XmlElements.Id)!.Value) == supplier.Id);
            if (element == null) return false;
            element.SetElementValue(XmlElements.Name, supplier.Name);
            element.SetElementValue(XmlElements.ContactPerson, supplier.ContactPerson);
            element.SetElementValue(XmlElements.Email, supplier.Email);
            element.SetElementValue(XmlElements.Phone, supplier.Phone);
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
                .Element(XmlElements.Suppliers)?
                .Elements(XmlElements.Supplier).FirstOrDefault(x => int.Parse(x.Element(XmlElements.Id)!.Value) == id);
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
    public const string Suppliers = "Suppliers";
    public const string XmlVersion = "1.0";
    public const string XmlUnicode = "utf-8";
    public const string XmlBool = "true";
    public const string Supplier = "Supplier";
    public const string Id = "Id";
    public const string Name = "Name";
    public const string ContactPerson = "ContactPerson";
    public const string Email = "Email";
    public const string Phone = "Phone";
    public const string Orders = "Orders";
    public const string Order = "Order";
    public const string SupplierId = "SupplierId";
    public const string ProductId = "ProductId";
    public const string Products = "Products";
    public const string Product = "Product";
    public const string Quantity = "Quantity";
}