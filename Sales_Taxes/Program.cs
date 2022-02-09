using System;
using System.Collections.Generic;

/// <summary>
/// Product class defines the attributes and methods of a product object
/// </summary>
class Product {
    private int quantity;
    private string name;
    private decimal price;
    private decimal total;
    public Product(int quantity, string name, decimal price)
    {
        this.quantity = quantity;
        this.name = name;
        this.price = price;
    }
    public string GetName()
    {
        return name;
    }
    public decimal GetPrice()
    {
        return this.price;
    }
    public void AddQuantity()
    {
        this.quantity++;
    }
    public int GetQuantity()
    {
        return quantity;
    }

    public void SetTotal(decimal total)
    {
        this.total = total;
    }
    public decimal GetTotal()
    {
        return total;
    }
}
/// <summary>
/// Products Setter sets products list
/// </summary>
class ProductsSetter
{
    /// <summary>
    /// Uses list of string and split the strings to get the products information and creates a list of products
    /// </summary>
    /// <param name="lines"> List of string with products information</param>
    /// <returns>List of products</returns>
    public List<Product> SetProducts(List<string> lines)
    {
        Dictionary<string, Product> nameProducts = new Dictionary<string, Product>();
        foreach (string line in lines)
        {
            string[] aux = line.Split(" at ");
            string price = aux.Last();
            string[] aux2 = aux[0].Split("1 ");
            string name = aux2.Last();

            if (nameProducts.ContainsKey(name))
            {
                nameProducts[name].AddQuantity();
            }
            else
            {
                nameProducts.Add(name, new Product(1, name, decimal.Parse(price) / 100));
            }
        }
        return nameProducts.Values.ToList();
    }
}
/// <summary>
/// Products input from console
/// </summary>
class ProductsInput
{
    /// <summary>
    /// Read console input and returns a list of strings (lines)
    /// </summary>
    /// <returns> List of string</returns>
    public List<string> ReadProducts()
    {
        List<string> lines = new List<string>();
        string line;
        Console.WriteLine("Ingrese productos:\n");
        while((line = Console.ReadLine()) != "")
        {
            lines.Add(line);
        }
        return lines;
    }

}
/// <summary>
/// Interface for total, taxes and discounts calculation
/// </summary>
interface IProductCalculator 
{
    decimal GetTotal();
    void Calculate(Product product);
}
/// <summary>
/// Tax calculation class
/// </summary>
class TaxCalculator : IProductCalculator
{
    private static string[] freeTaxesProducts = new string[] { "Book", "Chocolate bar" };
    private static decimal tax = 0.10m;
    private decimal taxTotal;

    public decimal GetTotal()
    {
        return taxTotal;
    }
    /// <summary>
    /// Round taxes after 5 cents
    /// </summary>
    /// <param name="taxes"> Receives the taxes</param>
    /// <returns>Taxes rounded</returns>
    private decimal RoundTaxes(decimal taxes)
    {
        if ((taxes % 5) % 2 == 0)
        {
            return taxes;
        }
        else
        {
            return Math.Round(taxes, 2);
        }
    }
    /// <summary>
    /// Calculates the total taxes from a product
    /// </summary>
    /// <param name="product">Product</param>
    public void Calculate(Product product)
    {
        if (freeTaxesProducts.Contains(product.GetName()))
        {           
           taxTotal = 0.0m;
        }
        else
        {
            IProductCalculator semiTotalCalculator = new TotalProductCalculator();
            semiTotalCalculator.Calculate(product);
            decimal semiTotal = semiTotalCalculator.GetTotal();
            taxTotal = RoundTaxes(semiTotal * tax);
        }        
    }
}
/// <summary>
/// Calculate the total cost of a product based on the quantity listed.
/// </summary>
class TotalProductCalculator : IProductCalculator // Product total
{
    private decimal totalProduct;
    public decimal GetTotal()
    {
        return totalProduct;
    }
    /// <summary>
    /// Calculates the total price according to the quantity and the single unit price of a product
    /// </summary>
    /// <param name="product">Product</param>
    public void Calculate(Product product)
    {
        totalProduct = product.GetPrice() * product.GetQuantity();
    }
}
/// <summary>
/// Total calculator gets the total ammount of money to pay and taxes total of a list of products 
/// </summary>
class TotalCalculator {
    private List<Product> products;
    private decimal salesTaxes = 0.0m;
    private decimal total = 0m;
    public TotalCalculator(List<Product> products)
    {
        this.products = products;
    }

    public List<Product> GetProducts()
    {
        return products;
    }
    public decimal GetSalesTaxes()
    {
        return salesTaxes;
    }
    public decimal GetTotal()
    {
        return total;
    }
    /// <summary>
    /// Calculates the totals using the IProductCalculator classes
    /// </summary>
    public void Calculate()
    {
        IProductCalculator taxCalculator = new TaxCalculator();
        IProductCalculator totalProductCalculator = new TotalProductCalculator();
        decimal totalProduct;
        foreach (Product product in products)
        {
            taxCalculator.Calculate(product);
            totalProductCalculator.Calculate(product);

            totalProduct = totalProductCalculator.GetTotal() + taxCalculator.GetTotal();
            product.SetTotal(totalProduct);

            total += totalProduct;
            salesTaxes += taxCalculator.GetTotal();
        }
    }

}

/// <summary>
/// Ticket printer prints the ticket information
/// </summary>
class TicketPrinter
{
    private List<Product> products;
    private TotalCalculator totalCalculated;
    public TicketPrinter(TotalCalculator totalCalculated)
    {
        this.totalCalculated = totalCalculated;
        this.products = totalCalculated.GetProducts();
    }
    /// <summary>
    /// Uses the total calculator to get the totals by product and the final total and taxes.
    /// </summary>
    public void PrintTicket()
    {
        string line;
        foreach (Product product in products) {
            line = product.GetName() + ": " + product.GetTotal().ToString("0.00");
            line += (product.GetQuantity() > 1) ? "("+product.GetQuantity()+" @ "+product.GetPrice().ToString("0.00")+ ")": "";
            Console.WriteLine(line);
        }
        Console.WriteLine("Sales Taxes: "+totalCalculated.GetSalesTaxes().ToString("0.00"));
        Console.WriteLine("Total: "+ totalCalculated.GetTotal().ToString("0.00"));
    }

}

/// <summary>
/// Main class
/// </summary>
class SalesTaxes{
    static public void Main(String[] args) {

        // Read and setting of product list
        ProductsInput input = new ProductsInput();
        ProductsSetter productList = new ProductsSetter();
        List<Product> products = productList.SetProducts(input.ReadProducts());

        // Totals calculation
        TotalCalculator calculator = new TotalCalculator(products);
        calculator.Calculate();

        // Printing
        TicketPrinter ticketPrinter = new TicketPrinter(calculator);
        ticketPrinter.PrintTicket();
    } 
}