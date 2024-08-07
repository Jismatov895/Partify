﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Partify.Service.Exceptions;
using Partify.Web.Models.Facilities;
using Partify.Web.WebServices.Facilities;

namespace Partify.Web.Controllers;

[Authorize]
public class FacilitiesController(IFacilityWebService facilityWebService) : Controller
{
    public async ValueTask<IActionResult> Index()
    {
        var facilities = await facilityWebService.GetAllAsync();
        return View(facilities);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async ValueTask<IActionResult> Create(FacilityCreateModel createModel)
    {
        try
        {
            var result = await facilityWebService.CreateAsync(createModel);
            if (result is not null)
                return RedirectToAction("Index");

            return View();
        }
        catch(AlreadyExistException ex)
        {
            ViewBag.Exception = ex.Message;
        }

        return View();
    }

    public async ValueTask<IActionResult> Edit(long id)
    {
        var result = await facilityWebService.GetByIdAsync(id);
        return View(result);
    }

    [HttpPost]
    public async ValueTask<IActionResult> Edit(long id, FacilityUpdateModel updateModel)
    {
        var result = await facilityWebService.UpdateAsync(id, updateModel);
        if (result is not null)
            return RedirectToAction("Index");

        return View();
    }

    public async ValueTask<IActionResult> Delete(long id)
    {
        var result = await facilityWebService.DeleteAsync(id);
        if (result)
            return RedirectToAction("Index");

        return View();
    }
}
