﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Entities.Base;
using ServiceStack.DataAnnotations;
using ServiceStack.Model;
using ServiceStack.OrmLite;

namespace Gallery.DataLayer.Entities
{
    [EnumAsInt]
    public enum UserRole : short
    {
        Regular = 0
    }

    [Alias("users")]
    public class User : Entity, IIdentity
    {
        [Alias("id")]
        [AutoIncrement]
        public override int Id { get; set; }

        [Required]
        [Alias("email")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [Alias("password")]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [Alias("full_name")]
        [StringLength(255)]
        public string FullName { get; set; }

        [Required]
        [Alias("created_at")]
        [Default(OrmLiteVariables.SystemUtc)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Alias("role")]
        [Default((int)UserRole.Regular)]
        public UserRole Role { get; set; }

        [Required]
        [Alias("row_state")]
        public override RowState RowState { get; set; }
    }
}
