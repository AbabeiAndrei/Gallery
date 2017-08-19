using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using Gallery.DataLayer.Managers;
using Gallery.Managers;
using Gallery.Models;

namespace Gallery.Controllers
{
    public class PhotoController : ApiController
    {
        private readonly PhotoManager _photoManager;
        private readonly AuthenticationManager _authManager;

        public PhotoController(PhotoManager photoManager, AuthenticationManager authManager)
        {
            _photoManager = photoManager;
            _authManager = authManager;
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

            _photoManager.Delete(photo);

            return Ok();
        }
    }
}