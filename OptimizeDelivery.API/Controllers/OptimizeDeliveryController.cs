using System;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Results;
using Common.Models.ApiModels;
using OptimizeDelivery.DataAccessLayer.Services;

namespace OptimizeDelivery.API.Controllers
{
    [RoutePrefix("optimizeDelivery")]
    public class OptimizeDeliveryController : ApiController
    {
        private CourierService CourierService { get; }

        public OptimizeDeliveryController()
        {
            CourierService = new CourierService();
        }
        
        [Route("courier/create")]
        [HttpPost]
        public JsonResult<CreateCourierResult> CreateCourier([FromBody]CreateCourierRequest request)
        {
            try
            {
                var newCourier = CourierService.CreateCourier(request);
                return Json(newCourier);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        [Route("courier/route")]
        [HttpPost]
        public JsonResult<GetRouteResult> GetRoute([FromBody]GetRouteRequest request)
        {
            return Json(CourierService.GetRouteForCourier(request));
        }
    }
}