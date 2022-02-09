using System;
using System.Collections.Generic;

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

class ProductsSetter
{
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
                nameProducts[aux2.Last()].AddQuantity();
            }
            else
            {
                nameProducts.Add(name, new Product(1, name, decimal.Parse(price) / 100));
            }
        }
        return nameProducts.Values.ToList();
    }
}

class ProductsInput
{
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
interface IProductCalculator // For total, taxes and discounts
{
    decimal GetTotal();
    void Calculate(Product product);
}
class TaxCalculator : IProductCalculator // Product taxes
{
    private static string[] freeTaxesProducts = new string[] { "Book", "Chocolate bar" };
    private static decimal tax = 0.10m;
    private decimal taxTotal;

    public decimal GetTotal()
    {
        return taxTotal;
    }
    public decimal RoundTaxes(decimal taxes)
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
class TotalProductCalculator : IProductCalculator // Product total
{
    private decimal totalProduct;
    public decimal GetTotal()
    {
        return totalProduct;
    }
    public void Calculate(Product product)
    {
        totalProduct = product.GetPrice() * product.GetQuantity();
    }
}
class TotalCalculator {
    private static string[] freeTaxesProducts = new string[] { "Book", "Chocolate bar" };
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

class TicketPrinter
{

    private List<Product> products;
    private TotalCalculator totalCalculated;
    public TicketPrinter(TotalCalculator totalCalculated)
    {
        this.totalCalculated = totalCalculated;
        this.products = totalCalculated.GetProducts();
    }
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