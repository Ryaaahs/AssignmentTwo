using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Data;
using EntityFramework.Models;
using EntityFramework.Models.ViewModels;

namespace AssignmentTwo.Controllers
{
    /**
     * BrokeragesController
     * Contains all the route bindings for the Brokerage pages
     * INDEX
     * ADS
     * DETAILS
     * CREATE
     * CREATE (POST)
     * EDIT
     * EDIT (POST)
     * DELETE
     * DELETE (POST)
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class BrokeragesController : Controller
    {
        private readonly MarketDbContext _context;

        /**
         * BrokeragesController
         * @param context Database content we use to make the assoicate with our Entities to the DB
         */
        public BrokeragesController(MarketDbContext context)
        {
            _context = context;
        }

        /**
         * Index
         * Main view in the Brokerage
         * 
         * @param id? Brokerage ID
         * @returns BrokeragesViewModel allows us to access the brokerages and subscriptions
         * If the optional param is provided, we include the clients assoicated with the brokerage
         */
        // GET: Brokerages
        public async Task<IActionResult> Index(string? ID)
        {
            var viewModel = new BrokeragesViewModel
            {
                Brokerages = await _context.Brokerage
                  .Include(i => i.Subscriptions)
                  .AsNoTracking()
                  .OrderBy(i => i.Title)
                  .ToListAsync(),

                Subscriptions = await _context.Subscription
                  .Include(i => i.Brokerage)
                  .Include(i => i.Client)
                  .AsNoTracking()
                  .ToListAsync()
            };

            if (ID != null)
            {
                viewModel.Clients = viewModel.Subscriptions.
                    Where(x => x.BrokerageId == ID).Select(x => new Client 
                    { 
                        FirstName = x.Client.FirstName,
                        LastName = x.Client.LastName,
                        BirthDate = x.Client.BirthDate,
                    });
            }

            return View(viewModel);
        }

        /**
         * Ads
         * Gives the user the option to create/remove Brokerage Advertisements
         * 
         * @param id Brokerage ID
         * @returns Redirects the user to the Ads index page
         */
        public IActionResult Ads(string id)
        {
            return RedirectToAction("Index", "Advertisements", new { id = id });
        }

        /**
         * Details
         * Gives the user the option to view an exisiting Brokerage (Name, Price)
         * 
         * @param id Brokerage ID
         * @returns Sends the user to the Brokerage information page (Name, Price)
         */
        // GET: Brokerages/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Brokerage == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brokerage == null)
            {
                return NotFound();
            }

            return View(brokerage);
        }

        /**
         * Create
         * Allows the user to create a new Brokerage
         * 
         * @returns Sends the user to the create view, to create a new brokerage
         */
        // GET: Brokerages/Create
        public IActionResult Create()
        {
            return View();
        }

        /**
         * Create (POST)
         * Validates the entity before creating the Brokerage
         * 
         * @param BIND("Id,Title,Fee") Binds these fields to the brokerage entity
         * @returns Creates the new brokerage and returns the user to the create page
         */
        // POST: Brokerages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] Brokerage brokerage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brokerage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brokerage);
        }

        /**
         * Edit
         * Gives the user the option to modifiy an exisiting brokerage (Name, Price) 
         * 
         * @param id Brokerage ID
         * @returns Sends the user to the edit page, to allow them to make modifications to the selected brokerage
         */
        // GET: Brokerages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Brokerage == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerage.FindAsync(id);
            if (brokerage == null)
            {
                return NotFound();
            }
            return View(brokerage);
        }


        /**
         * Edit (POST)
         * Validates the entity before making the modification
         * 
         * @param id Brokerage ID
         * @param BIND("Id,Title,Fee") Binds these fields to the brokerage entity
         * @returns Validates the user modifications, if allowed, it will make the changes and send them back to the edit page
         */
        // POST: Brokerages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] Brokerage brokerage)
        {
            if (id != brokerage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brokerage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrokerageExists(brokerage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brokerage);
        }

        /**
         * Delete
         * Gives the user the option to remove a brokerage
         * 
         * @param id Brokerage ID
         * @returns Sends the user to the Delete page, to remove a brokerage from the application
         */
        // GET: Brokerages/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Brokerage == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brokerage == null)
            {
                return NotFound();
            }

            return View(brokerage);
        }

        /**
         * Delete (POST)
         * Validates that the brokerage exists before removing it
         * 
         * @param id Brokerage ID
         * @returns Sends the user back to the Brokerage index page
         */
        // POST: Brokerages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Brokerage == null)
            {
                return Problem("Entity set 'MarketDbContext.Brokerage'  is null.");
            }
            var brokerage = await _context.Brokerage.FindAsync(id);
            if (brokerage != null)
            {
                _context.Brokerage.Remove(brokerage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrokerageExists(string id)
        {
          return _context.Brokerage.Any(e => e.Id == id);
        }
    }
}
