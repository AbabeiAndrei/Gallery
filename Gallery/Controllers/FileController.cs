using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AutoMapper;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using Gallery.DataLayer.Managers;
using Gallery.Managers;
using Gallery.Models;
using File = Gallery.DataLayer.Entities.File;

namespace Gallery.Controllers
{
    [Route("api/file")]
    public class FileController : ApiController
    {
        public const string FILE_LOCATION = "D:\\Gallery";
        public const string PHOTOS_LOCATION = "Photos";
        public const string THUMBNAILS_LOCATION = "Thumbnail";

        public static Size ThumbnailSize { get; }

        private readonly FileManager _fileManager;

        public FileController(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        static FileController()
        {
            ThumbnailSize = new Size(100, 100);
        }

        [HttpGet]
        [Route("api/file/{id}")]
        public IHttpActionResult Get(int id)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var file = _fileManager.GetById(id);

            if (file == null)
                return NotFound();
            
            return Ok(Mapper.Map<FileViewModel>(file));
        }

        [HttpPost]
        public IHttpActionResult Create([FromBody]HttpPostedFileBase fileBase)
        {
            if (fileBase == null)
                return BadRequest("File null");

            if (fileBase.ContentLength <= 0)
                return BadRequest("File empty");

            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var fileInfo = new FileInfo(fileBase.FileName);
            var extension = fileInfo.Extension;

            var fileName = $"{Guid.NewGuid():N}.{extension}";
            
            var path = Path.Combine(FILE_LOCATION, PHOTOS_LOCATION, fileName);
            fileBase.SaveAs(path);

            string thumbnailPath = null;

            try
            {
                var img = Image.FromFile(path);
                var thumbnail = (Image)new Bitmap(img, ThumbnailSize);

                thumbnailPath = Path.Combine(FILE_LOCATION, THUMBNAILS_LOCATION, fileName);
                thumbnail.Save(thumbnailPath);
            }
            catch
            {
                //todo log
            }

            var file = new File
                       {
                           RowState = RowState.Created,
                           Size = fileBase.ContentLength * 1024,
                           UploadedBy = userId,
                           UploadedAt = DateTime.Now,
                           Extension = extension,
                           FilePath = path,
                           ThumbnailPath = thumbnailPath
                       };

            _fileManager.Add(file);

            return Created($"/{file.Id}", Mapper.Map<FileViewModel>(file));
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var file = _fileManager.GetById(id);

            if (file == null)
                return NotFound();

            _fileManager.Delete(file);

            return Ok();
        }
    }
}