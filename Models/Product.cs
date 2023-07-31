using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product : Entity, ICloneable
    {
        //public int Klucz { get; set; }

        private ILazyLoader _lazyLoader;

        public Product(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public Product()
        {
        }

        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        //public virtual Order? Order { get; set; }

        private Order _order;
        public Order? Order { get => _lazyLoader?.Load(this, ref _order) ?? _order; set => _order = value; }

        //Odpowiednik IsRowVersion z konfiguracji
        //[Timestamp]
        public byte[] Timestamp { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}