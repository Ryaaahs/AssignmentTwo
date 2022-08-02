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

namespace AssignmentTwo.Controllers
{
    public class BrokeragesController : Controller
    {
        private readonly MarketDbContext _context;

        public BrokeragesController(MarketDbContext context)
        {
            _context = context;
        }

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

               /* viewModel.Clients = viewModel.Clients.
                    Where(x => x.Id = subslect.ClientId);*/
            }

            return View(viewModel);
        }

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

        // GET: Brokerages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brokerages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // POST: Brokerages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
