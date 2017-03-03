using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnleashedApi
{
  public class Customer
  {
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string CustomerName { get; set; }
  }
}
