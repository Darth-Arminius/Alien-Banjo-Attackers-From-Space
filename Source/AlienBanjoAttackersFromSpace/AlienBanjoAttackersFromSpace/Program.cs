using System;

namespace AlienBanjoAttackersFromSpace
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (ABAFS game = new ABAFS())
            {
                game.Run();
            }
        }
    }
#endif
}

