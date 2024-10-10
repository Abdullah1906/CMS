using courierMs.Data;
using courierMs.DataModel;
using courierMs.Models;
using courierMs.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace courierMs.Controllers
{
    public class ImageController : Controller
    {
        private readonly courierMsContext _context;
        IWebHostEnvironment _environment;

        public ImageController(courierMsContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateLookup(ProductVM model)
        {
            if (model == null)
                return Json(new { success = false, message = PopupMessage.error });

            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.CreatedAt = DateTime.Now;
            model.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);

            
            Product data = new Product();
            data.Name = model.Name;


            if (model.Image != null && model.Image.Length > 0)
            {
                // Define the path where the file will be saved
                var fileName = Path.GetFileName(model.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(stream);
                }

                // Store the relative file path to save in the database
                data.Image = $"/images/{fileName}";
            }
            else
            {
                return Json(new { success = false, message = "Image is required." });
            }
            data.price = model.price;
            data.CreatedBy = model.CreatedBy;
            data.UpdatedAt = model.UpdatedAt;
            data.UpdatedBy = model.UpdatedBy;
            data.CreatedAt = model.CreatedAt;



            try
            {
                _context.Product.Add(data);// for Lookup table
                _context.SaveChanges();



                var submitdata = _context.Product.Where(x => x.Id == data.Id).ToList();
                return Json(new { success = true, message = PopupMessage.success, data = submitdata });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = PopupMessage.error });
            }


        }


        [HttpPost]
        public IActionResult DeleteLookup(int Id)
        {
            if (Id <= 0)
                return Json(new { success = false, message = PopupMessage.error });


            var data = _context.Product.Where(x => x.Id == Id).FirstOrDefault();

            if (data == null)
                return Json(new { success = false, message = PopupMessage.error });


            try
            {
                _context.Product.Remove(data);// for Lookup table
                _context.SaveChanges();
                return Json(new { success = true, message = PopupMessage.success });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = PopupMessage.error });
            }

        }




        //[HttpPost]
        //public IActionResult UpdateLookup(ProductVM model)
        //{
        //    if (model == null || model.Id <= 0)
        //        return Json(new { success = false, message = PopupMessage.error });



        //    var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    model.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
        //    model.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);



        //    var data = _context.Product.FirstOrDefault(x => x.Id == model.Id);

        //    if (data == null)
        //    {
        //        return Json(new { success = false, message = PopupMessage.error });
        //    }

        //    data.Name = model.Name;

        //    if (model.Image != null && model.Image.Length > 0)
        //    {
        //        // Define the path where the file will be saved
        //        var fileName = Path.GetFileName(model.Image.FileName);
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

        //        // Save the file to the server
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            model.Image.CopyTo(stream);
        //        }

        //        // Store the relative file path to save in the database
        //        data.Image = $"/images/{fileName}";
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "Image is required." });
        //    }
        //    data.price = model.price;
        //    data.CreatedBy = model.CreatedBy;
        //    data.UpdatedAt = model.UpdatedAt;
        //    data.UpdatedBy = model.UpdatedBy;
        //    data.CreatedAt = model.CreatedAt;

        //    try
        //    {
        //        _context.Product.Update(data);// for Lookup table
        //        _context.SaveChanges();



        //        return Json(new { success = true, message = PopupMessage.success, data });

        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = PopupMessage.error });
        //    }


        //}



        //public IActionResult GetUpdateLookup(int Id)
        //{
        //    if (Id <= 0)
        //        return Json(new { success = false, message = PopupMessage.error });


        //    var imagedata = _context.Product.FirstOrDefault(x => x.Id == Id);

        //    if (imagedata == null)
        //        return Json(new { success = false, message = PopupMessage.error });

        //    return Json(new
        //    {
        //        success = true,
        //        message = PopupMessage.success,

        //        data = new
        //        {
        //            imagedata.Id,
        //            imagedata.Name,
        //            imagedata.Image,
        //            imagedata.price,

        //        }



        //    });

        //}
    }
}
