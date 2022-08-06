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
    public class ClientsController : Controller
    {
        private readonly MarketDbContext _context;

        public ClientsController(MarketDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(int? Id)
        {
            var viewModel = new ClientsBrokerageView { };

            if (Id == null)
            {
                viewModel = new ClientsBrokerageView
                {
                    CurrentClientBrokerages = null,
                    Clients = await _context.Client.ToListAsync(),
                };

                return View(viewModel);
            }

            var Clients = await _context.Client.AsNoTracking().ToListAsync();

            var Subscriptions = await _context.Subscription
                 .Include(i => i.Client)
                 .Include(i => i.Brokerage)
                 .AsNoTracking()
                 .Where(m => m.ClientId == Id)
                 .ToListAsync();

            List<String> brokerageTitles = new List<String>();

            Subscriptions.ForEach(x =>
            {
                brokerageTitles.Add(x.Brokerage.Title);
            });

            viewModel = new ClientsBrokerageView
            {
                CurrentClientBrokerages = brokerageTitles.Count == 0 ? null : brokerageTitles,
                Clients = Clients,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AddSubscription([Bind("ClientId,BrokerageId")] Subscription subscription)
        {
            // Add a subscription
            if (ModelState.IsValid)
            {
                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EditSubscriptions), new {id = subscription.ClientId});
            }
            return View(subscription);
        }

        public async Task<IActionResult> RemoveSubscription(int clientId, string brokerageId)
        {
            // Remove a subscription
            if (_context.Subscription == null)
            {
                return Problem("Entity set 'MarketDbContext.Subscription'  is null.");
            }
            var subscription = await _context.Subscription.Where(m => m.BrokerageId == brokerageId && m.ClientId == clientId).FirstAsync();
            if (subscription != null)
            {
                _context.Subscription.Remove(subscription);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EditSubscriptions), new { id = subscription.ClientId });
        }

        // GET: Clients/EditSubscriptions/5
        public async Task<IActionResult> EditSubscriptions(int Id)
        {
            return View(await getClientSubscriptions(Id));
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Client == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Client == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,BirthDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Client == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Client == null)
            {
                return Problem("Entity set 'MarketDbContext.Client'  is null.");
            }
            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
          return _context.Client.Any(e => e.Id == id);
        }

        private async Task<ClientSubscriptionsViewModel> getClientSubscriptions(int Id)
        {
            IList<BrokerageSubscriptionsViewModel> brokerageSubscriptionsViewModel = new List<BrokerageSubscriptionsViewModel>();
            List<string> currentBrokerageIds = new List<string>();
            Boolean addedSubscription = false;

            var Subscriptions = await _context.Subscription
                 .Include(i => i.Client)
                 .Include(i => i.Brokerage)
                 .AsNoTracking()
                 .Where(m => m.ClientId == Id)
                 .ToListAsync();

            var Brokerages = await _context.Brokerage
                 .Include(i => i.Subscriptions)
                 .Include(i => i.Advertisements)
                 .AsNoTracking()
                 .ToListAsync();

            var Client = await _context.Client.AsNoTracking().Where(m => m.Id == Id).FirstOrDefaultAsync();

            Brokerages.ForEach(x =>
            {
                Subscriptions.ForEach(y =>
                {
                    if (!currentBrokerageIds.Contains(y.BrokerageId) && x.Id == y.BrokerageId)
                    {
                        var viewModel = new BrokerageSubscriptionsViewModel
                        {
                            BrokerageId = y.Brokerage.Id,
                            Title = y.Brokerage.Title,
                            IsMember = true,
                        };
                        currentBrokerageIds.Add(y.Brokerage.Id);
                        brokerageSubscriptionsViewModel.Add(viewModel);
                        addedSubscription = true;
                    }
                });

                if (!addedSubscription)
                {
                    var viewModel = new BrokerageSubscriptionsViewModel
                    {
                        BrokerageId = x.Id,
                        Title = x.Title,
                        IsMember = false,
                    };
                    brokerageSubscriptionsViewModel.Add(viewModel);
                }
                else
                {
                    addedSubscription = false;
                }
            });

            var viewModel = new ClientSubscriptionsViewModel
            {
                Client = Client,
                Subscriptions = brokerageSubscriptionsViewModel,

            };

            return viewModel;
        }
    }
}
