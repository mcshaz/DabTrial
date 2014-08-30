using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Infrastructure.Interfaces
{
    public class RandomAdaptor : Random, IRandom { }
    public interface IRandom
    {
        double NextDouble();
    }
}