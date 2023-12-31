﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Models
{
    public abstract class Entity : INotifyPropertyChanged, IModifiedDate
    {
        public virtual int Id { get; set; }

        //wykorzystujemy Shadow Property
        //public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate {get; set;}

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}