﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Converters
{
    public class StringConverter : ValueConverter<string, string>
    {
        public StringConverter() : base(
            x => Convert.ToBase64String(Encoding.Default.GetBytes(x)),
            x => Encoding.Default.GetString(Convert.FromBase64String(x)))
        {
        }
    }
}
