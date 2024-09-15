using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class ExternalLogin
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string ExternalProvider { get; set; } = null!;
        public string ExternalUserId { get; set; } = null!;
        public string? AccessToken { get; set; }

        public virtual User UidNavigation { get; set; } = null!;
    }
}
