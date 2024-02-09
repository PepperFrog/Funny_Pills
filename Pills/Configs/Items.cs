using System.Collections.Generic;


namespace FunnyPills
{
    public class Items
    {
        public List<Scp500> SCP500 { get; private set; } = new()
        {
            new Scp500(),
        };
    }
}
