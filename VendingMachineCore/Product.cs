using System;

namespace VendingMachineCore
{
    public interface IProduct : IEquatable<IProduct>
    {
        string Name { get; }
        int Cost { get; }
    }

    internal class Product : IProduct
    {
        public Product(string name, int cost)
        {
            this.Name = name;
            this.Cost = cost;
        }

        public string Name {get; private set;}

        public int Cost { get; private set; }

        public override bool Equals(object other)
        {
            Product product = other as Product;
            return product == null ? false : Equals(product);
        }
        
        public bool Equals(IProduct other)
        {
            return this.Name == other.Name;
        }
    }
}
