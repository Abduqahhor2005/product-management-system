using System.Xml.Linq;

namespace ProductManagementSystem.Services.Order;

public class OrderService : IOrderService
{
    private readonly string _pathData;

    public OrderService(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;
        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration =
                new XDeclaration(XmlElements.XmlVersion, XmlElements.XmlUnicode, XmlElements.XmlBool);
            XElement element = new XElement(XmlElements.DataSource, new XElement(XmlElements.Orders));
            xDocument.Add(element);
            xDocument.Save(_pathData);
        }
    }

    public async Task<IEnumerable<Entities.Order>> GetAll()
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var nodeList = xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Orders)!
                .Elements(XmlElements.Order).Select(x => new Entities.Order() 
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    ProductId = int.Parse(x.Element(XmlElements.ProductId)!.Value),
                    Quantity = int.Parse(x.Element(XmlElements.Quantity)!.Value),
                    OrderDate = DateTime.Parse(x.Element(XmlElements.OrderDate)!.Value),
                    SupplierId = int.Parse(x.Element(XmlElements.SupplierId)!.Value),
                    Status = x.Element(XmlElements.Status)!.Value
                });
            return nodeList!;
        }
    }
    
    public async Task<IEnumerable<Entities.Order>> GetOrdersBySupplier(int supplierId,string status)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var nodeList = xDocument.Element(XmlElements.DataSource)?
                .Element(XmlElements.Orders)!
                .Elements(XmlElements.Order)
                .Where(x=>int.Parse(x.Element(XmlElements.SupplierId)!.Value)==supplierId
                && x.Element(XmlElements.Status)!.Value == status)
                .Select(x => new Entities.Order() 
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    ProductId = int.Parse(x.Element(XmlElements.ProductId)!.Value),
                    Quantity = int.Parse(x.Element(XmlElements.Quantity)!.Value),
                    OrderDate = DateTime.Parse(x.Element(XmlElements.OrderDate)!.Value),
                    SupplierId = int.Parse(x.Element(XmlElements.SupplierId)!.Value),
                    Status = x.Element(XmlElements.Status)!.Value
                });
            return nodeList!;
        }
    }
    
    public async Task<IEnumerable<Entities.Order>> GetOrdersByDate(DateTime startDate,DateTime endDate)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var nodeList = from o in xDocument.Element(XmlElements.DataSource)!
                .Element(XmlElements.Orders)!
                .Elements(XmlElements.Order)
                where DateTime.Parse(o.Element(XmlElements.OrderDate)!.Value) > startDate
                && DateTime.Parse(o.Element(XmlElements.OrderDate)!.Value) < endDate
                select new Entities.Order() 
                {
                    Id = int.Parse(o.Element(XmlElements.Id)!.Value),
                    ProductId = int.Parse(o.Element(XmlElements.ProductId)!.Value),
                    Quantity = int.Parse(o.Element(XmlElements.Quantity)!.Value),
                    OrderDate = DateTime.Parse(o.Element(XmlElements.OrderDate)!.Value),
                    SupplierId = int.Parse(o.Element(XmlElements.SupplierId)!.Value),
                    Status = o.Element(XmlElements.Status)!.Value
                };
            return nodeList!;
        }
    }
    
    public async Task<IEnumerable<Entities.Order>> GetAllByPagination(int pageNumber, int pageSize)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var nodeList = xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Orders)!
                .Elements(XmlElements.Order).Select(x => new Entities.Order() 
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    ProductId = int.Parse(x.Element(XmlElements.ProductId)!.Value),
                    Quantity = int.Parse(x.Element(XmlElements.Quantity)!.Value),
                    OrderDate = DateTime.Parse(x.Element(XmlElements.OrderDate)!.Value),
                    SupplierId = int.Parse(x.Element(XmlElements.SupplierId)!.Value),
                    Status = x.Element(XmlElements.Status)!.Value
                }).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            return nodeList!;
        }
    }

    public async Task<Entities.Order> GetById(int id)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.Read))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        
            var node = xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Orders)!
                .Elements(XmlElements.Order).Select(x => new Entities.Order() 
                {
                    Id = int.Parse(x.Element(XmlElements.Id)!.Value),
                    ProductId = int.Parse(x.Element(XmlElements.ProductId)!.Value),
                    Quantity = int.Parse(x.Element(XmlElements.Quantity)!.Value),
                    OrderDate = DateTime.Parse(x.Element(XmlElements.OrderDate)!.Value),
                    SupplierId = int.Parse(x.Element(XmlElements.SupplierId)!.Value),
                    Status = x.Element(XmlElements.Status)!.Value
                }).FirstOrDefault(x=>x.Id==id);
            return node!;
        }
    }

    public async Task<bool> Create(Entities.Order? order)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.ReadWrite))
        {
            if (order == null) return false;
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            int maxId = 0;
            if (xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Orders)!.HasElements)
            {
                maxId = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Orders)!
                    .Elements(XmlElements.Order).Select(x => int.Parse(x.Element(XmlElements.Id)!.Value))
                    .LastOrDefault();
            }

            XElement element = new XElement(XmlElements.Order,
                new XElement(XmlElements.Id, maxId + 1),
                new XElement(XmlElements.ProductId, order.ProductId),
                new XElement(XmlElements.Quantity, order.Quantity),
                new XElement(XmlElements.OrderDate, order.OrderDate),
                new XElement(XmlElements.SupplierId, order.SupplierId),
                new XElement(XmlElements.Status, order.Status));
            xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Orders)!.Add(element);
            stream.SetLength(0);
            await xDocument.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
            return true;
        }
    }

    public async Task<bool> Update(Entities.Order order)
    {
        using (var stream = new FileStream(_pathData, FileMode.Open, FileAccess.ReadWrite))
        {
            XDocument xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            XElement? element = xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Orders)!
                .Elements(XmlElements.Order)
                .FirstOrDefault(x => int.Parse(x.Element(XmlElements.Id)!.Value) == order.Id);
            if (element == null) return false;
            element.SetElementValue(XmlElements.ProductId, order.ProductId);
            element.SetElementValue(XmlElements.Quantity, order.Quantity);
            element.SetElementValue(XmlElements.OrderDate, order.OrderDate);
            element.SetElementValue(XmlElements.SupplierId, order.SupplierId);
            element.SetElementValue(XmlElements.Status, order.Status);
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
                .Element(XmlElements.Orders)?
                .Elements(XmlElements.Order).FirstOrDefault(x => int.Parse(x.Element(XmlElements.Id)!.Value) == id);
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
    public const string Orders = "Orders";
    public const string XmlVersion = "1.0";
    public const string XmlUnicode = "utf-8";
    public const string XmlBool = "true";
    public const string Order = "Order";
    public const string Id = "Id";
    public const string ProductId = "ProductId";
    public const string Quantity = "Quantity";
    public const string OrderDate = "OrderDate";
    public const string SupplierId = "SupplierId";
    public const string Status = "Status";
}