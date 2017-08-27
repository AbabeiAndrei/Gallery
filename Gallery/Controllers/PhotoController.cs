using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Autofac;
using AutoMapper;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using Gallery.DataLayer.Managers;
using Gallery.Managers;
using Gallery.Models;
using AuthenticationManager = Gallery.Managers.AuthenticationManager;
using File = System.IO.File;

namespace Gallery.Controllers
{
    [Route("Gallery/photo")]
    public class PhotoController : ApiController
    {
        private readonly PhotoManager _photoManager;
        private readonly AuthenticationManager _authManager;
        private readonly FileManager _fileManager;

        public PhotoController()
            : this(Startup.Resolver.Resolve<PhotoManager>(), Startup.Resolver.Resolve<AuthenticationManager>(),
                   Startup.Resolver.Resolve<FileManager>())
        {
        }

        public PhotoController(PhotoManager photoManager, AuthenticationManager authManager, FileManager fileManager)
        {
            _photoManager = photoManager;
            _authManager = authManager;
            _fileManager = fileManager;
        }


        [HttpGet]
        public IHttpActionResult Get()
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var photo = _photoManager.GetUserPhotos(userId);

            return Ok(photo.Select(Mapper.Map<PhotoViewModel>));
        }

        [HttpGet]
        [Route("Gallery/photo/{id}")]
        public IHttpActionResult Get(int id)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var photo = _photoManager.GetById(id);

            if (photo == null)
                return NotFound();

            if (!_authManager.HasAccess(userId, photo, Operation.Read))
                return Unauthorized();

            return Ok(Mapper.Map<PhotoViewModel>(photo));
        }

        [HttpPost]
        public IHttpActionResult Create([FromBody]PhotoModel model)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var photo = Mapper.Map<Photo>(model);

            photo.RowState = RowState.Created;
            photo.UploadedBy = userId;
            photo.UploadedAt = DateTime.Now;

            _photoManager.Add(photo);

            return Created($"/{photo.Id}", Mapper.Map<PhotoViewModel>(photo));
        }

        [HttpPut]
        [Route("Gallery/photo/{id}")]
        public IHttpActionResult Update(int id, [FromBody]PhotoModel model)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var photo = _photoManager.GetById(id);

            if (photo == null)
                return NotFound();

            if (_authManager.HasAccess(userId, photo, Operation.Update))
                return Unauthorized();

            photo.Name = model.Name;
            photo.Privacy = model.Privacy;
            photo.AlbumId = model.AlbumId;
            photo.FileId = model.FileId;

            _photoManager.Update(photo);

            return Ok(photo);
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var photo = _photoManager.GetById(id);

            if (photo == null)
                return NotFound();

            if (_authManager.HasAccess(userId, photo, Operation.Delete))
                return Unauthorized();

            _photoManager.Delete(photo.Id);

            return Ok();
        }

        [HttpPost]
        [Route("Gallery/photo/download")]
        public IHttpActionResult Download([FromBody]DownloadViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest();

            var userId = Request.GetUserId();

            if (userId < 0)
                return BadRequest();


            var listPhotos = _photoManager.GetAll(model.PhotoIds).ToList();

            var files = _fileManager.GetAll(listPhotos.Select(p => p.FileId).Distinct()).ToList();

            var photosWithFile = listPhotos.Select(p => new
                                                        {
                                                            Photo = p,
                                                            File = files.FirstOrDefault(f => f.Id == p.FileId)
                                                        });


            if (files.Count <= 0)
                return NotFound();
            
            string zipName;
            string filePath;

            var directory = Path.Combine(HttpRuntime.AppDomainAppPath, FileManager.RELATIVE_PATH_ARCHIVE);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            do
            {
                zipName = $"{DateTime.Now:yyMMdd_hhmmss}_photos.zip";

                filePath = Path.Combine(directory, zipName);

            } while (File.Exists(filePath));

            using (var zip = ZipFile.Open(filePath, ZipArchiveMode.Create))
            {
                foreach (var photo in photosWithFile)
                {
                    if(photo.File == null)
                        continue;

                    zip.CreateEntryFromFile(_fileManager.GetLocalPath(photo.File, HttpRuntime.AppDomainAppPath), 
                                            $"{photo.Photo.Name}.{photo.File.Extension}");
                }
            }

            return Ok(FileController.GetUrlPath(FileManager.RELATIVE_PATH_ARCHIVE, zipName));

        }

        [HttpPut]
        [Route("Gallery/photo/all")]
        public IHttpActionResult SaveAll([FromBody] MultiplePhotoSaveViewModel model)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();
            
            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var photoId in model.PhotoIds)
            {
                var photo = _photoManager.GetById(photoId);

                if(photo == null)
                    continue;
                
                if(!_authManager.HasAccess(userId, photo, Operation.Update))
                    continue;

                photo.AlbumId = model.AlbumId;
                photo.Privacy = model.Privacy;

                _photoManager.Update(photo);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("Gallery/photo/all")]
        public IHttpActionResult Delete([FromBody] MultiplePhotoSaveViewModel model)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            foreach (var photoId in model.PhotoIds)
            {
                if(!_authManager.HasAccess<Photo>(userId, photoId, Operation.Delete))
                    continue;
                
                _photoManager.Delete(photoId);
            }

            return Ok();
        }

        [HttpPost]
        [Route("Gallery/photo/all")]
        public IHttpActionResult Upload([FromBody] MultiplePhotoUploadViewModel model)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var photoModel in model.Photos)
            {
                var photo = new Photo
                            {
                                AlbumId = model.AlbumId,
                                FileId = photoModel.FileId,
                                Name = photoModel.Name,
                                Privacy = photoModel.Privacy,
                                RowState = RowState.Created,
                                UploadedAt = DateTime.Now,
                                UploadedBy = userId
                            };

                _photoManager.Add(photo);
            }

            return Ok();
        }
    }
}