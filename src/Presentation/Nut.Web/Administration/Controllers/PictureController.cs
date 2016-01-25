using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Nut.Services.Media;
using Newtonsoft.Json;
using Nut.Web.Framework.Security;
using System.Collections.Generic;
using Nut.Web.Framework.Kendoui;
using System.Linq;

namespace Nut.Admin.Controllers {
    public partial class PictureController : BaseAdminController {
        private readonly IPictureService _pictureService;

        public PictureController(IPictureService pictureService) {
            this._pictureService = pictureService;
        }

        [HttpPost]
        //do not validate request token (XSRF)
        [AdminAntiForgery(true)]
        public ActionResult AsyncUpload(string sessionId) {
            //if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
            //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"])) {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            } else {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            if (String.IsNullOrEmpty(contentType)) {
                switch (fileExtension) {
                    case ".bmp":
                        contentType = "image/bmp";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".tiff":
                    case ".tif":
                        contentType = "image/tiff";
                        break;
                    default:
                        break;
                }
            }

            var picture = _pictureService.InsertPicture(fileBinary, contentType, fileName, sessionId);
            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog

            //return "{ \"files\":[{\"name\":\"" + picture.SeoFilename
            //    + "\",\"size\":" + fileBinary.Length + ",\"type\":\"" + contentType + "\",\"url\":\"" + _pictureService.GetPictureUrl(picture, 100)
            //    + "\",\"thumbnailUrl\":\"" + _pictureService.GetPictureUrl(picture, 100) + "\",\"deleteType\":\"GET\",\"deleteUrl\":\"/Admin/Picture/AsyncDelete?id=" + picture.Id + "\"}]}";

            return Json(new {
                success = true, pictureId = picture.Id,
                imageUrl = _pictureService.GetPictureUrl(picture, 100)
            },
                "text/plain");
        }


        [HttpPost]
        //do not validate request token (XSRF)
        [AdminAntiForgery(true)]
        public ActionResult AsyncList(string sessionId) {

            var pictures = _pictureService.GetPictures(sessionId: sessionId);

            var gridModel = new DataSourceResult {
                Data = pictures.Select(x => {
                    return new {
                        Name = x.SeoFilename,
                        SessionId = x.SessionId,
                        Id = x.Id,
                        MimeType = x.MimeType,
                        Url = _pictureService.GetPictureUrl(x, 200)
                    };
                }),
                Total = pictures.TotalCount
            };

            return Json(gridModel);

        }

        //do not validate request token (XSRF)
        [AdminAntiForgery(true)]
        public void AsyncDelete(int id) {
            var pricutre = _pictureService.GetPictureById(id);
            if (pricutre != null) {
                _pictureService.DeletePicture(pricutre);
            }
        }
    }

}