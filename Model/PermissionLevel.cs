using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class PermissionLevel
    {
        public int PerLevelID { get; }
        [StringLength(15)]
        public String LevelName { get; set; }
        public byte PerLevel { get; set; }

        public PermissionLevel(int perLevelId,String levelName,byte perLevel)
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
