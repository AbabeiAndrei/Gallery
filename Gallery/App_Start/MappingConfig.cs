using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Gallery.DataLayer.Entities;
using Gallery.Models;

namespace Gallery
{
    public static class MappingConfig
    {
        public static void CreateConfiguration(IMapperConfigurationExpression obj)
        {
            obj.CreateMap<Album, AlbumModel>().ReverseMap();
            obj.CreateMap<Album, AlbumViewModel>().ReverseMap();
            obj.CreateMap<Photo, PhotoModel>().ReverseMap();
            obj.CreateMap<Photo, PhotoViewModel>().ReverseMap();
            obj.CreateMap<File, FileViewModel>().ReverseMap();
        }
    }
}