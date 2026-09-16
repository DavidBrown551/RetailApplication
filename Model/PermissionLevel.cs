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
        public short PerLevelID { get; }
        [StringLength(15)]
        public String LevelName { get; set; }
        public short PerLevel { get; set; }

        public PermissionLevel(short perLevelId,String levelName,short perLevel)
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
