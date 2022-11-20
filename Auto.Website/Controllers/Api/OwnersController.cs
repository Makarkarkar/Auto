using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Messages;
using Auto.Website.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

namespace Auto.Website.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController: ControllerBase
    {
        private readonly IAutoDatabase db;
        private readonly IBus bus;
        
        public OwnersController(IAutoDatabase db, IBus bus)
        {
            this.db = db;
            this.bus = bus;
        }
        
        private dynamic Paginate(string url, int index, int count, int total) {
            dynamic links = new ExpandoObject();
            links.self = new { href = url };
            links.final = new { href = $"{url}?index={total - (total % count)}&count={count}" };
            links.first = new { href = $"{url}?index=0&count={count}" };
            if (index > 0) links.previous = new { href = $"{url}?index={index - count}&count={count}" };
            if (index + count < total) links.next = new { href = $"{url}?index={index + count}&count={count}" };
            return links;
        }
        
        [HttpGet]
        [Produces("application/hal+json")]
        public IActionResult Get(int index = 0, int count = 5) {
            var items = db.ListOwners().Skip(index).Take(count);
            var total = db.CountOwners();
            var _links = Paginate("/api/owners", index, count, total);
            var _actions = new {
                create = new {
                    method = "POST",
                    type = "application/json",
                    name = "Create a new owner",
                    href = "/api/owners"
                },
                delete = new {
                    method = "DELETE",
                    name = "Delete a owner",
                    href = "/api/owners/{id}"
                }
            };
            var result = new {
                _links, _actions, index, count, total, items
            };
            return Ok(result);
        }
        [HttpGet("{email}")]
        public IActionResult Get(string email) {
            var owner = db.FindOwner(email);
            if (owner == default) return NotFound();
            var json = owner.ToDynamic();
            json._links = new {
                self = new { href = $"/api/owners/{email}" },
                ownerVehicle = new { href = $"/api/vehicles/{owner.VehicleRegistration}" }
            };
            json._actions = new {
                update = new {
                    method = "PUT",
                    href = $"/api/owners/{email}",
                    accept = "application/json"
                },
                delete = new {
                    method = "DELETE",
                    href = $"/api/owners/{email}"
                }
            };
            return Ok(json);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OwnerDto dto) {
            var ownerVehicle = db.FindVehicle(dto.VehicleRegistration);
            var owner = new Owner {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Age = dto.Age,
                VehicleRegistration = dto.VehicleRegistration,
                OwnerVehicle = ownerVehicle
            };
            db.CreateOwner(owner);
            PublishNewOwnerMessage(owner);
            return Ok(dto);
        }
        
        private void PublishNewOwnerMessage(Owner owner) {
            var message = new NewOwnerMessage {
                Name = owner.Name,
                Surname = owner.Surname,
                Age = owner.Age,
                VehicleRegistration = owner.OwnerVehicle?.Registration,
                Email = owner.Email,
                ListedAtUtc = DateTime.UtcNow
            };
            bus.PubSub.Publish(message);
        }
        
        [HttpPut("{email}")]
        public IActionResult Put(string email, [FromBody] dynamic dto) {
            var ownerVehicleHref = dto._links.ownerVehicle.href;
            var ownerVehicleReg = VehiclesController.ParseRegistration(ownerVehicleHref);
            var ownerVehicle = db.FindVehicle(ownerVehicleReg);
            var owner = new Owner {
                Name = dto.name,
                Surname = dto.surname,
                Age = dto.age,
                VehicleRegistration = ownerVehicle.Registration
            };
            db.UpdateOwner(owner);
            return Get(email);
        }
        
        [HttpDelete("{email}")]
        public IActionResult Delete(string email) {
            var owner = db.FindOwner(email);
            if (owner == default) return NotFound();
            db.DeleteOwner(owner);
            return NoContent();
        }
    }
}