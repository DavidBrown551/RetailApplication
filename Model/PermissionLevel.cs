using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    internal class PermissionLevel
    {
        public ushort PerLevelID { get; }
        public String LevelName { get; set; }
        public ushort PerLevel { get; set; }

        public PermissionLevel(ushort perLevelId,String levelName,ushort perLevel)
        {
            this.PerLevelID = perLevelId;
            this.LevelName = levelName;
            this.PerLevel = perLevel;
        }
        public override string ToString()
        {
            return $"({LevelName},{PerLevel})" ;
        }
    }
}
