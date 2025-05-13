using Azure.Storage.Blobs;
using EventeaseP7.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Azure.Storage.Blobs.Models;


namespace EventeaseP7.Controllers
{
    public class VenueController : Controller
    {
        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=amstorageag;AccountKey=fg/V35xNysYxkTSOyVRaA9T+dK2mYpgmJwRY8Y2MJfx8oPkQiDnvXSnQsGGifYDtzwl8gjk5nQtD+ASt36BYkw==;EndpointSuffix=core.windows.net";
            var containerName = "chickenjoe";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }

        private readonly ApplicationDbContext _context;

        public VenueController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var venue = await _context.Venues.ToListAsync();
            return View(venue);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null)
                {
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                    venue.Image_Url = blobUrl;
                }
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }



        // This method will return a form to delete a venue
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var venue = await _context.Venues
                .FirstOrDefaultAsync(v => v.Venue_ID == id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // This method will handle the deletion of the venue
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: Venue/Edit/{id}
        public async Task<IActionResult> Edit(int? id, Venue venue)
        {
            if (id != venue.Venue_ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);
                        venue.Image_Url = blobUrl;
                    }
                    else
                    {

                    }
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.Venue_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(venue);
            }
            if (id == null)
            {
                return NotFound();
            }

            var venueFromDb = await _context.Venues.FindAsync(id);

            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venue/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
        {
            if (id != venue.Venue_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.Venue_ID))
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
            return View(venue);
        }



        // This method will return the details of a venue
        // '?' means that the parameter is nullable, which means it can be null or have a value of type int.

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();  // it will return a 404 Not Found error if the id is null
            }

            //This will  find the venue with the specified id and include the events associated with the venue
            var venue = await _context.Venues
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.Venue_ID == id);

            if (venue == null)
            {
                return NotFound();  // it will return a 404 Not Found error if the venue is null
            }

            return View(venue);
        }

        // Check if the venue exists in the database
        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.Venue_ID == id);
        }
    }

}