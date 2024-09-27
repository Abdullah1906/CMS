using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using courierMs.Data;
using courierMs.DataModel;
using courierMs.Models;
using System.Security.Claims;
using courierMs.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace courierMs.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly courierMsContext _context;

        public CustomerController(courierMsContext context)
        {
            _context = context;
        }

        // GET: Customer
        [Authorize(Roles = RoleType.Admin)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customerinfo.ToListAsync());
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerinfo = await _context.Customerinfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerinfo == null)
            {
                return NotFound();
            }

            return View(customerinfo);
        }

        // GET: Customer/Create
        [Authorize(Roles = RoleType.Admin)]
        public IActionResult Create()
        {
            ViewBag.CityList = _context.Lookup
                .Where(x=> x.Type== LookupType.City && x.Serial>0)
                .OrderBy(x=> x.Serial)
                .ToList();

            ViewBag.PercelList = _context.Lookup
                .Where(x=> x.Type== LookupType.Percel && x.Serial>0)
                .OrderBy(x=> x.Serial)
                .ToList();

            return View();
        }

        [Authorize(Roles = RoleType.S_Admin)]
        public IActionResult CreateLookup()
        {
            ViewBag.List = _context.Lookup
                .Where(x=> x.Id>0)
                .OrderBy(x=>x.Serial)
                .ToList();

            var lastSerial = _context.Lookup
                .OrderByDescending(x => x.Serial)
                .Select(x => x.Serial)
                .FirstOrDefault();

            // Pass the last serial to ViewBag
            ViewBag.LastSerial = lastSerial;


            return View();
        }

        //get lastserial
        [HttpGet]
        public IActionResult GetLastSerial()
        {
            var lastSerial = _context.Lookup.OrderByDescending(x => x.Serial).Select(x => x.Serial).FirstOrDefault();

            return Json(new { lastSerial });
        }

        public ActionResult UploadImage()
        {
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleType.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MultiModelVM model)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customerinfo customerData = new Customerinfo();
            Percelinfo percelData = new Percelinfo();


            //phoneNumber check
            //var exCustomer = _context.Customerinfo.Where(x => x.PhoneNumber == model.Customerinfo.PhoneNumber).Any(); // for using any() because it will return true false


            // for customer 
            customerData.Name = model.Customerinfo.Name;
            customerData.PhoneNumber = model.Customerinfo.PhoneNumber;
            customerData.Address = model.Customerinfo.Address;
            customerData.Email = model.Customerinfo.Email;
            customerData.city = model.Customerinfo.city;
            customerData.Note = model.Customerinfo.Note;
            customerData.CreatedAt = DateTime.Now;
            customerData.UpdatedAt = DateTime.Now;
            customerData.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            customerData.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);
            customerData.CustomerId=Guid.NewGuid();

            // for receiver
            Receiverinfo receiverData = new Receiverinfo();

            receiverData.Name = model.Receiverinfo.Name;
            receiverData.PhoneNumber = model.Receiverinfo.PhoneNumber;
            receiverData.Address = model.Receiverinfo.Address;
            receiverData.Email = model.Receiverinfo.Email;
            receiverData.city = model.Receiverinfo.city;
            receiverData.Note = model.Receiverinfo.Note;
            receiverData.CreatedAt = DateTime.Now;
            receiverData.UpdatedAt = DateTime.Now;
            receiverData.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            receiverData.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);
            receiverData.ReceiverId = Guid.NewGuid();


            // for percel
            percelData.ParcelType = model.Percelinfo.ParcelType;
            percelData.Weight = model.Percelinfo.Weight;
            percelData.Price = model.Percelinfo.Price;
            percelData.Note = model.Percelinfo.Note;
            percelData.CreatedAt = DateTime.Now;
            percelData.UpdatedAt = DateTime.Now;
            customerData.UpdatedAt = DateTime.Now;
            percelData.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            percelData.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);
            percelData.ParcelId = Guid.NewGuid();



            if (ModelState.IsValid)
            {
                _context.Add(customerData);
                await _context.SaveChangesAsync();

                _context.Add(receiverData);
                await _context.SaveChangesAsync();

                percelData.SenderId = customerData.CustomerId;
                percelData.ReceiverId = receiverData.ReceiverId;

                _context.Add(percelData);
                await _context.SaveChangesAsync();

            }

            return View(model);
        }




        // CREATE LOOKUP
        [Authorize(Roles = RoleType.S_Admin)]
        [HttpPost]
        public IActionResult CreateLookup(LookupVM model)
        {
            if (model == null)
                return Json(new { success = false, message = PopupMessage.error });

            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.CreatedAt = DateTime.Now;
            model.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);


            Lookup data = new Lookup();
            data.Type = model.Type;
            data.Name = model.Name;
            data.Value = model.Value;
            data.Serial = model.Serial;
            data.IsActive = model.IsActive;
            data.CreatedBy = model.CreatedBy;
            data.UpdatedAt = model.UpdatedAt;
            data.UpdatedBy = model.UpdatedBy;
            data.CreatedAt = model.CreatedAt;



            try
            {
                _context.Lookup.Add(data);// for Lookup table
                _context.SaveChanges();
                


                var submitdata = _context.Lookup.Where(x => x.Id == data.Id).ToList();
                return Json(new { success = true, message = PopupMessage.success, data= submitdata });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = PopupMessage.error });
            }


        }

        // UPDATE 
        [HttpPost]
        public IActionResult UpdateLookup(LookupVM model)
        {
            if (model == null || model.Id <= 0)
                return Json(new { success = false, message = PopupMessage.error });



            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            model.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);



            var data = _context.Lookup.FirstOrDefault(x => x.Id == model.Id);

            if (data == null)
            {
                return Json(new { success = false, message = PopupMessage.error });
            }

            data.Name = model.Name;
            data.Value = model.Value;
            data.Type = model.Type;
            data.Serial = model.Serial;
            data.IsActive = model.IsActive;
            data.UpdatedAt = model.UpdatedAt;
            data.UpdatedBy = model.UpdatedBy;

            try
            {
                _context.Lookup.Update(data);// for Lookup table
                _context.SaveChanges();
                

                
                return Json(new { success = true, message = PopupMessage.success, data });

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = PopupMessage.error });
            }


        }

        public IActionResult GetUpdateLookup(int Id)
        {
            if (Id <= 0)
                return Json(new { success = false, message = PopupMessage.error });


            var lookupdata = _context.Lookup.FirstOrDefault(x => x.Id == Id);

            if (lookupdata == null)
                return Json(new { success = false, message = PopupMessage.error});

            return Json(new { success = true, message = PopupMessage.success ,

                data = new
                {
                    lookupdata.Id,
                    lookupdata.IsActive,
                    lookupdata.Name,
                    lookupdata.Serial,
                    lookupdata.Type,
                    lookupdata.Value
                }



            });

        }



        // delete
        [HttpPost]
        public IActionResult DeleteLookup(int Id)
        {
            if (Id <=0)
                return Json(new { success = false, message = PopupMessage.error });


            var data = _context.Lookup.Where(x => x.Id == Id).FirstOrDefault();

            if (data == null)
                return Json(new { success = false, message = PopupMessage.error });


            try
            {
                _context.Lookup.Remove(data);// for Lookup table
                _context.SaveChanges();
                return Json(new { success = true, message = PopupMessage.success });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = PopupMessage.error });
            }


           
        }




        // uploadfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {

                var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFile");

                // Check if the directory exists; if not, create it
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(uploadsFolderPath, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return Json(new { success = true, fileName = fileName });
            }

            return Json(new { success = false, message = "File upload failed." });
        }


        //// GET: Customer/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customerinfo = await _context.Customerinfo.FindAsync(id);
        //    if (customerinfo == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(customerinfo);
        //}

        //// POST: Customer/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,Name,PhoneNumber,Email,Address,Note,city,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy")] Customerinfo customerinfo)
        //{
        //    if (id != customerinfo.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(customerinfo);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CustomerinfoExists(customerinfo.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(customerinfo);
        //}

        //// GET: Customer/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customerinfo = await _context.Customerinfo
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (customerinfo == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(customerinfo);
        //}

        //// POST: Customer/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var customerinfo = await _context.Customerinfo.FindAsync(id);
        //    if (customerinfo != null)
        //    {
        //        _context.Customerinfo.Remove(customerinfo);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CustomerinfoExists(int id)
        //{
        //    return _context.Customerinfo.Any(e => e.Id == id);
        //}
    }
}
