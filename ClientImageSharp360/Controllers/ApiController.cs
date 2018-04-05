using ImageSharp360.Imaging;
using ImageSharp360.Watermaking;
using ImageSharp360.Watermaking.Algorithm;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ClientImageSharp360.Controllers
{
    public class ApiResponse
    {
        public bool Error { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class ApiController : Controller
    {
        public ActionResult Index()
        {
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ImageProcessing(HttpPostedFileBase image, HttpPostedFileBase watermark, float factor = 0.5F)
        {
            ApiResponse response = new ApiResponse
            {
                Error = true,
                Status = "failed",
                Message = "Información no presente"
            }; ;

            if (ModelState.IsValid)
            {
                // TODO: Validar tipo de archivo.

                if (image != null && watermark != null)
                {
                    Bitmap btmp = new Bitmap(image.InputStream);

                    if (btmp.Width < 7776 || btmp.Height > 3888)
                    {
                        response = new ApiResponse
                        {
                            Error = true,
                            Status = "failed",
                            Message = "No son correctas las dimenciones de la imagen."
                        };
                    }
                    else if (image != null && image.ContentLength > 0)
                    {
                        try
                        {
                            // Imagen 360
                            string _360ImageName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                            string _360ImagePath = Path.Combine(Server.MapPath("~/Images"), _360ImageName);
                            image.SaveAs(_360ImagePath);

                            // Watermark
                            string _watermarkName = Guid.NewGuid() + Path.GetExtension(watermark.FileName);
                            string _watermarkPath = Path.Combine(Server.MapPath("~/Images"), _watermarkName);
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

                            string resultPath = Path.Combine(Server.MapPath("~/Images"), Guid.NewGuid().ToString() + Path.GetExtension(image.FileName));
                            var result = proceso.Apply();
                            result.Save(resultPath, ImageFormat.Jpeg);

                            response = new ApiResponse
                            {
                                Error = false,
                                Status = "processed",
                                Message = resultPath
                            };
                        }
                        catch (Exception ex) { }
                    }
                }
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}