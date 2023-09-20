using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace KungFuPlatform
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var game = new SampleGame()) game.Run();
        }
    }
}
