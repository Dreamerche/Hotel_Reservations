using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Rsesrvations.Data;
using Hotel_Rsesrvations.Models;

namespace Hotel_Rsesrvations.Controllers
{
    public class ReservationClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary> Отваряне на таблица със записи за резервация-клиент връзка
        /// </summary>
        // GET: ReservationClients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ReservationClients.Include(r => r.Client).Include(r => r.Reservation);
            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary> Извеждане на детайли за дадена резервация-клиент връзка
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationClient = await _context.ReservationClients
                .Include(r => r.Client)
                .Include(r => r.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationClient == null)
            {
                return NotFound();
            }

            return View(reservationClient);
        }

        /// <summary> Създаване на резервация-клиент връзка
        /// </summary>
        // GET: ReservationClients/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "firstName");
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id");
            return View();
        }

        /// <summary> Добавяне на резервация-клиент връзка
        /// </summary>
        // POST: ReservationClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReservationId,ClientId")] ReservationClient reservationClient)
        {

            int peopleReservatedForTRheRoom = await _context.ReservationClients
    .CountAsync(rc => rc.ReservationId == reservationClient.ReservationId);
            var reservation = await _context.Reservations
               .Include(r => r.Room)
               .FirstOrDefaultAsync(m => m.Id == reservationClient.ReservationId);
            var room = reservation.Room;
            if (room.capacity <= peopleReservatedForTRheRoom)
            {
                ModelState.AddModelError("ReservationId", " The room is already fully booked.");
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "firstName", reservationClient.ClientId);
                ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", reservationClient.ReservationId);
                return View(reservationClient);
            }
            if (ModelState.IsValid)
            {
                _context.Add(reservationClient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "firstName", reservationClient.ClientId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", reservationClient.ReservationId);
            return View(reservationClient);
        }

        /// <summary> Отваряне на страница за редактиране на резервация-клиент връзка
        /// </summary>
        // GET: ReservationClients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationClient = await _context.ReservationClients
                .Include(r => r.Client)
                .Include(r => r.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationClient == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "firstName", reservationClient.ClientId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", reservationClient.ReservationId);
            return View(reservationClient);
        }

        /// <summary> Редактиране на резервация-клиент връзка
        /// </summary>
        // POST: ReservationClients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,ClientId")] ReservationClient reservationClient)
        {
            /*
            if (id != reservationClient.Id)
            {
                return NotFound();
            }*/

            if (ModelState.IsValid)
            {
                // Remove the existing reservation client from the database
                var reservationClientOld = await _context.ReservationClients
                    .Include(r => r.Client)
                    .Include(r => r.Reservation)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (reservationClientOld == null)
                {
                    return NotFound();
                }

                _context.ReservationClients.Remove(reservationClientOld);
                await _context.SaveChangesAsync();

                // Add the new reservation client to the database
                _context.Add(reservationClient);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "firstName", reservationClient.ClientId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", reservationClient.ReservationId);
            return View(reservationClient);
        }


        /// <summary> Отваряне на страница за изтриване на дадена резервация-клиент връзка
        /// </summary>
        // GET: ReservationClients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationClient = await _context.ReservationClients
                .Include(r => r.Client)
                .Include(r => r.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationClient == null)
            {
                return NotFound();
            }

            return View(reservationClient);
        }

        /// <summary> Изтриване на дадена резерация-клиент връзка
        /// </summary>
        // POST: ReservationClients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservationClient = await _context.ReservationClients
     .Include(r => r.Client)
     .Include(r => r.Reservation)
     .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationClient == null)
            {
                return NotFound();
            }
            _context.ReservationClients.Remove(reservationClient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>Проверка дали резерация-клиент връзката съществува
        /// </summary>
        private bool ReservationClientExists(int id)
        {
            return _context.ReservationClients.Any(e => e.ReservationId == id);
        }
    }
}
