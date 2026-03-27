using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Pizzeria.Pos.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty!;
        public string Pin { get; set; } = string.Empty!;
        public string Role { get; set; } = "Kelner"; // "Kelner" lub "Manager"
        public bool IsActive { get; set; } = true;
    }
}
