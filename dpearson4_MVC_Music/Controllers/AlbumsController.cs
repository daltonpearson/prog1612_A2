using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dpearson4_MVC_Music.Data;
using dpearson4_MVC_Music.Models;

namespace dpearson4_MVC_Music.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly MusicContext _context;

        public AlbumsController(MusicContext context)
        {
            _context = context;
        }

       


        // GET: Albums
        public async Task<IActionResult> Index()
        {
            var musicContext = _context.Albums.Include(a => a.Genre);
            return View(await musicContext.ToListAsync());
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            ViewData["GenreID"] = new SelectList(_context.Genres.OrderBy(x => x.Name), "ID", "Name");
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,YearProduced,Price,GenreID")] Album album)
        {
            if (TitleYearDuplicate(album.Name, album.YearProduced))
            {
                ModelState.AddModelError("", "Unable to add album. Title and Year Produced must be unique.");

                ViewData["GenreID"] = new SelectList(_context.Genres.OrderBy(x => x.Name), "ID", "Name", album.GenreID);
                return View(album);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(album);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["GenreID"] = new SelectList(_context.Genres, "ID", "Name", album.GenreID);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to add album. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(album);
        }
                
        

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            ViewData["GenreID"] = new SelectList(_context.Genres.OrderBy(x => x.Name), "ID", "Name", album.GenreID);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,YearProduced,Price,GenreID")] Album album)
        {

            if (TitleYearDuplicate(album.Name, album.YearProduced, id))
            {
                ModelState.AddModelError("", "Unable to save changes. Title and Year Produced must be unique.");
                ViewData["GenreID"] = new SelectList(_context.Genres.OrderBy(x => x.Name), "ID", "Name", album.GenreID);
                return View(album);
            }

            if (id != album.ID)
            {
                return NotFound();
            }

            var albumToUpdate = await _context.Albums.SingleOrDefaultAsync(s => s.ID == id);

            if (await TryUpdateModelAsync<Album>(albumToUpdate, "",
                a => a.Name, a => a.YearProduced, a => a.Price, a => a.GenreID))
            {
                try
                {
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreID"] = new SelectList(_context.Genres.OrderBy(x => x.Name), "ID", "Name", album.GenreID);
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (AlbumHasSongs(id))
            {
                ModelState.AddModelError("", "Unable to delete album. Album must not have any songs on it.");
                return RedirectToAction(nameof(Index));
            }

            var album = await _context.Albums.FindAsync(id);

            if (album == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.ID == id);
        }

        private bool TitleYearDuplicate(string title, string yearProduced, int? id = null)
        {
            return _context.Albums.Any(a => a.Name == title && a.YearProduced == yearProduced && a.ID != id);
        }

        private bool AlbumHasSongs(int id)
        {
            return _context.Songs.Any(s => s.AlbumID == id);
        }
    }
}
