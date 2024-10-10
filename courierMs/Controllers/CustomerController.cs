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
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Plugins;

namespace courierMs.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly courierMsContext _context;
        IWebHostEnvironment _environment;

        public CustomerController(courierMsContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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

        //showlist for admin
        public IActionResult ShowList(Guid senderId,Guid receiverId)
        {

            var senderData = _context.Customerinfo.FirstOrDefault(x => x.CustomerId == senderId);
            var receiverData = _context.Customerinfo.FirstOrDefault(x => x.CustomerId == receiverId);
            var percelData = _context.Percelinfo.FirstOrDefault(x=>x.SenderId== senderId);


            if(senderData!=null && receiverData != null)
            {
                var Data = new MultiModelVM
                {
                    Customerinfo = new CustomerinfoVM
                    {
                        S_Name = senderData.Name,
                        S_PhoneNumber = senderData.PhoneNumber,
                        S_city = senderData.city,
                        S_Address = senderData.Address,
                        S_Email = senderData.Address,
                        S_Note = senderData.Note,
                        R_Name = receiverData.Name,
                        R_PhoneNumber = receiverData.PhoneNumber,
                        R_city = receiverData.city,
                        R_Address = receiverData.Address,
                        R_Email = receiverData.Address,
                        R_Note = receiverData.Note


                    },
                    Percelinfo = new PercelinfoVM
                    {
                        ParcelType = percelData.ParcelType,
                        Weight = percelData.Weight,
                        Price = percelData.Price,
                        Note = percelData.Note,
                        SenderId = percelData.SenderId,
                        ReceiverId = percelData.ReceiverId


                    }
                };
                return View(Data);
            }

            return View();

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


        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleType.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MultiModelVM model)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customerinfo senderData = new Customerinfo();
            Customerinfo receiverData = new Customerinfo();
            Percelinfo percelData = new Percelinfo();


            //phoneNumber check
            //var exCustomer = _context.Customerinfo.Where(x => x.PhoneNumber == model.Customerinfo.PhoneNumber).Any(); // for using any() because it will return true false


            // for customer 
            senderData.CustomerId = Guid.NewGuid();
            senderData.Name = model.Customerinfo.S_Name;
            senderData.PhoneNumber = model.Customerinfo.S_PhoneNumber;
            senderData.Address = model.Customerinfo.S_Address;
            senderData.Email = model.Customerinfo.S_Email;
            senderData.city = model.Customerinfo.S_city;
            senderData.Note = model.Customerinfo.S_Note;
            senderData.CreatedAt = DateTime.Now;
            senderData.UpdatedAt = DateTime.Now;
            senderData.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            senderData.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);


            // for receiver
            receiverData.CustomerId = Guid.NewGuid();
            receiverData.Name = model.Customerinfo.R_Name;
            receiverData.PhoneNumber = model.Customerinfo.R_PhoneNumber;
            receiverData.Address = model.Customerinfo.R_Address;
            receiverData.Email = model.Customerinfo.R_Email;
            receiverData.city = model.Customerinfo.R_city;
            receiverData.Note = model.Customerinfo.R_Note;
            receiverData.CreatedAt = DateTime.Now;
            receiverData.UpdatedAt = DateTime.Now;
            receiverData.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            receiverData.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);


            // for percel
            percelData.ParcelType = model.Percelinfo.ParcelType;
            percelData.Weight = model.Percelinfo.Weight;
            percelData.Price = model.Percelinfo.Price;
            percelData.Note = model.Percelinfo.Note;
            percelData.CreatedAt = DateTime.Now;
            percelData.UpdatedAt = DateTime.Now;
            percelData.UpdatedAt = DateTime.Now;
            percelData.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            percelData.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);
            percelData.ParcelId = Guid.NewGuid();



            if (ModelState.IsValid)
            {
                _context.Add(senderData);
                await _context.SaveChangesAsync();


                _context.Add(receiverData);
                await _context.SaveChangesAsync();

                percelData.SenderId = senderData.CustomerId;
                percelData.ReceiverId = receiverData.CustomerId;

                _context.Add(percelData);
                await _context.SaveChangesAsync();


                return RedirectToAction("ShowList", new
                {
                    senderId = senderData.CustomerId,
                    receiverId = receiverData.CustomerId
                });

            }
            ViewBag.CityList = _context.Lookup
                .Where(x=> x.Type== LookupType.City && x.Serial>0)
                .OrderBy(x=> x.Serial)
                .ToList();

            ViewBag.PercelList = _context.Lookup
                .Where(x=> x.Type== LookupType.Percel && x.Serial>0)
                .OrderBy(x=> x.Serial)
                .ToList();


            


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


        //upload image
        public IActionResult addproduct()
        {
            ViewBag.Plist = _context.Product.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult addproduct(ProductVM model)
        {
            string filename = "";
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (model.Image != null)
            {
                string uploadfolder = Path.Combine(_environment.WebRootPath, "uploadimages");
                filename = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filepath = Path.Combine(uploadfolder, filename);
                model.Image.CopyTo(new FileStream(filepath, FileMode.Create));
            }

            Product product = new Product();
            product.Name = model.Name;
            product.Image = filename;
            product.price = model.price;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            product.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);

            if (ModelState.IsValid) 
            {
                _context.Product.Add(product);
                _context.SaveChanges();

            }

            ViewBag.Plist = _context.Product.ToList();
            ViewBag.success = "Record Add";
            return View(model);
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
