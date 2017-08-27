using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Autofac;
using AutoMapper;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using Gallery.DataLayer.Managers;
using Gallery.Managers;
using Gallery.Models;
using File = Gallery.DataLayer.Entities.File;

namespace Gallery.Controllers
{
    [Route("Gallery/file")]
    public class FileController : ApiController
    {
        public const string FILE_LOCATION = "D:\\Gallery";
        public const string PHOTOS_LOCATION = "Photos";
        public const string THUMBNAILS_LOCATION = "Thumbnail";

        public static Size ThumbnailSize { get; }

        private readonly FileManager _fileManager;

        public FileController()
            : this(Startup.Resolver.Resolve<FileManager>())
        {
        }

        public FileController(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        static FileController()
        {
            ThumbnailSize = new Size(100, 100);
        }

        [HttpGet]
        [Route("Gallery/file/{id}")]
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
        [Route("Gallery/file/fromResult")]
        public IHttpActionResult CreateFromResult([FromBody] ResultFileUploadModel model)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            if (model == null)
                return BadRequest("file is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fileName = $"{Guid.NewGuid():N}.{model.Extension}";

            var directory = Path.Combine(HttpRuntime.AppDomainAppPath, FileManager.RELATIVE_PATH);
            var directoryThumbnail = Path.Combine(HttpRuntime.AppDomainAppPath, FileManager.RELATIVE_PATH_THUMBNAIL);

            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!Directory.Exists(directoryThumbnail))
                Directory.CreateDirectory(directoryThumbnail);


            var filePath = Path.Combine(directory, fileName);

            var filePathThumbnail = Path.Combine(directoryThumbnail, fileName);

            System.IO.File.WriteAllText(filePath, model.FileContent);

            //var image = Image.FromFile(filePath);

            //var thumbnail = (Image)new Bitmap(image, new Size(200, 200));

            //thumbnail.Save(filePathThumbnail);

            var urlPath = GetUrlPath(FileManager.RELATIVE_PATH, fileName);
            var urlPathThumbnail = GetUrlPath(FileManager.RELATIVE_PATH_THUMBNAIL, fileName);

            var fileInfo = new FileInfo(filePath);

            var file = new File
                       {
                           Extension = model.Extension,
                           FilePath = urlPath,
                           ThumbnailPath = urlPathThumbnail,
                           Size = (int) (fileInfo.Length / 1024),
                           RowState = RowState.Created,
                           UploadedAt = DateTime.Now,
                           UploadedBy = userId
                       };

            _fileManager.Add(file);

            return Ok(Mapper.Map<FileViewModel>(file));
        }


        [HttpPost]
        [Route("Gallery/file/fromBase64")]
        public IHttpActionResult CreateFromBase64([FromBody] ResultFileUploadModel model)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            if (model == null)
                return BadRequest("file is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var strSplit = model.FileContent.Split(new[] { "base64," }, StringSplitOptions.None);

            if (strSplit.Length != 2)
                return BadRequest("invalid base64");

            var base64Str = strSplit[1];

            var fileName = $"{Guid.NewGuid():N}.{model.Extension}";

            var directory = Path.Combine(HttpRuntime.AppDomainAppPath, FileManager.RELATIVE_PATH);
            var directoryThumbnail = Path.Combine(HttpRuntime.AppDomainAppPath, FileManager.RELATIVE_PATH_THUMBNAIL);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!Directory.Exists(directoryThumbnail))
                Directory.CreateDirectory(directoryThumbnail);


            var filePath = Path.Combine(directory, fileName);

            var filePathThumbnail = Path.Combine(directoryThumbnail, fileName);

            var bytes = Convert.FromBase64String(base64Str);
            using (var imageFile = new FileStream(filePath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }

            var image = Image.FromFile(filePath);

            var thumbnail = (Image)new Bitmap(image, new Size(200, 200));

            thumbnail.Save(filePathThumbnail);

            var urlPath = GetUrlPath(FileManager.RELATIVE_PATH, fileName);
            var urlPathThumbnail = GetUrlPath(FileManager.RELATIVE_PATH_THUMBNAIL, fileName);

            var fileInfo = new FileInfo(filePath);

            var file = new File
                       {
                           Extension = model.Extension,
                           FilePath = urlPath,
                           ThumbnailPath = urlPathThumbnail,
                           Size = (int)(fileInfo.Length / 1024),
                           RowState = RowState.Created,
                           UploadedAt = DateTime.Now,
                           UploadedBy = userId,
                           Name = fileName
            };

            _fileManager.Add(file);

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
                           ThumbnailPath = thumbnailPath,
                           Name = fileName
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

            _fileManager.Delete(file.Id);

            return Ok();
        }

        public static string GetUrlPath(string relativePath, string fileName)
        {
            relativePath = relativePath.Replace('\\', '/');
            relativePath += '/' + fileName;

            return Startup.SITE_LOCATION + relativePath;
        }
    }
}