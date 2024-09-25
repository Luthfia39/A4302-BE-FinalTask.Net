
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace DAL.DTO.Req
    {
        public class ReqUpdateUserDto
        {
            public string Name { get; set; }
            public string Role { get; set; }
            public decimal Balance { get; set; }
        }
    }