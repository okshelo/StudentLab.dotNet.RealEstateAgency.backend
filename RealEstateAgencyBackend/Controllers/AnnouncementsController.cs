﻿using AutoMapper;
using Microsoft.Owin.Security;
using RealEstateAgencyBackend.BLL.DTO;
using RealEstateAgencyBackend.BLL.Interfaces;
using RealEstateAgencyBackend.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RealEstateAgencyBackend.Controllers
{
    public class AnnouncementsController : ApiController
    {
        private IMapper _mapper;
        private IRentalAnnouncementService _rentalAnnouncementService;
        private IUserService _userService;
        private IImageService _imageService;

        public AnnouncementsController(IRentalAnnouncementService rentalAnnouncementService, IUserService userService,IImageService imageService, IMapper mapper)
        {
            _rentalAnnouncementService = rentalAnnouncementService;
            _userService = userService;
            _mapper = mapper;
            _imageService = imageService;
        }


        [HttpGet]
        [Route("api/announcements")]
        public IEnumerable<RentalAnnouncementViewModel> GetRentalAnnouncements(int page,int pageSize)
        {
			//if(Request.QueryString.)
			IEnumerable<RentalAnnouncementDto> announcements;
			if (HttpContext.Current.Request.QueryString.Count > 0)
			{
				NameValueCollection filteringParams = HttpContext.Current.Request.QueryString;

				announcements = _rentalAnnouncementService.GetPageWithFilters(page, pageSize,filteringParams);
			}
			else 
			{
				announcements = _rentalAnnouncementService.GetAll();
			}
            IEnumerable<RentalAnnouncementViewModel> results = _mapper.Map<IEnumerable<RentalAnnouncementDto>, IEnumerable<RentalAnnouncementViewModel>>(announcements);
            return results;
        }

		[HttpGet]
		[Route("api/announcements/count")]
		public Int32 GetRentalAnnouncementsCount()
		{
			NameValueCollection filteringParams = HttpContext.Current.Request.QueryString;
			Int32 announcementsCount = _rentalAnnouncementService.GetPageCount(filteringParams);
			return announcementsCount;
		}


		[HttpGet]
        [Route("api/announcements/{id}")]
        [ResponseType(typeof(RentalAnnouncementViewModel))]
        public IHttpActionResult GetRentalAnnouncement(int id)
        {
            RentalAnnouncementDto rentalAnnouncement = _rentalAnnouncementService.Find(id);

            if (rentalAnnouncement == null)
                return NotFound();

            RentalAnnouncementViewModel view = _mapper.Map<RentalAnnouncementViewModel>(rentalAnnouncement);

            return Ok(view);
        }


        [Authorize]
        [HttpPost]
        [Route("api/announcements/create")]
        [ResponseType(typeof(RentalAnnouncementCreateModel))]
        public IHttpActionResult Create(RentalAnnouncementCreateModel rentalAnnouncementCreateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            //var folderPath = HttpContext.Current.Server.MapPath("~/Public/");
			var folderPath = "Public";
			var appFolderPath = HttpContext.Current.Server.MapPath("~/");

			ICollection<ImageDto> images = new List<ImageDto>();
            foreach (var img in rentalAnnouncementCreateModel.Base64Images)
            {
                images.Add(_imageService.Save(img, appFolderPath ,folderPath));
            }

            String userName = AuthManager.User.Identity.Name;
            String userId = _userService.GetUserId(userName);

            RentalAnnouncementDto rentalAnnouncement = _mapper.Map<RentalAnnouncementDto>(rentalAnnouncementCreateModel);//RentalAnnouncementCreateModel
            rentalAnnouncement.UserId = userId;
            rentalAnnouncement.Images = images;

            _rentalAnnouncementService.Create(rentalAnnouncement);

            return Created("", rentalAnnouncementCreateModel);
        }


        // PUT: api/RentalAnnouncements/5
        /*[ResponseType(typeof(void))]
        public IHttpActionResult UpdateRentalAnnouncement(int id, RentalAnnouncement rentalAnnouncement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rentalAnnouncement.Id)
            {
                return BadRequest();
            }

            db.Entry(rentalAnnouncement).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalAnnouncementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }*/


        // DELETE: api/RentalAnnouncements/5
        [Authorize]
        [HttpDelete]
        [Route("api/announcements/{id}")]
        [ResponseType(typeof(RentalAnnouncementViewModel))]
        public IHttpActionResult DeleteRentalAnnouncement(int id)
        {
            String userName = AuthManager.User.Identity.Name;
            String userId = _userService.GetUserId(userName);

            RentalAnnouncementDto announcement = _rentalAnnouncementService.Find(id);

            if (announcement.UserId != userId)
                return NotFound();

            RentalAnnouncementDto deletedAnnouncement = _rentalAnnouncementService.Remove(announcement);

            return Ok(_mapper.Map<RentalAnnouncementViewModel>(deletedAnnouncement));
        }
        

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

    }
}