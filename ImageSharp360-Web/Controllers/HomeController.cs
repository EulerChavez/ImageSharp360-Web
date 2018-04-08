using ImageSharp360.Imaging;
using ImageSharp360.Watermaking;
using ImageSharp360.Watermaking.Algorithm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageSharp360_Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string Image360Url, string Image360MarkedUrl)
        {
            ViewBag.Image360Url = Image360Url;
            ViewBag.Image360MarkedUrl = Image360MarkedUrl;

            return View();
        }

        public ActionResult Informacion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProcessImage(HttpPostedFileBase image360, HttpPostedFileBase watermark, float factor)
        {
            string serverUrl = Server.MapPath("~/Images");

            string Image360Url = string.Empty;
            string Image360MarkedUrl = string.Empty;

            factor = factor / 10;

            if (ModelState.IsValid)
            {
                // TODO: Validar tipo de archivo.

                if (image360 != null && watermark != null)
                {
                    Bitmap btmp = new Bitmap(image360.InputStream);

                    if (btmp.Width < 7776 || btmp.Height > 3888)
                    {
                        return RedirectToAction("Index");
                    }
                    else if (image360 != null && image360.ContentLength > 0)
                    {
                        try
                        {
                            // Imagen 360
                            string _360ImageName = Guid.NewGuid() + Path.GetExtension(image360.FileName);
                            string _360ImagePath = Path.Combine(serverUrl, _360ImageName);
                            image360.SaveAs(_360ImagePath);

                            // Watermark
                            string _watermarkName = Guid.NewGuid() + Path.GetExtension(watermark.FileName);
                            string _watermarkPath = Path.Combine(serverUrl, _watermarkName);
                            watermark.SaveAs(_watermarkPath);

                            // Procesamiento de la imagen
                            var _360Image = new Bitmap360(_360ImagePath);
                            var _watermark = new WatermarkBitmap(_watermarkPath);

                            Watermarking proceso = new Watermarking(_360Image, _watermark, new Factores(factor),
                                TissotIndicatrix.TopIndicatrix,
                                TissotIndicatrix.BottomIndicatrix,
                                TissotIndicatrix.FirstIndicatrix,
                                TissotIndicatrix.SecondIndicatrix,
                                TissotIndicatrix.ThirdIndicatrix,
                                TissotIndicatrix.FourthIndicatrix,
                                TissotIndicatrix.FifthIndicatrix,
                                TissotIndicatrix.SixthIndicatrix,
                                TissotIndicatrix.SeventhIndicatrix,
                                TissotIndicatrix.EighthIndicatrix,
                                TissotIndicatrix.NinthIndicatrix,
                                TissotIndicatrix.TenthIndicatrix,
                                TissotIndicatrix.EleventhIndicatrix,
                                TissotIndicatrix.TwelfthIndicatrix);

                            proceso.Prepare();

                            var nameResult = Guid.NewGuid().ToString() + Path.GetExtension(image360.FileName);
                            string resultPath = Path.Combine(serverUrl, nameResult);
                            var result = proceso.Apply();
                            result.Save(resultPath, ImageFormat.Jpeg);

                            // Resultados.
                            Image360Url = GetBaseImageUrl() + _360ImageName;
                            Image360MarkedUrl = GetBaseImageUrl() + nameResult;
                        }
                        catch (Exception ex) { }
                    }
                }
            }

            return RedirectToAction("Index", new { Image360Url, Image360MarkedUrl });
        }

        [NonAction]
        public string GetBaseImageUrl()
        {
            var request = HttpContext.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            var baseUrl = string.Format("{0}://{1}/Images/", request.Url.Scheme, request.Url.Authority);

            return baseUrl;
        }
    }
}