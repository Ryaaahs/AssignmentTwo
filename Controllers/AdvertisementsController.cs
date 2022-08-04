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
    public class AdvertisementsController : Controller
    {
        private readonly MarketDbContext _context;

        public AdvertisementsController(MarketDbContext context)
        {
            _context = context;
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
                  .OrderBy(i => i.Title)
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileName,Url")] Advertisement advertisement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(advertisement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(advertisement);
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
                    if (!AdvertisementExists(advertisement.Id))
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Advestisement == null)
            {
                return Problem("Entity set 'MarketDbContext.Advestisement'  is null.");
            }
            var advertisement = await _context.Advestisement.FindAsync(id);
            if (advertisement != null)
            {
                _context.Advestisement.Remove(advertisement);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdvertisementExists(int id)
        {
          return _context.Advestisement.Any(e => e.Id == id);
        }
    }
}
