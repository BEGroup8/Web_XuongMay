using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Web_XuongMay.Data;
using Web_XuongMay.Models;
using Catagory = Web_XuongMay.Data.Catagory;

namespace Web_XuongMay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatagoryController : ControllerBase
    {
        public static List<Catagory> catagories = new List<Catagory>();

        [HttpGet]
        public ActionResult GetAll()
        {
            return Ok(catagories);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                var catagory = catagories.SingleOrDefault(hh => hh.Mahh == Guid.Parse(id));
                if (catagory == null)
                {
                    return NotFound();
                }
                return Ok(catagory);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create(CatagoryVM catagoryVM)
        {
            var catagory = new Catagory
            {
                Mahh = Guid.NewGuid(),
                Tenhang = catagoryVM.Tenhang,
                DonGia = catagoryVM.DonGia,
            };
            catagories.Add(catagory);
            return Ok(new
            {
                Success = true,
                Data = catagories
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, CatagoryVM catagoryVM)
        {
            try
            {
                var catagory = catagories.SingleOrDefault(hh => hh.Mahh == Guid.Parse(id));
                if (catagory == null)
                {
                    return NotFound();
                }

                catagory.Tenhang = catagoryVM.Tenhang;
                catagory.DonGia = catagoryVM.DonGia;

                return Ok(new
                {
                    Success = true,
                    Data = catagory
                });
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var catagory = catagories.SingleOrDefault(hh => hh.Mahh == Guid.Parse(id));
                if (catagory == null)
                {
                    return NotFound();
                }

                catagories.Remove(catagory);

                return Ok(new
                {
                    Success = true,
                    Message = "Deleted successfully"
                });
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
