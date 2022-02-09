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
class ProductsInput
{
    private static List<string> lines = new List<string>();
    public ProductsInput() {}
    public void ReadProducts()
    {
        string line;
        Console.WriteLine("Ingrese productos:\n");
        while((line = Console.ReadLine()) != "")
        {
            lines.Add(line);
        }
        Console.WriteLine(lines.Count);
    }
    public List<Product> SetProducts()
    {
        Dictionary<string,Product> nameProducts = new Dictionary<string,Product>();
        foreach(string line in lines)
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
                nameProducts.Add(name, new Product(1, name, decimal.Parse(price) / 100 ));
            }            
        }
        return nameProducts.Values.ToList();
    }

}

class Calculator {
    private static string[] freeTaxesProducts = new string[] { "Book", "Chocolate bar" };
    private List<Product> products;
    private decimal salesTaxes = 0.0m;
    private decimal total = 0m;
    public Calculator(List<Product> products)
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
    public void Calculate()
    {
        foreach(Product product in products)
        {
            if (freeTaxesProducts.Contains(product.GetName())){
                product.SetTotal(product.GetPrice()*product.GetQuantity());
                total += product.GetPrice() * product.GetQuantity();
            }
            else
            {
                decimal semiTotal = product.GetPrice() * product.GetQuantity();
                this.salesTaxes += RoundTaxes(semiTotal * 0.10m);
                product.SetTotal( semiTotal + RoundTaxes(semiTotal * 0.10m));
                total += semiTotal + RoundTaxes(semiTotal * 0.10m);
            }
        }
    }

}

class TicketPrinter
{

    private List<Product> products;

    public void PrintTicket(Calculator calculator)
    {
        this.products = calculator.GetProducts();
        string line;
        foreach (Product product in products) {
            line = product.GetName() + ": " + product.GetTotal().ToString("0.00");
            line += (product.GetQuantity() > 1) ? "("+product.GetQuantity()+" @ "+product.GetPrice().ToString("0.00")+ ")": "";
            Console.WriteLine(line);
        }
        Console.WriteLine("Sales Taxes: "+calculator.GetSalesTaxes().ToString("0.00"));
        Console.WriteLine("Total: "+calculator.GetTotal().ToString("0.00"));
    }

}


class SalesTaxes{
    static public void Main(String[] args) {

        ProductsInput p = new ProductsInput();
        p.ReadProducts();
        List<Product> products = p.SetProducts();
        Calculator calculator = new Calculator(products);
        calculator.Calculate();
        TicketPrinter ticketPrinter = new TicketPrinter();
        ticketPrinter.PrintTicket(calculator);
    } 
}