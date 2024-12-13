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
using courierMs.Services;
using courierMs.Migrations;
using Newtonsoft.Json;


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
        // customer tracking
        public IActionResult UpdateStatus()
        {
            return View();
        }
        // for assign rider
        public IActionResult Assign()
        {
            ViewBag.EmployeeList = _context.Lookup.Where(x => x.Type == LookupType.Rider).OrderBy(x=> x.Serial).ToList();

            var query = from percel in _context.Percelinfo
                        join sender in _context.Customerinfo on percel.SenderId equals sender.CustomerId
                        join receiver in _context.ReceiverInfo on percel.ReceiverId equals receiver.ReceiverId
                        select new TrackerinfoVM
                        {
                            Percelinfo = new PercelinfoVM
                            {
                                Id = percel.Id,
                                
                                ParcelType = percel.ParcelType,
                                Weight = percel.Weight,
                                Price = percel.Price,
                                
                            },
                            Customerinfo = new CustomerinfoVM
                            {
                                
                                Name = sender.Name,
                                PhoneNumber = sender.PhoneNumber,
                                
                                Address = sender.Address,
                                
                                city = sender.city
                            },
                            ReceiverInfo = new ReceiverInfoVM
                            {
                                
                                Name = receiver.Name,
                                PhoneNumber = receiver.PhoneNumber,
                                
                                Address = receiver.Address,
                                
                                city = receiver.city
                            },
                            

                        };
            var resultList = query.ToList();

            return View(resultList);

        }

        
        [HttpPost]
        public IActionResult Assign(int ParcelId, string EmployeeId)
        {
            var percel = _context.Percelinfo.Find(ParcelId);
            var employee = _context.Lookup.FirstOrDefault(x=>x.Value== EmployeeId);

            if (percel == null)
            {
                return NotFound();
            }

            percel.Status = Status.OnTheWay;
            percel.Rider = employee.Name;
            percel.TrackingNumber = employee.Value;
            _context.SaveChanges();
            return View();
        }

        //showlist for admin
        public IActionResult ShowList()
        {
            var query = from percel in _context.Percelinfo
                        join sender in _context.Customerinfo on percel.SenderId equals sender.CustomerId
                        join receiver in _context.ReceiverInfo on percel.ReceiverId equals receiver.ReceiverId
                        select new MultiModelVM
                        {
                            Percelinfo = new PercelinfoVM
                            {
                                Id = percel.Id,
                                ParcelId = percel.ParcelId,
                                ParcelType = percel.ParcelType,
                                Weight = percel.Weight,
                                Price = percel.Price,
                                Note = percel.Note,
                                SenderId = percel.SenderId,
                                ReceiverId = percel.ReceiverId
                            },
                            Customerinfo = new CustomerinfoVM
                            {
                                Id=sender.Id,
                                Name = sender.Name,
                                PhoneNumber = sender.PhoneNumber,
                                Email = sender.Email,
                                Address = sender.Address,
                                Note = sender.Note,
                                city = sender.city
                            },
                            ReceiverInfo = new ReceiverInfoVM
                            {
                                Id=receiver.Id,
                                Name = receiver.Name,
                                PhoneNumber = receiver.PhoneNumber,
                                Email = receiver.Email,
                                Address = receiver.Address,
                                Note = receiver.Note,
                                city = receiver.city
                            }
                        };

            var result = query.ToList(); 
            
                return View(result);
            
 

        }
        [HttpGet]
        
        public async Task<IActionResult> GetUpdate(int Id)
        {
            if (Id <= 0)
            {
                return Json(new { success = false, message = PopupMessage.error });
            }
            
                


            var Senderdata = await _context.Customerinfo.FirstOrDefaultAsync(x => x.Id == Id);
            var Receiverdata = await _context.ReceiverInfo.FirstOrDefaultAsync(x => x.Id == Id);
            var Perceldata = await _context.Percelinfo.FirstOrDefaultAsync(x => x.Id == Id);


            if (Senderdata == null && Receiverdata == null && Perceldata == null)
                return NotFound();

            var MultiModelVM = new MultiModelVM()
            {
                Customerinfo = new CustomerinfoVM
                {
                    Id = Senderdata.Id,
                    Name = Senderdata.Name,
                    PhoneNumber = Senderdata.PhoneNumber,
                    Email = Senderdata.Email,
                    Address = Senderdata.Address,
                    city = Senderdata.city,
                    Note = Senderdata.Note
                },
                ReceiverInfo = new ReceiverInfoVM
                {
                    Id = Receiverdata.Id,
                    Name = Receiverdata.Name,
                    PhoneNumber = Receiverdata.PhoneNumber,
                    Email = Receiverdata.Email,
                    Address = Receiverdata.Address,
                    city = Receiverdata.city,
                    Note = Receiverdata.Note
                },
                Percelinfo = new PercelinfoVM
                {
                    Id = Perceldata.Id,
                    ParcelType = Perceldata.ParcelType,
                    Note = Perceldata.Note,
                    Weight = Perceldata.Weight,
                    Price = Perceldata.Price
                }

            };

            ViewBag.CityList = _context.Lookup
               .Where(x => x.Type == LookupType.City && x.Serial > 0)
               .OrderBy(x => x.Serial)
               .ToList();

            ViewBag.PercelList = _context.Lookup
                .Where(x => x.Type == LookupType.Percel && x.Serial > 0)
                .OrderBy(x => x.Serial)
                .ToList();


            return View("Create", MultiModelVM);


        }
        [HttpGet]
        public IActionResult GetNumber(int query)
        {
            if (query <= 0)
                return Json(new { success = false, message = PopupMessage.error });

            var percelData = _context.Percelinfo.FirstOrDefault(x => x.Id == query);
            var customerData = _context.Customerinfo.FirstOrDefault(x => x.CustomerId == percelData.SenderId);
            var receiverData = _context.ReceiverInfo.FirstOrDefault(x => x.ReceiverId == percelData.ReceiverId);

            var submitData = new MultiModelVM
            {
                Percelinfo = new PercelinfoVM
                {
                   
                    Id = percelData.Id,
                    ParcelType = percelData.ParcelType,
                    Weight=percelData.Weight,
                    Price=percelData.Price,
                    Note=percelData.Note
                    
                },
                Customerinfo = new CustomerinfoVM
                {

                
                    Name = customerData?.Name,
                    PhoneNumber=customerData?.PhoneNumber,
                    Address=customerData?.Address,
                    Email=customerData?.Email

                },
                ReceiverInfo = new ReceiverInfoVM
                {

                    Name = receiverData?.Name,
                    PhoneNumber = receiverData?.PhoneNumber,
                    Address = receiverData?.Address,
                    Email = receiverData?.Email

                }
            };


            //MultiModelVM data = new MultiModelVM();



            return Json(new { success = true, message = PopupMessage.success, data = submitData });
        }
        [HttpGet]
        public IActionResult Invoice()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Invoice(ReportVM model)
        {
            return View();
                
        }
        public IActionResult Report(MultiModelVM model)
        {
            if (model == null)
                return Json(new { success = false, message = PopupMessage.error });

            if (model.IsPrint)
            {


                HttpContext.Session.Remove("InvoiceModel");

                HttpContext.Session.SetString("InvoiceModel", JsonConvert.SerializeObject(model));


                return Json(new { success = true, message = PopupMessage.success });
            }
            return Json(new { success = true, message = PopupMessage.success });
        }
        public IActionResult InvoicePrint()
        {

            var invoiceModelJson = HttpContext.Session.GetString("InvoiceModel");


            MultiModelVM model = JsonConvert.DeserializeObject<MultiModelVM>(invoiceModelJson);


            return View(model);
        }

        // GET: Customer/
        // 
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
        public async Task<IActionResult> CreateLookup(int? pageNumber)
        {


            var lastSerial = _context.Lookup
                .OrderByDescending(x => x.Serial)
                .Select(x => x.Serial)
                .FirstOrDefault();

            // Pass the last serial to ViewBag
            ViewBag.LastSerial = lastSerial;
            int pageSize = 5;
            var lookup = _context.Lookup.AsQueryable();
            var paginatedLookup = await Pagination<Lookup>.CreateAsync(lookup.OrderBy(x => x.Id), pageNumber ?? 1, pageSize);
            return View(paginatedLookup);


        }
        public async Task<IActionResult> LoadPage(int? pageNumber)
        {
            int pageSize = 5;
            var lookup = _context.Lookup.AsQueryable();
            var paginatedLookup = await Pagination<Lookup>.CreateAsync(lookup.OrderBy(x => x.Id), pageNumber ?? 1, pageSize);
            return PartialView("_lookupPartial", paginatedLookup);
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

            Customerinfo sender = new Customerinfo();
            ReceiverInfo receiver = new ReceiverInfo();
            Percelinfo percelData = new Percelinfo();
            Invoice InvoiceData = new Invoice();


            //phoneNumber check
            //var exCustomer = _context.Customerinfo.Where(x => x.PhoneNumber == model.Customerinfo.PhoneNumber).Any(); // for using any() because it will return true false
            //for all customer in single table



            // for customer sender and reciever
            sender.CustomerId = Guid.NewGuid();
            sender.Name = model.Customerinfo.Name;
            sender.PhoneNumber = model.Customerinfo.PhoneNumber;
            sender.Address = model.Customerinfo.Address;
            sender.Email = model.Customerinfo.Email;
            sender.city = model.Customerinfo.city;
            sender.Note = model.Customerinfo.Note;
            sender.CreatedAt = DateTime.Now;
            sender.UpdatedAt = DateTime.Now;
            sender.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            sender.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);



            receiver.ReceiverId = Guid.NewGuid();
            receiver.Name = model.ReceiverInfo.Name;
            receiver.PhoneNumber = model.ReceiverInfo.PhoneNumber;
            receiver.Address = model.ReceiverInfo.Address;
            receiver.Email = model.ReceiverInfo.Email;
            receiver.city = model.ReceiverInfo.city;
            receiver.Note = model.ReceiverInfo.Note;
            receiver.CreatedAt = DateTime.Now;
            receiver.UpdatedAt = DateTime.Now;
            receiver.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            receiver.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);



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

            // for Invoice
            InvoiceData.InvoiceId = model.Invoice.InvoiceId;
            InvoiceData.Email = model.Invoice.Email;
            InvoiceData.Date = model.Invoice.Date;
            InvoiceData.ParcelId = percelData.ParcelId;
            InvoiceData.CreatedAt = DateTime.Now;
            InvoiceData.UpdatedAt = DateTime.Now;
            InvoiceData.CreatedBy = GuidHelper.ToGuidOrDefault(userid);
            InvoiceData.UpdatedBy = GuidHelper.ToGuidOrDefault(userid);

            if (ModelState.IsValid)
            {
                _context.Add(sender);
                await _context.SaveChangesAsync();


                _context.Add(receiver);
                await _context.SaveChangesAsync();




                percelData.SenderId = sender.CustomerId;
                percelData.ReceiverId = receiver.ReceiverId;

                _context.Add(percelData);
                await _context.SaveChangesAsync();

                _context.Add(InvoiceData);
                await _context.SaveChangesAsync();


                //return RedirectToAction("ShowList", new
                //{
                //    senderId = sender.CustomerId,
                //    receiverId = receiver.CustomerId
                //});

            }
            ViewBag.CityList = _context.Lookup
                .Where(x => x.Type == LookupType.City && x.Serial > 0)
                .OrderBy(x => x.Serial)
                .ToList();

            ViewBag.PercelList = _context.Lookup
                .Where(x => x.Type == LookupType.Percel && x.Serial > 0)
                .OrderBy(x => x.Serial)
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
