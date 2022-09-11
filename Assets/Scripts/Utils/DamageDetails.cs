using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public struct DamageDetails
{
    public int damage;
    public int incomingDirection;

    public DamageDetails(int damage, int incomingDirection)
    {
        this.damage = damage;
        this.incomingDirection = incomingDirection;
    }
}
