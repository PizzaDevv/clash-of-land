using System.Collections.Generic;

namespace ClashLand.Logic
{
    public class AchievementsSlot : List<Achievement>
    {
        public new void Add(Achievement achievement)
        {
            if (!Contains(achievement))
                base.Add(achievement);
        }
    }
}