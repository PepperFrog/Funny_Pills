using FunnyPills;
using InventorySystem.Items.ThrowableProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FunnyPills
{
    public class Pills
    {
        public List<Scp500> SCP500 { get; private set; } = new()
        {
            new Scp500(),
        };
    }
}
