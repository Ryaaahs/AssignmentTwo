
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Data;
using EntityFramework.Models;
using EntityFramework.Models.ViewModels;
using Azure.Storage.Blobs;

namespace AssignmentTwo.Controllers
{
    /**
     * AdvertisementsController
     * Contains all the route bindings for the Advertisement pages
     * INDEX
     * CREATE
     * CREATE (POST)
     * DELETE
     * DELETE (POST)
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class AdvertisementsController : Controller
    {
        private readonly MarketDbContext _context;
        private readonly String _blobConnectionString;
        BlobServiceClient _blobServiceClient;

        /**
         * AdvertisementsController
         * @param context Database content we use to make the assoicate with our Entities to the DB
         * @param configuration Allows use to get access to the AzureBlobStorage connectionString
         */
        public AdvertisementsController(MarketDbContext context, IConfiguration configuration)
        {
            _context = context;
            _blobConnectionString = configuration.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(this._blobConnectionString);
        }
        
        /**
         * Index
         * Main view in the Adverisement
         * 
         * @param id Brokerage ID
         * @returns AdsViewModel containing the brokerage selected and the advertiments tied to it
         */
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

        /**
         * Create
         * Allows the user to create new Advertisements
         * 
         * @param id Brokerage ID
         * @returns FileInputViewModel Passes in the Brokerage ID and Title to the view
         */
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

        /**
         * Create (POST)
         * Gathers all the data from the user and attempts to create a advertisement (Creating the blob, store the blob, 
         * making the connect to the blob with the entity)
         * 
         * @param file IFormFile that we get from sending the image through a POST request
         * @param BIND BrokerageId binds the brokerageID to the advertisement entity
         * @returns Redirect Sends the user back to the main Index page, along with the brokerageID
         * 
         * IF THERE IS AN ERROR: Redirect the user to the error page
         */
        // POST: Advertisements/Create
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
                return RedirectToAction("Error", "Error");
            }

            return RedirectToAction("Index", "Advertisements", new { id = brokerageId });
        }

        /**
         * Delete
         * Gives the user the option to remove Advertisements from a brokerage
         * 
         * @param int? Advertisement Id that we want to remove
         * @returns Advertisement model that we use within the Delete cshtml page
         * 
         */
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

            var brokerage = await _context.Brokerage
                  .Include(i => i.Advertisements)
                  .AsNoTracking()
                  .Where(m => m.Id == advertisement.BrokerageId)
                  .FirstOrDefaultAsync();

            advertisement.Brokerage = brokerage;

            return View(advertisement);
        }

        /**
         * Create (POST)
         * Removes the advertisement from the database (DB/AzureBlobStorage)
         * 
         * @param int? Advertisement Id that we want to remove
         * @returns Sends the user back to the Ads index page, along with the brokerageID to continue the process
         * 
         * IF THERE IS AN ERROR: Redirect the user to the error page
         */
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
                    return RedirectToAction("Error", "Error");
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
                    return RedirectToAction("Error", "Error");
                }
            }

            return RedirectToAction("Index", "Advertisements", new { id = brokerageId });
        }
    }
}
