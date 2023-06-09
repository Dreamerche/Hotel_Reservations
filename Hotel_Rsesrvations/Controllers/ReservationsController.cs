﻿using System;
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
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary> Отваряне на таблица със записи за резервации
        /// </summary>
        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reservations.Include(r => r.Room);

            //update totalPrice

            var reservations = _context.Reservations
    .Include(r => r.Room)
    .ToList();
            foreach ( Reservation r in reservations)
            {
 List<ReservationClient> reservationClients = await _context.ReservationClients
    .Include(rc => rc.Client) // Load the associated Client entities
    .Where(rc => rc.ReservationId == r.Id)
    .ToListAsync();
            List<Client> clients = reservationClients.Select(rc => rc.Client).ToList();
            r.totalPrice = calculateTotalCost(r, clients);
            await _context.UpdateReservationClients(r, reservationClients, clients);
            }
           

            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary> Извеждане на детайли за дадена резервация
        /// </summary>
        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            List<ReservationClient> reservationClients = await _context.ReservationClients
                .Include(rc => rc.Client) // Load the associated Client entities
                .Where(rc => rc.ReservationId == reservation.Id)
                .ToListAsync();

            List<Client> clients = reservationClients.Select(rc => rc.Client).ToList();

            reservation.totalPrice = calculateTotalCost(reservation, clients);

            await _context.UpdateReservationClients(reservation, reservationClients, clients);

            return View(reservation);
        }

        /// <summary> Създаване на резервация
        /// </summary>
        // GET: Reservations/Create
        public IActionResult Create()
        {
var rooms = _context.Rooms.Select(r => new
{
    ID = r.ID,
    NUMBER=r.number,
    DisplayText = r.roomType + " - Capacity: " + r.capacity + " - Price for Adult: " + r.priceForAdult + " - Price for Child: " + r.priceForChild
}).ToList();

ViewData["RoomId"] = new SelectList(rooms, "ID", "DisplayText", null, "NUMBER");

            return View();
        }

        /// <summary> Добавяне на резервация
        /// </summary>
        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoomId,checkInDate,vacatingDate,includingBreakfast,isAllInclusive,totalPrice")] Reservation reservation)
        {
            var room = await _context.Rooms
                .Include(r => r.Reservations) 
                    .ThenInclude(r => r.ReservationClients) 
                        .ThenInclude(rc => rc.Client) 
                .FirstOrDefaultAsync(m => m.ID == reservation.RoomId);

            List<Reservation> reservations = _context.Reservations
                    .Where(r => r.RoomId == room.ID)
                    .ToList();

            foreach (Reservation item in reservations)
            {
                if (item.checkInDate<=reservation.checkInDate && item.vacatingDate>=reservation.checkInDate)
                {
                    string message = "The room is already reservated for the period " + item.checkInDate + " - " + item.vacatingDate + ".";
                    ModelState.AddModelError("vacatingDate", message);
                    ViewData["RoomId"] = new SelectList(_context.Rooms, "ID", "roomType", reservation.RoomId);
                    return View(reservation);
                }
                if(item.checkInDate <= reservation.vacatingDate && item.vacatingDate >= reservation.vacatingDate)
                {
                    string message = "The room is already reservated for the period " + item.checkInDate + " - " + item.vacatingDate + ".";
                    ModelState.AddModelError("vacatingDate", message);
                    ViewData["RoomId"] = new SelectList(_context.Rooms, "ID", "roomType", reservation.RoomId);
                    return View(reservation);
                }
                if (item.checkInDate >= reservation.checkInDate && item.vacatingDate <= reservation.vacatingDate)
                {
                    string message = "The room is already reservated for the period " + item.checkInDate + " - " + item.vacatingDate + ".";
                    ModelState.AddModelError("vacatingDate", message);
                    ViewData["RoomId"] = new SelectList(_context.Rooms, "ID", "roomType", reservation.RoomId);
                    return View(reservation);
                }

            }

            if (reservation.vacatingDate < reservation.checkInDate)
            { 
                ModelState.AddModelError("vacatingDate", " Vacating date cannot be earlier than check-in date.");
                ViewData["RoomId"] = new SelectList(_context.Rooms, "ID", "roomType", reservation.RoomId);
                return View(reservation);
            }

            if (reservation.includingBreakfast == true && reservation.isAllInclusive == true)
            {
                ModelState.AddModelError("includingBreakfast", "You can't have both breakfast and all inclusive.");
                ViewData["RoomId"] = new SelectList(_context.Rooms, "ID", "roomType", reservation.RoomId);
                return View(reservation);
            }
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoomId"] = new SelectList(_context.Rooms, "ID", "roomType", reservation.RoomId);
            return View(reservation);
        }

        /// <summary> Отваряне на страница за редактиране на резервация
        /// </summary>
        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var rooms = _context.Rooms.Select(r => new
            {
                ID = r.ID,
                NUMBER = r.number,
                DisplayText = r.roomType + " - Capacity: " + r.capacity + " - Price for Adult: " + r.priceForAdult + " - Price for Child: " + r.priceForChild
            }).ToList();

            ViewData["RoomId"] = new SelectList(rooms, "ID", "DisplayText", reservation.RoomId, "NUMBER");
            return View(reservation);
        }

        /// <summary> Редактиране на резервация
        /// </summary>
        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomId,checkInDate,vacatingDate,includingBreakfast,isAllInclusive,totalPrice")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }
            if (reservation.vacatingDate < reservation.checkInDate)
            {
                ModelState.AddModelError("vacatingDate", "Vacating date cannot be earlier than check-in date.");
                ViewData["RoomId"] = new SelectList(_context.Rooms, "ID", "roomType", reservation.RoomId);
                return View(reservation);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["RoomId"] = new SelectList(_context.Rooms, "ID", "roomType", reservation.RoomId);
            return View(reservation);
        }

        /// <summary> Отваряне на страница за изтриване на дадена резервация
        /// </summary>
        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        /// <summary> Изтриване на дадена резервация
        /// </summary>
        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Проверка дали резервация съществува
        /// </summary>

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
        /// <summary>
        /// Изчисляване на крайната сметка за резервация
        /// </summary>

        private double calculateTotalCost(Reservation reservation, List<Client> clients)
        {
            double totalPrice = 0;
            double priceForAdult = reservation.Room.priceForAdult;
            double priceForChild = reservation.Room.priceForChild;
            foreach (Client c in clients)
            {
                if (c.isAdult == true)
                {
                    totalPrice += priceForAdult;
                }
                else
                {
                    totalPrice += priceForChild;
                }
            }
            totalPrice*= GetNumberOfDays(reservation);
            if (reservation.includingBreakfast)
            {
                return Math.Round(totalPrice * 1.1, 2);
            }
            if (reservation.isAllInclusive)
            {
                return Math.Round(totalPrice * 1.3, 2);
            }
            return totalPrice;
        }
        /// <summary>
        /// Пресмятане на резервираните дни
        /// </summary>
        public int GetNumberOfDays(Reservation reservation)
        {
            TimeSpan span = reservation.vacatingDate - reservation.checkInDate;
            return (int)Math.Ceiling(span.TotalDays);
        }

    }
}

