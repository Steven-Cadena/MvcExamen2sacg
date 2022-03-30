using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcExamen2sacg.Filters;
using MvcExamen2sacg.Models;
using MvcExamen2sacg.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcExamen2sacg.Controllers
{
    public class TicketsController : Controller
    {
        private ServiceLogicAppUsuarios service;

        public TicketsController(ServiceLogicAppUsuarios service) 
        {
            this.service = service;
        }
        public IActionResult MostrarTicketsId() 
        {
            return View();
        }
        [HttpPost]
        [AuthorizeUsuarios]
        public async Task<IActionResult> MostrarTicketsId(string id)
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            List<Ticket> tickets = await this.service.GetTicketsAsync(token, id);
            return View(tickets);
        }

        public IActionResult InsertUsuario() 
        {
            return View();
        }
        [HttpPost]
        [AuthorizeUsuarios]
        public async Task<IActionResult> InsertUsuario(UsuarioTicket usuario)
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            await this.service.CreateUsuarioAsync(usuario.IdUsuario,usuario.Nombre,usuario.Apellidos,usuario.Email,usuario.Username,usuario.Password,token);

            return RedirectToAction("MostrarTicketsId", "Tickets");
        }

        public IActionResult InsertTicket() 
        {
            return View();
        }
        //[HttpPost]
        //[AuthorizeUsuarios]
        //public async Task<IActionResult> InsertTicket(Ticket ticket,IFormFile file) 
        //{
        //    string fileName = file.FileName;
        //    string token = HttpContext.User.FindFirst("TOKEN").Value;
        //    //using (Stream stream = file.OpenReadStream()) 
        //    //{
        //    //    await this.service.CreateTicketAsync(ticket.IdTicket,ticket.IdUsuario, ticket.Fecha, ticket.Importe, ticket.Producto, "",token,fileName, stream);
        //    //}
        //}
    }
}
