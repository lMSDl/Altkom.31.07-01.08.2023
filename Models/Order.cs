﻿using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order : Entity
    {
        private DateTime dateTime;

        //Odpowiednik IsConcurencyToken w konfiguracji
        //[ConcurrencyCheck]
        public DateTime DateTime
        {
            get => dateTime;
            set
            {
                dateTime = value;
                OnPropertyChanged();
            }
        }

        public string Number { get; }
        public string Description { get; }
        public OrderType OrderType { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        public Roles Role { get; set; }

        public Point? DeliveryPoint { get; set; }
    }
}
