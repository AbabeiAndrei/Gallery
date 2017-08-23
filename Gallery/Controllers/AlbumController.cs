using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Autofac;
using AutoMapper;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using Gallery.DataLayer.Managers;
using Gallery.DataLayer.Repositories;
using Gallery.Managers;
using Gallery.Models;

namespace Gallery.Controllers
{
    [Route("api/album")]
    public class AlbumController : ApiController
    {
        private readonly AlbumManager _albumManager;
        private readonly AuthenticationManager _authManager;

        public AlbumController() 
            : this(Startup.Resolver.Resolve<AlbumManager>(), Startup.Resolver.Resolve<AuthenticationManager>())
        {
        }

        public AlbumController(AlbumManager albumManager, AuthenticationManager authManager)
        {
            _albumManager = albumManager;
            _authManager = authManager;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var userId = Request.GetUserId();

            if(userId < 0)
                return Unauthorized();
            
            var albums = _albumManager.GetUserAlbums(userId);

            return Ok(albums.Select(Mapper.Map<AlbumViewModel>));
        }

        [HttpGet]
        [Route("api/album/discovery")]
        public IHttpActionResult Discovery()
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var albums = _albumManager.GetAll().Where(a => a.Privacy == AlbumPrivacy.Public);

            return Ok(albums.Select(Mapper.Map<AlbumViewModel>));
        }

        [HttpGet]
        [Route("api/album/{id}")]
        public IHttpActionResult GetById(int id)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var album = _albumManager.GetById(id);

            if (album == null)
                return NotFound();

            if (!_authManager.HasAccess(userId, album, Operation.Read))
                return Unauthorized();

            return Ok(Mapper.Map<AlbumViewModel>(album));
        }

        [HttpPost]
        public IHttpActionResult Create([FromBody]AlbumModel model)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var album = Mapper.Map<Album>(model);

            album.RowState = RowState.Created;
            album.CreatedBy = userId;
            album.CreatedAt = DateTime.Now;

            _albumManager.Add(album);

            return Created($"/{album.Id}", Mapper.Map<AlbumViewModel>(album));
        }
        
        [HttpPut]
        [Route("api/album/{id}")]
        public IHttpActionResult Update(int id, [FromBody]AlbumModel model)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var album = _albumManager.GetById(id);

            if (album == null)
                return NotFound();
            
            if (!_authManager.HasAccess(userId, album, Operation.Update))
                return Unauthorized();

            album.Name = model.Name;
            album.Privacy = model.Privacy;

            _albumManager.Update(album);

            return Ok(album);
        }
        
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var album = _albumManager.GetById(id);

            if (album == null)
                return NotFound();

            if (!_authManager.HasAccess(userId, album, Operation.Delete))
                return Unauthorized();

            _albumManager.Delete(album);

            return Ok();
        }
    }
}