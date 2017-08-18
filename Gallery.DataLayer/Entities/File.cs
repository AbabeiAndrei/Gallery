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
    [Alias("albums")]
    public class File : IEntity, IHasId<int>
    {
        [Alias("id")]
        [AutoIncrement]
        public int Id { get; set; }

        [Required]
        [Alias("file_path")]
        [StringLength(1024)]
        public string FilePath { get; set; }
        
        [Alias("thumbnail_path")]
        [StringLength(1024)]
        public string ThumbnailPath { get; set; }

        [Required]
        [Alias("extension")]
        [StringLength(64)]
        public string Extension { get; set; }

        [Required]
        [Alias("uploaded_by")]
        [References(typeof(User))]
        public int UploadedBy { get; set; }

        [Required]
        [Alias("uploaded_at")]
        [Default(OrmLiteVariables.SystemUtc)]
        public DateTime UploadedAt { get; set; }

        [Required]
        [Alias("size")]
        public int Size { get; set; }

        [Required]
        [Alias("row_state")]
        [Default((int)RowState.Created)]
        public RowState RowState { get; set; }
    }
}