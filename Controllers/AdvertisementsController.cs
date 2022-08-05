using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Data;
using EntityFramework.Models;
using EntityFramework.Models.ViewModels;
using Azure.Storage.Blobs;

namespace AssignmentTwo.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly MarketDbContext _context;
        private readonly String _blobConnectionString;
        BlobServiceClient _blobServiceClient;

        public AdvertisementsController(MarketDbContext context, IConfiguration configuration)
        {
            _context = context;
            _blobConnectionString = configuration.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(this._blobConnectionString);
        }

        // GET: Advertisements
        public async Task<IActionResult> Index(string id)
        {
            var viewModel = new AdsViewModel
            {
                Brokerage = await _context.Brokerage
                  .Include(i => i.Advertisements)
                  .AsNoTracking()
                  .Where(m => m.Id == id)
                  .FirstOrDefaultAsync(),

                Advertisements = await _context.Advestisement
                  .Include(i => i.Brokerage)
                  .AsNoTracking()
                  .Where(m => m.BrokerageId == id)
                  .ToListAsync()
            };

            return View(viewModel);
        }

        // GET: Advertisements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Advestisement == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advestisement
                .FirstOrDefaultAsync(m => m.Id == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }

        // GET: Advertisements/Create
        public async Task<IActionResult> Create(string id)
        {
            var Brokerage = await _context.Brokerage
                 .Include(i => i.Advertisements)
                 .AsNoTracking()
                 .Where(m => m.Id == id)
                 .FirstOrDefaultAsync();

            var viewModel = new FileInputViewModel
            {
                BrokerageId = Brokerage.Id,
                BrokerageTitle = Brokerage.Title,
            };

            return View(viewModel);
        }

        // POST: Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("BrokerageId")] Advertisement advertisement)
        {
            BlobContainerClient containerClient;
            var containerName = "brokerage" + advertisement.BrokerageId.ToLower();
            String brokerageId = advertisement.BrokerageId;

            // Create the container and return a container client object
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (Azure.RequestFailedException e)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }


            try
            {
                string randomFileName = Path.GetRandomFileName();
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(randomFileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                // add the photo to the database if it uploaded successfully
                var image = new Advertisement
                {
                    Url = blockBlob.Uri.AbsoluteUri,
                    FileName = randomFileName,
                    BrokerageId = advertisement.BrokerageId,
                };
                _context.Advestisement.Add(image);
                _context.SaveChanges();
            }
            catch (Azure.RequestFailedException)
            {
                return RedirectToPage("Index", new { id = advertisement.BrokerageId });
            }

            return RedirectToAction("Index", "Advertisements", new { id = brokerageId });
        }

        // GET: Advertisements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Advestisement == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advestisement.FindAsync(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }

        // POST: Advertisements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FileName,Url")] Advertisement advertisement)
        {
            if (id != advertisement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(advertisement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    /*if (!Advertisement.Exists(advertisement.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }*/
                }
                return RedirectToAction(nameof(Index));
            }
            return View(advertisement);
        }

        // GET: Advertisements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Advestisement == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advestisement
                .FirstOrDefaultAsync(m => m.Id == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }

        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Advestisement = await _context.Advestisement.FindAsync(id);
            String brokerageId = Advestisement.BrokerageId;

            if (Advestisement != null)
            {
                BlobContainerClient containerClient;
                try
                {
                    var containerName = "brokerage" + Advestisement.BrokerageId.ToLower();
                    containerClient = this._blobServiceClient.GetBlobContainerClient(containerName);
                }
                catch (Azure.RequestFailedException)
                {
                    return RedirectToPage("Error");
                }

                try
                {
                    // Get the blob that holds the data
                    var blockBlob = containerClient.GetBlobClient(Advestisement.FileName);
                    if (await blockBlob.ExistsAsync())
                    {
                        await blockBlob.DeleteAsync();
                    }

                    _context.Advestisement.Remove(Advestisement);
                    await _context.SaveChangesAsync();

                }
                catch (Azure.RequestFailedException)
                {
                    return RedirectToPage("Error");
                }
            }

            return RedirectToAction("Index", "Advertisements", new { id = brokerageId });
        }
    }
}
