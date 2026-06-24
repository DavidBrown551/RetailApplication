using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class Job
    {
        public short JobID { get; }
        public string JobName { get; set; }//JobName has a maximum length of 20 characters
        public decimal Pay { get; set; }
        public Job(short jobId,string jobName,decimal pay)
        {
            this.JobID = jobId;
            this.JobName = jobName;
            this.Pay = pay;
        }


        public override string ToString()
        {
            return $"({JobName},{Pay})";
        }


    }
}
