using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Data;
using EntityFramework.Models;
using EntityFramework.Models.ViewModels;

namespace AssignmentTwo.Controllers
{
    /**
     * ClientsController
     * Contains all the route bindings for the Client pages
     * INDEX
     * ADDSUBSCRIPTION
     * REMOVESUBSCRIPTION
     * EDITSUBSCRIPTION
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
    public class ClientsController : Controller
    {
        private readonly MarketDbContext _context;

        /**
         * ClientsController
         * @param context Database content we use to make the assoicate with our Entities to the DB
         */
        public ClientsController(MarketDbContext context)
        {
            _context = context;
        }

        /**
         * Index
         * Main view in the Client
         * 
         * @param id? Brokerage ID
         * @returns List of clients to display on the index view
         * If the optional param is provided, we include the brokerages assoicated with the client
         */
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
                CurrentClientBrokerages = brokerageTitles,
                Clients = Clients,
            };

            return View(viewModel);
        }

        /**
         * AddSubscription
         * Allows the user to add additional brokerages to the clients
         * 
         * @param BIND("ClientId,BrokerageId")] Binds these values into subscription
         * @returns Sends the user back to the EditSubscription page once we create the new subscription
         */
        public async Task<IActionResult> AddSubscription([Bind("ClientId,BrokerageId")] Subscription subscription)
        {
            // Add a subscription
            if (ModelState.IsValid)
            {
                _context.Add(subscription);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(EditSubscriptions), new { id = subscription.ClientId });
        }

        /**
         * RemoveSubscription
         * Allows the user to remove brokerages in the clients
         * 
         * @param clientID Client Id
         * @param brokerageID Brokerage Id
         * @returns Sends the user back to the EditSubscription page once we remove the subscription
         */
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

        /**
         * EditSubscriptions
         * Gives the user the option to add/remove brokerage subscriptions to a specific client
         * 
         * @param Id Client Id
         * @returns Sends the user to the EditSubscription page, to allow them to add/remove brokerage subscriptions
         */
        // GET: Clients/EditSubscriptions/5
        public async Task<IActionResult> EditSubscriptions(int Id)
        {
            return View(await getClientSubscriptions(Id));
        }

        /**
         * EditSubscriptions
         * Gives the user the option to add/remove brokerage subscriptions to a specific client
         * 
         * @param Id Client Id
         * @returns Sends the user to the EditSubscription page, to allow them to add/remove brokerage subscriptions
         */
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

        /**
         * Create
         * Gives the user the option to add new clients to the system
         * 
         * @returns Create view
         */
        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        /**
         * Create (POST)
         * Gives the user the option to add new clients to the system
         * 
         * @param BIND("Id,FirstName,LastName,BirthDate") Binds these values into client
         * @returns Client Index view // Create view
         */
        // POST: Clients/Create
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

        /**
         * Edit
         * Gives the user the option to edit an exisiting client
         * 
         * @param id? Client Id
         * @returns Client Edit view
         */
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

        /**
         * Edit (POST)
         * Validates the user values before making the modification to the client
         * 
         * @param id Client Id
         * @param BIND("Id,FirstName,LastName,BirthDate") Binds these values into client
         * @returns Client Edit view // Client Index view
         */
        // POST: Clients/Edit/5
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

        /**
         * Delete 
         * Gives the user the option to remove a client from the system
         * 
         * @param id? Client Id
         * @returns Client Delete view
         */
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

        /**
         * Delete (POST)
         * Removes the client from the system (DB)
         * 
         * @param id Client Id
         * @returns Client Index view
         */
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

            // Used to setup the brokerageSubscriptionsViewModel
            List<string> currentBrokerageIds = new List<string>();
            Boolean addedSubscription = false;

            // Grab all the needed entites
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

            // || How this logic works ||
            // Interate through all the brokerages, then compare each of the subscriptions aganist it.
            // If we don't have the id already saved, and the IDs match. Create the viewModel and add the ID to the list collection
            // Most likely a better way to implement this, but I'm tight on time :^)
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

                // If we don't add any subscriptions through that cycle. It means that we are not subscripted to the brokerage
                // So we can add the brokerage and set the memeber status to false.
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
                    // Reset the value
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
