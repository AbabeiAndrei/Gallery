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
using Gallery.Extensions;
using Gallery.Managers;
using Gallery.Models;

namespace Gallery.Controllers
{
    [Route("Gallery/album")]
    public class AlbumController : ApiController
    {
        public const int MAX_NUMBER_PHOTOS_IN_DISCOVERY = 5;

        private readonly AlbumManager _albumManager;
        private readonly AuthenticationManager _authManager;
        private readonly UserManager _userManager;
        private readonly PhotoManager _photoManager;
        private readonly FileManager _fileManager;

        public AlbumController() 
            : this(Startup.Resolver.Resolve<AlbumManager>(), Startup.Resolver.Resolve<AuthenticationManager>(),
                   Startup.Resolver.Resolve<UserManager>(), Startup.Resolver.Resolve<PhotoManager>(),
                   Startup.Resolver.Resolve<FileManager>())
        {
        }

        public AlbumController(AlbumManager albumManager, AuthenticationManager authManager, UserManager userManager, PhotoManager photoManager, FileManager fileManager)
        {
            _albumManager = albumManager;
            _authManager = authManager;
            _userManager = userManager;
            _photoManager = photoManager;
            _fileManager = fileManager;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var userId = Request.GetUserId();

            if(userId < 0)
                return Unauthorized();

            var user = _userManager.GetById(userId);

            if (user == null)
                return BadRequest();

            var albums = _albumManager.GetUserAlbums(userId).ToList();

            return Ok(MapAlbumToModel(albums, user));
        }

        [HttpGet]
        [Route("Gallery/album/discovery")]
        public IHttpActionResult Discovery()
        {
            var userId = Request.GetUserId();

            if (userId < 0)
                return Unauthorized();

            var user = _userManager.GetById(userId);

            if (user == null)
                return BadRequest();

            var albums = MapAlbumToModel(_albumManager.GetPublicAlbums(userId).ToList(), user);
                                      

            return Ok(albums);
        }

        private IEnumerable<DiscoveryAlbumViewModel> MapAlbumToModel(IEnumerable<Album> toList, IIdentity identity)
        {
            return toList.OrderByDescending(a => a.CreatedAt)
                         .Select(Mapper.Map<AlbumViewModel>)
                         .Select(avm =>
                         {
                             var photos = _photoManager.GetAlbumPhotos(avm.Id).ToList()
                                                       .Where(p => p.HasAccess(identity, Operation.Read, avm))
                                                       .OrderByDescending(p => p.UploadedAt)
                                                       .Select(Mapper.Map<PhotoViewModel>)
                                                       .Select(pvm => new DiscoveryPhotoViewModel(pvm)
                                                       {
                                                           Url = _fileManager.GetById(pvm.FileId)?.ThumbnailPath
                                                       })
                                                       .ToList();

                             const int maxItems = MAX_NUMBER_PHOTOS_IN_DISCOVERY;

                             return new DiscoveryAlbumViewModel(avm)
                             {
                                 ProfilePicture = "http://gallery.code40.local/app/resources/images/fbpic.jpg",
                                 UserName = _userManager.GetById(avm.CreatedBy)?.FullName,
                                 PhotoCount = photos.Count,
                                 Photos = photos.OnItem(maxItems, (pvm, count) => pvm.OtherX = count - maxItems)
                                                .Take(MAX_NUMBER_PHOTOS_IN_DISCOVERY)
                                                .ToList()
                             };
                         });
        }

        [HttpGet]
        [Route("Gallery/album/{id}")]
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

            var user = _userManager.GetById(userId);

            if (user == null)
                return BadRequest();

            var albumViewModel = Mapper.Map<AlbumViewModel>(album);

            var albumMap = new DiscoveryAlbumViewModel(albumViewModel)
            {
                Photos = _photoManager.GetAlbumPhotos(albumViewModel.Id).ToList()
                                      .Where(p => p.HasAccess(user, Operation.Read, albumViewModel))
                                      .OrderByDescending(p => p.UploadedAt)
                                      .Select(Mapper.Map<PhotoViewModel>)
                                      .Select(pvm => new DiscoveryPhotoViewModel(pvm)
                                      {
                                          Url = _fileManager.GetById(pvm.FileId)?.ThumbnailPath
                                      })
                                      .ToList()
            };
            
            return Ok(albumMap);
        }

        [HttpPost]
        public IHttpActionResult Create([FromBody]AlbumPostModel model)
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

            foreach (var photoModel in model.Photos)
            {
                var photo = Mapper.Map<Photo>(photoModel);

                photo.AlbumId = album.Id;
                photo.RowState = RowState.Created;
                photo.UploadedAt = DateTime.Now;
                photo.UploadedBy = userId;

                _photoManager.Add(photo);
            }

            return Created($"/{album.Id}", Mapper.Map<AlbumViewModel>(album));
        }
        
        [HttpPut]
        [Route("Gallery/album/{id}")]
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
        [Route("Gallery/album/{id}")]
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

            _albumManager.Delete(album.Id);

            return Ok();
        }
    }
}